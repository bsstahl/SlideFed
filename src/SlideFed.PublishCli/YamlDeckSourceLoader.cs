using SlideFed.PublishCore;
using LiquidVictor.Data.YamlFile;
using LiquidVictor.Exceptions;
using YamlDotNet.Core;

namespace SlideFed.PublishCli;

internal sealed class YamlDeckSourceLoader(string repositoryPath) : IDeckSourceLoader
{
    private readonly SlideDeckReadRepository _readRepository = new(ValidateRepositoryPath(repositoryPath));

    public Task<SourceDeck> LoadAsync(string deckIdentifier, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(deckIdentifier);

        if (!Guid.TryParse(deckIdentifier, out var deckId))
        {
            throw new DeckPublishValidationException("Deck identifier must be a GUID.");
        }

        cancellationToken.ThrowIfCancellationRequested();

        LiquidVictor.Entities.SlideDeck deck;
        try
        {
            deck = _readRepository.GetSlideDeck(deckId);
        }
        catch (SlideDeckNotFoundException ex)
        {
            throw new DeckPublishValidationException($"Slide deck '{deckId}' was not found in repository.", ex);
        }
        catch (YamlException ex)
        {
            throw new DeckPublishValidationException(
                $"Slide deck '{deckId}' could not be parsed by LiquidVictor.Data.YamlFile.",
                ex);
        }

        ArgumentNullException.ThrowIfNull(deck);

        var deckItems = deck.Slides
            .OrderBy(pair => pair.Key)
            .Select(pair => MapSlide(pair.Value))
            .ToList();

        return Task.FromResult(new SourceDeck(deck.Id.ToString(), deckItems));
    }

    private static Slide MapSlide(LiquidVictor.Entities.Slide source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var contentItems = source.ContentItems
            .OrderBy(pair => pair.Key)
            .Select(pair => pair.Value)
            .Where(item => item is not null)
            .Select(item => new ContentItem(item.Id.ToString()))
            .ToList();

        return new Slide(source.Id.ToString(), contentItems);
    }

    private static string ValidateRepositoryPath(string repositoryPath)
    {
        return string.IsNullOrWhiteSpace(repositoryPath)
            ? throw new ArgumentException("Repository path is required.", nameof(repositoryPath))
            : repositoryPath;
    }
}
