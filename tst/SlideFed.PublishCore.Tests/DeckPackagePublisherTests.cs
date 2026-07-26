using SlideFed.PublishCore;

namespace SlideFed.PublishCore.Tests;

public class DeckPackagePublisherTests
{
    [Fact]
    public async Task PublishAsyncThrowsWhenDeckIdentifierIsNotGuid()
    {
        var deck = CreateValidDeck();
        var loader = new StubLoader(deck);
        var activityPublisher = new RecordingActivityPublisher();
        var sut = new DeckPackagePublisher(loader, activityPublisher);

        await Assert.ThrowsAsync<DeckPublishValidationException>(
            () => sut.PublishAsync(new PublishRequest("demo-deck.yaml", "presenter-1")));

        Assert.Empty(activityPublisher.Published);
    }

    [Fact]
    public async Task PublishAsyncAcceptsGuidDeckIdentifier()
    {
        var deck = CreateValidDeck();
        var loader = new StubLoader(deck);
        var activityPublisher = new RecordingActivityPublisher();
        var sut = new DeckPackagePublisher(loader, activityPublisher);

        var deckId = Guid.NewGuid();
        var summary = await sut.PublishAsync(new PublishRequest(deckId.ToString(), "presenter-1"));

        Assert.Equal("deck-1", summary.DeckId);
        Assert.NotEmpty(activityPublisher.Published);
    }

    [Fact]
    public async Task PublishAsyncPublishesCreateInOrder()
    {
        var deck = new SourceDeck(
            "deck-1",
            new[]
            {
                new Slide("slide-1", new[] { new ContentItem("c1"), new ContentItem("c2") }),
                new Slide("slide-2", new[] { new ContentItem("c2"), new ContentItem("c3") }),
                new Slide("slide-3", new[] { new ContentItem("c4") })
            });

        var loader = new StubLoader(deck);
        var activityPublisher = new RecordingActivityPublisher();
        var sut = new DeckPackagePublisher(loader, activityPublisher);

        var summary = await sut.PublishAsync(new PublishRequest(Guid.NewGuid().ToString(), "presenter-1"));

        Assert.Equal(3, summary.ResolvedSlideCount);
        Assert.Equal(4, summary.PublishedContentItemCount);
        Assert.Equal(3, summary.PublishedSlideCount);
        Assert.Equal(8, activityPublisher.Published.Count);

        Assert.Collection(
            activityPublisher.Published,
            item => AssertPublishObject(item, PublishObjectKind.ContentItem, "c1"),
            item => AssertPublishObject(item, PublishObjectKind.ContentItem, "c2"),
            item => AssertPublishObject(item, PublishObjectKind.ContentItem, "c3"),
            item => AssertPublishObject(item, PublishObjectKind.ContentItem, "c4"),
            item => AssertPublishObject(item, PublishObjectKind.Slide, "slide-1"),
            item => AssertPublishObject(item, PublishObjectKind.Slide, "slide-2"),
            item => AssertPublishObject(item, PublishObjectKind.Slide, "slide-3"),
            item => AssertPublishObject(item, PublishObjectKind.Deck, "deck-1"));
    }

    [Fact]
    public async Task PublishAsyncThrowsWhenDeckHasNoSlides()
    {
        var deck = new SourceDeck(
            "deck-1",
            Array.Empty<Slide>());

        var loader = new StubLoader(deck);
        var activityPublisher = new RecordingActivityPublisher();
        var sut = new DeckPackagePublisher(loader, activityPublisher);

        await Assert.ThrowsAsync<DeckPublishValidationException>(
            () => sut.PublishAsync(new PublishRequest(Guid.NewGuid().ToString(), "presenter-1")));
        Assert.Empty(activityPublisher.Published);
    }

    [Fact]
    public async Task PublishAsyncThrowsWhenSlideHasNoContentItems()
    {
        var deck = new SourceDeck(
            "deck-1",
            new[] { new Slide("slide-1", Array.Empty<ContentItem>()) });

        var loader = new StubLoader(deck);
        var activityPublisher = new RecordingActivityPublisher();
        var sut = new DeckPackagePublisher(loader, activityPublisher);

        await Assert.ThrowsAsync<DeckPublishValidationException>(
            () => sut.PublishAsync(new PublishRequest(Guid.NewGuid().ToString(), "presenter-1")));
        Assert.Empty(activityPublisher.Published);
    }

    [Fact]
    public async Task PublishAsyncThrowsWhenContentItemIdIsBlank()
    {
        var deck = new SourceDeck(
            "deck-1",
            new[]
            {
                new Slide("slide-1", new[] { new ContentItem(" ") })
            });

        var loader = new StubLoader(deck);
        var activityPublisher = new RecordingActivityPublisher();
        var sut = new DeckPackagePublisher(loader, activityPublisher);

        await Assert.ThrowsAsync<DeckPublishValidationException>(
            () => sut.PublishAsync(new PublishRequest(Guid.NewGuid().ToString(), "presenter-1")));
        Assert.Empty(activityPublisher.Published);
    }

    private static void AssertPublishObject(PublishObject actual, PublishObjectKind expectedKind, string expectedId)
    {
        Assert.Equal(expectedKind, actual.Kind);
        Assert.Equal(expectedId, actual.Id);
    }

    private static SourceDeck CreateValidDeck()
    {
        return new SourceDeck(
            "deck-1",
            new[]
            {
                new Slide("slide-1", new[] { new ContentItem("c1") })
            });
    }

    private sealed class StubLoader(SourceDeck deck) : IDeckSourceLoader
    {
        public Task<SourceDeck> LoadAsync(string deckIdentifier, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(deck);
        }
    }

    private sealed class RecordingActivityPublisher : IActivityPublisher
    {
        public List<PublishObject> Published { get; } = new();

        public Task PublishCreateAsync(PublishObject activityObject, CancellationToken cancellationToken = default)
        {
            Published.Add(activityObject);
            return Task.CompletedTask;
        }
    }
}
