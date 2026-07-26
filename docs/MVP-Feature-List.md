# SlideFed MVP Feature List

This document tracks MVP delivery scope as a feature backlog.

## MVP Principles

- Prefer standard ActivityStreams and ActivityPub semantics first.
- Keep transitions and operations explicit and manually triggered for MVP.
- Preserve URI-only references for Slide and Deck composition.
- Support snapshot-only late join bootstrap in MVP.
- Build features using TDD: write tests first for an agreed feature slice, review/approve tests, implement until tests pass, then decide on refactoring.
- Implement all code in C# with .NET analyzers enabled at the strictest level and warnings treated as errors.

## TDD Approval Gate (Mandatory)

This process is mandatory for every feature slice.

1. Create or update tests only.
2. Share the proposed tests for review.
3. Wait for explicit approval of the tests.
4. Implement production code only after that approval.
5. Run tests and share results for implementation approval.
6. Refactor only after implementation approval.

Hard rule: no production code changes are allowed before explicit test approval.

## Feature 1: Publish a Complete Deck Package

### Goal

Allow a presenter (or publishing tool) to publish a complete deck package (Deck, Slides, and ContentItems) so all referenced objects are first-class ActivityStreams objects that remote servers and local clients can dereference.

### In Scope (MVP)

- Accept a publish request for an existing source Deck and presenter actor.
- Resolve IncludeBlocks before publication; each IncludeBlock is expanded to a concrete ordered collection of Slides.
- Publish all ContentItems referenced by the resolved Slides.
- Publish all resolved Slides.
- Publish the resolved Deck as an ActivityStreams `OrderedCollection` of Slide URIs.
- Emit ActivityStreams `Create` activities for ContentItems, Slides, and Deck.
- Deliver outbound federation work items through the worker path.
- Expose status for success and failure of publication delivery.

### Out of Scope (MVP)

- Update semantics for Decks, Slides, or ContentItems.
- Delete semantics for Decks, Slides, or ContentItems.
- Rich policy or permissions model beyond basic presenter ownership assumptions.
- Advanced bulk publish orchestration across multiple Decks.
- Full history replay for newly joined followers.

### Primary User Story

As a presenter, I can publish one complete deck package so that the Deck and all referenced Slides and ContentItems are available as federated objects.

### Acceptance Criteria

1. Given a Deck with IncludeBlocks, when publication is requested, then IncludeBlocks are expanded into a concrete ordered list of Slides before publish activities are emitted.
2. Given a valid resolved Deck, when publication is requested, then the system emits `Create` for every ContentItem, then every Slide, then the Deck.
3. Given any invalid or unresolved IncludeBlock, Slide, or ContentItem reference, when publication is requested, then the request fails with a clear validation result and no publish activities are emitted.
4. Given federation delivery failures, when retries are exhausted, then publication status includes failure details for operators.

### API and Contract Slice

- `POST /decks/{deckId}/publish`
- Request fields:
  - `actor`
  - `to` (optional delivery audience override)
- Response fields:
  - `deck`
  - `resolvedSlideCount`
  - `publishedContentItemCount`
  - `publishedSlideCount`
  - `activityType` (`Create`)
  - `publishedAt`
  - `deliveryStatus`

### Implementation Tasks

1. Add deck-package publication command and handler in the Publication Application Service.
2. Add IncludeBlock resolver that expands to concrete ordered Slides.
3. Add reference integrity checks for resolved Slides and ContentItems.
4. Add create-only publication pipeline for ContentItems, Slides, then Deck.
5. Add event emission to Projection Trigger and Federation Gateway.
6. Add worker command generation for outbound fanout.
7. Add publication status query surface for operator and tool feedback.
8. Add automated tests for include resolution, happy path, invalid references, and delivery-failure states.

### Implementation Note

- The LiquidVictor.Data.YamlFile package is available for loading the source slide deck and related assets during the publish flow.

### Done Definition

- All acceptance criteria pass in automated tests.
- Publication appears in read model views used by presentation tools.
- Federation work is queued and observable.
- Errors are deterministic and documented.
- Update and delete operations are explicitly rejected for this feature path.

## Next MVP Features (Candidate Order)

1. Create and publish PresentationSession with optional `startTime`.
2. Manual session lifecycle updates (`draft`, `live`, `paused`, `canceled`, `ended`).
3. Follow and Undo(Follow) for Presenter and PresentationSession.
4. Snapshot-only late join bootstrap for presentation clients.
5. Present, Advance/Rewind, and Reveal activity flow for live session rendering.
6. Announce published Decks as an explicit distribution story.
