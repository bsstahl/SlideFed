# SlideFed Publication Process  

This document describes the **full publication pipeline** for SlideFed. It explains how Slides, ContentItems, Decks, and PresentationSessions become **first‑class ActivityStreams objects** and how ActivityPub is used to distribute them, synchronize presentation state, and allow audiences to follow along in real time.

SlideFed treats **every meaningful unit of presentation** as an addressable, federated object with its own URI. This enables decentralized publishing, caching, annotation, reuse, and live presentation updates.

## 1. Overview

Publishing a SlideFed deck is a multi‑stage process:

1. **Create and publish ContentItems**  
2. **Create and publish Slides referencing those ContentItems**  
3. **Create and publish a Deck referencing those Slides**  
4. **Create and publish a PresentationSession (optionally with `startTime`)**  
5. **Audience follows the Presenter or PresentationSession (including before `startTime`)**  
6. **Activate the PresentationSession to `live`**  
7. **Present the first Slide**  
8. **Reveal ContentItems (fragments)**  
9. **Advance/Rewind through Slides**  
10. **Update objects as needed**  
11. **End or cancel the session**

This pipeline ensures that **all content is published once**, and **presentation actions are lightweight ActivityPub messages** referencing already‑published objects.

## 2. Publishing ContentItems

ContentItems are the atomic units of presentation content.  
Examples:

* Text blocks  
* Images  
* Diagrams  
* Code snippets  
* Widgets  
* Reveal fragments  

Each ContentItem is a **first‑class ActivityStreams Object** with its own URI.

### 2.1 Create Activity

To publish a ContentItem:

```
type: Create
actor: Presenter
object: ContentItem
to: Public or Audience
```

This makes the ContentItem:

* dereferenceable  
* cacheable  
* discoverable  
* independently updatable  

All Slides referencing this ContentItem will use its URI.

## 3. Publishing Slides

A Slide is an ActivityStreams Object composed of **ContentItem URIs**.

Example structure:

```
Slide
  name: "Introduction"
  contentItems: [
    https://slidefed.example/items/abc123,
    https://slidefed.example/items/xyz789
  ]
```

### 3.1 Create Activity

To publish a Slide:

```
type: Create
actor: Presenter
object: Slide
to: Public or Audience
```

This makes the Slide:

* dereferenceable  
* cacheable  
* discoverable  
* independently followable through associated presenter/session streams  

Slides do **not** embed content directly; they reference ContentItems.

## 4. Publishing the Deck

A Deck is an ActivityStreams `OrderedCollection` whose `items` are Slide URIs.

Example:

```
Deck
  items: [
    https://slidefed.example/slides/1,
    https://slidefed.example/slides/2,
    https://slidefed.example/slides/3
  ]
```

### 4.1 Create Activity

To publish the Deck:

```
type: Create
actor: Presenter
object: Deck
to: Public or Audience
```

This makes the Deck:

* a stable URI  
* a versionable object  
* something other servers can dereference and reuse  

Like Slides, Decks are reference-based and do not embed object payloads.

---

## 5. Announcing the Deck (Optional)

To broadcast the existence of the deck:

```
type: Announce
actor: Presenter
object: DeckURI
to: Public
```

This is equivalent to “boosting” the deck.

## 6. Audience Follow

Audience members may follow:

* the Presenter  
* the PresentationSession  

Follow ensures they receive:

* Present  
* Reveal  
* Advance  
* Rewind  
* Update  
* Annotate  

activities in real time.

Follow is valid before `startTime` and also during active runtime (`live` and `paused`) so audience can join mid-stream.
Default behavior is permissive: post-start join is allowed.
Restricting post-start join is a possible future session-level option.
New follows are not accepted after a session reaches `ended`.

For MVP, a late follower receives a current session snapshot rather than activity history replay.
Snapshot-plus-history may be introduced later.

To stop receiving updates, audience members use `Undo` against a prior `Follow`.

Deck follow is optional and may be enabled by specific interoperability profiles.

## 7. Starting a PresentationSession

A PresentationSession is an ActivityStreams Object representing a live or asynchronous presentation.

Example:

```
PresentationSession
  deck: DeckURI
  startTime: "2026-10-01T17:00:00Z"
  currentSlide: null
  state: "draft"
  audience: [...]
```

### 7.0 Session Create Activity

```
type: Create
actor: Presenter
object: PresentationSession
to: Audience
```

This publishes the session in advance so audience members can follow it before `startTime`.

### 7.1 Session Activation Update

```
type: Update
actor: Presenter
object: PresentationSession
state: "live"
to: Audience
```

This creates the “room” where presentation actions occur.
The session object itself is created via `Create`, then activated via `Update`.
For MVP, all session state transitions are manually triggered.

### 7.2 Paused Behavior (MVP)

While the session is `paused`, all session commands are paused except unpause and cancel.

### 7.3 Unpause via Undo(Pause)

Unpause may be represented as AS2 `Undo` targeting the prior pause activity.
When unpaused, session state returns to `live`.

## 8. Presenting the First Slide

To begin presenting:

### 8.1 Present Activity

```
type: Present
actor: Presenter
object: Slide1URI
target: PresentationSessionURI
to: Audience
```

Important:

**The slide content is NOT resent.**  
Audience members dereference the Slide URI if needed.

Presentation clients subscribed to the session follow updates and render the appropriate slide as the session advances or reveals content.

---

## 9. Revealing ContentItems (Fragments)

If Slide 1 has fragments:

### 9.1 Reveal Activity

```
type: Reveal
actor: Presenter
object: ContentItemURI
target: SlideURI
to: Audience
```

This reveals a specific ContentItem within the slide.

---

## 10. Advancing to the Next Slide

To move to Slide 2:

### 10.1 Advance Activity

```
type: Advance
actor: Presenter
object: Slide2URI
target: PresentationSessionURI
to: Audience
```

Again, no content is resent.

## 11. Updating Slides or ContentItems

If a slide or content item changes mid‑presentation:

### 11.1 Update Activity

```
type: Update
actor: Presenter
object: SlideURI or ContentItemURI
to: Audience
```

Servers update their cached copy.

## 12. Ending the Session

### 12.1 Session End Update

```
type: Update
actor: Presenter
object: PresentationSessionURI
state: "ended"
to: Audience
```

Session state becomes `"ended"`.

### 12.2 Canceling a Session

Cancellation is represented by:

```
type: Update
actor: Presenter
object: PresentationSessionURI
state: "canceled"
to: Audience
```

Cancellation is allowed from any non-ended state.
For MVP, `canceled` is non-terminal.
From `canceled`, session state may transition to `draft` or `live`.
Reopening after `ended` is not allowed.

## 13. Summary

The SlideFed publication process is:

1. Publish **ContentItems**  
2. Publish **Slides** referencing ContentItems  
3. Publish **Deck** referencing Slides  
4. Publish **PresentationSession** (optionally with `startTime`)  
5. Audience follows Presenter or PresentationSession  
6. Activate session to **live**  
7. Present Slide 1  
8. Reveal ContentItems  
9. Advance/Rewind  
10. Update objects  
11. End or cancel session  

This architecture ensures:

* **slides and content are published once**  
* **presentation actions are lightweight**  
* **everything is addressable and federated**  
* **audiences can follow in real time**  
* **annotations and updates are granular**  

SlideFed becomes a fully decentralized, composable, ActivityPub‑native presentation system.
