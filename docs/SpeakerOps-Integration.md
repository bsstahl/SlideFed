# SpeakerOps Integration

SlideFed is the SpeakerOps bounded context responsible for publishing built presentations into the Fediverse.

This document records SlideFed's integration boundary inside the wider SpeakerOps system. SlideFed's core domain remains ActivityPub/ActivityStreams publication and interaction; SpeakerOps does not change that boundary.

## Role in SpeakerOps

SlideFed begins after a presentation artifact has been built. It consumes built deck data from LiquidVictor and turns that presentation into federated resources and activities.

Within SpeakerOps, SlideFed is responsible for:

- publishing decks, slides, content items, speaker notes, sessions, and annotations as federated resources
- assigning SlideFed-owned URIs to published resources
- exposing ActivityPub endpoints and JSON-LD vocabulary
- delivering presentation/session activity to Fediverse servers and followers
- managing federated session state such as `draft`, `live`, `paused`, `ended`, and `canceled`

## Upstream Boundary

LiquidVictor owns the source presentation artifact. Its existing `SlideDeck.Id` is the upstream identity for a built deck.

SlideFed may retain provenance back to the originating `SlideDeck.Id`, but LiquidVictor does not need any field that points to SlideFed publication state.

```text
LiquidVictor SlideDeck.Id -> SlideFed published Deck URI
```

The published Deck URI is owned by SlideFed. The source `SlideDeck.Id` remains owned by LiquidVictor.

## Downstream / Consumer Boundary

TalkFolio and TalkCircuit may use SlideFed outputs when they need public or federated presentation references, such as:

- a public deck URL shown in a talk catalog
- a federated PresentationSession URI associated with a delivery record
- a publication URL included in post-conference follow-up material

SlideFed should expose these as normal published resource identifiers. It should not depend on TalkFolio or TalkCircuit's internal models.

## Explicitly Out of Scope

SlideFed does not own:

- talk categories or tags
- PresentationFamily grouping
- CFP-specific tag mapping
- abstracts, elevator pitches, target-audience statements, or selection-committee memos
- conference submission state
- acceptance/rejection status
- non-federated delivery history

Those concerns belong to TalkFolio or TalkCircuit.

## Integration Principle

SlideFed publishes presentation artifacts and sessions. It does not decide which talks should exist, which talks should be submitted, whether talks are in the same family, or whether a conference is a good fit.

Reporting and composition tools may combine SlideFed publication data with LiquidVictor, TalkFolio, and TalkCircuit data, but SlideFed's own model should remain focused on federation and presentation interaction.
