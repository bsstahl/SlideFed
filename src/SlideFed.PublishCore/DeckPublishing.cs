namespace SlideFed.PublishCore;

public sealed record PublishRequest(string DeckIdentifier, string Actor);

public sealed record PublishSummary(
    string DeckId,
    int ResolvedSlideCount,
    int PublishedContentItemCount,
    int PublishedSlideCount);

public enum PublishObjectKind
{
    ContentItem,
    Slide,
    Deck
}

public sealed record PublishObject(PublishObjectKind Kind, string Id);

public sealed record ContentItem(string Id);

public sealed record Slide(string Id, IReadOnlyList<ContentItem> ContentItems);

public sealed record SourceDeck(string Id, IReadOnlyList<Slide> Slides);

public interface IDeckSourceLoader
{
    Task<SourceDeck> LoadAsync(string deckIdentifier, CancellationToken cancellationToken = default);
}

public interface IActivityPublisher
{
    Task PublishCreateAsync(PublishObject activityObject, CancellationToken cancellationToken = default);
}

public sealed class DeckPublishValidationException : Exception
{
    public DeckPublishValidationException()
    {
    }

    public DeckPublishValidationException(string message)
        : base(message)
    {
    }

    public DeckPublishValidationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

public sealed class DeckPackagePublisher(IDeckSourceLoader loader, IActivityPublisher activityPublisher)
{
    public async Task<PublishSummary> PublishAsync(PublishRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ValidateRequest(request);

        var sourceDeck = await loader.LoadAsync(request.DeckIdentifier, cancellationToken).ConfigureAwait(false);
        ValidateDeck(sourceDeck);

        ValidateSlides(sourceDeck.Slides);

        var contentItemsToPublish = GetDistinctContentItemsInOrder(sourceDeck.Slides);

        foreach (var contentItem in contentItemsToPublish)
        {
            await activityPublisher
                .PublishCreateAsync(new PublishObject(PublishObjectKind.ContentItem, contentItem.Id), cancellationToken)
                .ConfigureAwait(false);
        }

        foreach (var slide in sourceDeck.Slides)
        {
            await activityPublisher
                .PublishCreateAsync(new PublishObject(PublishObjectKind.Slide, slide.Id), cancellationToken)
                .ConfigureAwait(false);
        }

        await activityPublisher
            .PublishCreateAsync(new PublishObject(PublishObjectKind.Deck, sourceDeck.Id), cancellationToken)
            .ConfigureAwait(false);

        return new PublishSummary(
            sourceDeck.Id,
            sourceDeck.Slides.Count,
            contentItemsToPublish.Count,
            sourceDeck.Slides.Count);
    }

    private static void ValidateRequest(PublishRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.DeckIdentifier))
        {
            throw new DeckPublishValidationException("Deck identifier is required.");
        }

        if (!Guid.TryParse(request.DeckIdentifier, out _))
        {
            throw new DeckPublishValidationException("Deck identifier must be a GUID.");
        }

        if (string.IsNullOrWhiteSpace(request.Actor))
        {
            throw new DeckPublishValidationException("Actor is required.");
        }
    }

    private static void ValidateDeck(SourceDeck sourceDeck)
    {
        if (string.IsNullOrWhiteSpace(sourceDeck.Id))
        {
            throw new DeckPublishValidationException("Deck id is required.");
        }

        if (sourceDeck.Slides.Count == 0)
        {
            throw new DeckPublishValidationException("Deck must contain at least one Slide.");
        }
    }

    private static void ValidateSlides(IReadOnlyList<Slide> slides)
    {
        if (slides.Count == 0)
        {
            throw new DeckPublishValidationException("Deck must resolve to at least one Slide.");
        }

        foreach (var slide in slides)
        {
            if (string.IsNullOrWhiteSpace(slide.Id))
            {
                throw new DeckPublishValidationException("Slide id is required.");
            }

            if (slide.ContentItems.Count == 0)
            {
                throw new DeckPublishValidationException($"Slide '{slide.Id}' must contain at least one ContentItem.");
            }

            foreach (var contentItem in slide.ContentItems)
            {
                if (string.IsNullOrWhiteSpace(contentItem.Id))
                {
                    throw new DeckPublishValidationException($"Slide '{slide.Id}' contains a ContentItem with an invalid id.");
                }
            }
        }
    }

    private static List<ContentItem> GetDistinctContentItemsInOrder(IReadOnlyList<Slide> resolvedSlides)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var ordered = new List<ContentItem>();

        foreach (var slide in resolvedSlides)
        {
            foreach (var contentItem in slide.ContentItems)
            {
                if (seen.Add(contentItem.Id))
                {
                    ordered.Add(contentItem);
                }
            }
        }

        return ordered;
    }
}
