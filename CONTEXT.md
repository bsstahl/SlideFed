# SlideFed Context

Glossary-only domain language for SlideFed. This file records canonical terms and invariants, not implementation details.

## Term: Slide
Definition: A discrete presentation unit identified by its own URI.
Composition: A Slide contains an ordered list of ContentItem URIs only.
Distinguish from: ContentItem payload data, which is not embedded in the Slide.
Invariants:
- A Slide references ContentItems by URI.
- A Slide does not embed ContentItem bodies.
- Slide content retrieval is by dereferencing referenced ContentItem URIs.

## Term: Deck
Definition: A presentation sequence identified by its own URI.
Type: OrderedCollection of Slide URIs.
Composition: A Deck contains an ordered list of Slide URIs only.
Distinguish from: Slide content, which is not embedded in the Deck.
Invariants:
- A Deck preserves slide order as domain-significant.
- A Deck references Slides by URI.
- A Deck does not embed Slide bodies.

## Relationship: Deck -> Slide
Definition: Deck membership is reference-based and ordered.
Invariants:
- Membership is by Slide URI.
- Order changes are meaningful domain events.

## Relationship: Slide -> ContentItem
Definition: Slide composition is reference-based and ordered.
Invariants:
- Membership is by ContentItem URI.
- Reveal and sequencing behavior operates over referenced ContentItems.

## Term: Domain Language Boundary
Definition: SlideFed uses two complementary semantic vocabularies.
Presentation language: LiquidVictor terms for modeling deck, slide, and content composition.
Publication/interaction language: Fediverse ActivityStreams/ActivityPub semantics for distribution and user interactions.
Invariants:
- Presentation modeling terms follow LiquidVictor language.
- Federation and social interaction verbs follow AS/AP terminology.

## Term: Follow
Definition: Fediverse action where an Actor requests to receive updates from another Actor or object stream.
Distinguish from: Informal "subscribe" wording.
Invariants:
- Follow is the canonical interaction verb.

## Term: Undo(Follow)
Definition: Fediverse action that reverses an existing Follow relationship.
Distinguish from: Informal "unsubscribe" wording.
Invariants:
- Unfollow semantics are represented as Undo of Follow.

## Term: Activity Extension Policy
Definition: Prefer standard AS2/AP verbs first and add SlideFed-specific activity extensions only when required.
Invariants:
- New custom activity types are introduced only when existing AS2/AP verbs cannot express required behavior.
- Session lifecycle should default to Create/Update semantics unless an explicit extension is justified.

## Term: Follow Scope
Definition: Required follow targets are Presenter and PresentationSession.
Invariants:
- Presenter follow is canonical.
- PresentationSession follow is canonical.
- Deck follow is optional and remains profile-dependent.
- Follow of PresentationSession is valid from pre-start through active runtime.
- New follows are not accepted after a session reaches `ended`.

## Term: PresentationSession State
Definition: Session lifecycle state for a PresentationSession object.
Canonical states: draft, live, paused, ended, canceled.
Invariants:
- A PresentationSession may be created in advance with a `startTime`.
- `startTime` enables audience follow before the session begins.
- Mid-stream join via Follow is permitted by default once the session is live.
- ended is terminal.
- canceled is permitted from any non-ended state.
- canceled is non-terminal for MVP.
- canceled may transition to `draft` or `live`.
- Reopening after ended is disallowed.

## Term: State Transition Triggering
Definition: How session state transitions are initiated.
Invariants:
- For MVP, all state transitions are manually triggered.
- Optional automatic transitions may be introduced later.

## Term: Join Policy
Definition: Session-level policy controlling whether new followers may join after start.
Invariants:
- Default policy is permissive: joining after start is allowed.
- Restricting post-start join is a possible future capability.
- No policy can allow join after `ended`.

## Term: Late Follow Bootstrap
Definition: Initial information delivered when a new follower joins an already-active session.
Invariants:
- MVP behavior is snapshot-only (current session state and current slide context).
- MVP does not require delivery of recent activity history.
- Snapshot-plus-history is a possible future behavior.

## Term: Paused Command Handling
Definition: Allowed command behavior while a session is in `paused` state.
Invariants:
- During `paused`, all session commands are paused except unpause and cancel.
- This rule is MVP-scoped and expected to be revisited with production usage.

## Term: Unpause Semantics
Definition: How a paused session resumes.
Invariants:
- Unpause may be expressed as an AS2 `Undo` targeting the prior pause activity.
- Session state is restored to `live` when unpaused.

## Term: Permissions Scope
Definition: Authorization and role policy for who may issue session-changing actions.
Invariants:
- Permissions are out of scope for the current domain model iteration.
- Permissions will be defined in a dedicated standards track.

## Term: PresentationSession startTime
Definition: Scheduled session start timestamp on a PresentationSession.
Invariants:
- `startTime` is an RFC 3339 timestamp.
- Sessions may be published and followed well before `startTime`.
- Activation to `live` occurs through an `Update` at or after `startTime`.

## Open Questions
- Defer clock-authority decisions until they are needed beyond manual triggering.
- Defer join-policy field naming and value vocabulary until last responsible moment.
- Defer audience channel/signal separation design until implementation needs are clearer.
- Consider Deck follow in vNext interoperability profile.
