# SlideFed Ubiquitous Language

A shared vocabulary for the SlideFed ecosystem, combining **LiquidVictor presentation-domain concepts** with **Fediverse ActivityStreams / ActivityPub semantics** for publication and interaction. This ubiquitous language defines the nouns, verbs, and relationships that form the conceptual backbone of SlideFed’s federated presentation model.

It is designed for clarity, interoperability, and implementation in JSON‑LD, .NET models, and ActivityPub endpoints.

# 1. Core Concepts

## Actor
An entity capable of performing activities. Typically a presenter, viewer, or automated system.  
Examples: human user, SlideFed server, bot.

## Audience
A collection of actors who receive presentation updates.  
May be followers or participants in a session.

## PresentationSession
A live or asynchronous event during which a deck is presented.  
Tracks current slide, audience, session state, and optional scheduled start.

**Properties**
* `type: "PresentationSession"`
* `deck` — Deck URI
* `state` — `draft | live | paused | ended | canceled`
* `startTime` — optional RFC 3339 timestamp for scheduled start
* `currentSlide` — Slide URI or null
* `audience` — audience collection

State guidance:
* Canonical states: `draft | live | paused | ended | canceled`
* `ended` is terminal
* `canceled` is allowed from any non-ended state
* `canceled` is non-terminal in MVP
* `canceled` may transition to `draft` or `live`
* Sessions may be created and followed before `startTime`

# 2. Presentation Objects (Nouns)

## Slide
A discrete unit of presentation content.  
A Slide is an ActivityStreams Object with its own URI.

**Properties**
* `type: "Slide"`
* `name`
* `contentItems` — ordered list of ContentItem URIs
* `notes` — SpeakerNotes
* `order`
* `version`
* `attributedTo` — Actor who created or maintains it

Slides are reference-based containers and do not embed ContentItem bodies.

## Deck
An ordered collection of slides.  
Modeled as an ActivityStreams OrderedCollection.

**Properties**
* `type: "Deck"`
* `items` — ordered list of Slide URIs
* `name`
* `description`
* `theme`
* `version`

Decks are reference-based containers and do not embed Slide bodies.

## ContentItem
A single piece of content within a slide.  
Aligned with LiquidVictor’s terminology.

**Properties**
* `type: "ContentItem"`
* `role` — `"text" | "image" | "video" | "diagram" | "code" | "widget" | "fragment"`
* `body` — HTML, Markdown, JSON-LD, URL, or structured data
* `metadata`
* `fragmentIndex` — for reveal sequencing

## SpeakerNotes
Presenter-only notes attached to a slide.

**Properties**
* `type: "SpeakerNotes"`
* `content`

## Annotation
Markup or comments applied to a slide or content item.

**Properties**
* `type: "Annotation"`
* `target` — slide or content item
* `body`
* `author`

## Transition
A visual or logical change between slides or content items.  
May be implicit (advance) or explicit (custom animation).

# 3. Activities (Verbs)

Activities are ActivityStreams Activities performed by Actors on Objects.
SlideFed prefers standard AS2/AP verbs first and introduces custom activity extensions only when they are strictly necessary.

## Create
Actor authors a new slide, deck, or content item.

## Update
Actor modifies an existing slide, deck, or content item.

## Add
Actor inserts a slide into a deck or a content item into a slide.

## Remove
Actor removes a slide or content item.

## Present
Actor displays a slide to an audience.

## Advance
Actor moves to the next slide.

## Rewind
Actor moves to the previous slide.

## Reveal
Actor exposes hidden content items (fragments).

## Annotate
Actor adds markup or comments to a slide or content item.

## Publish
Actor makes a slide or deck discoverable.

## Announce
Actor broadcasts the existence of a slide, deck, or session.

## Session lifecycle
By default, session lifecycle is represented with `Create` (session creation) and `Update` (state transitions such as live, paused, ended, canceled).

Scheduled sessions are created with optional `startTime`, then activated to `live` via `Update` at or after the scheduled time.

For MVP, state transitions are manually triggered. Optional automatic transitions are a future capability.

`StartSession` and `EndSession` are optional SlideFed extension activities, used only when explicit event verbs are required for interoperability profiles.

Paused behavior (MVP): while `paused`, all session commands are paused except unpause and cancel.

Unpause semantics: unpause may be represented as AS2 `Undo` of the prior pause activity.

## Follow
Audience follows a presenter or presentation session to receive updates.

Follow is valid before `startTime` for scheduled sessions.
Follow is also valid after start during `live` and `paused` states so audience can join mid-stream.
By default, post-start join is allowed; restricting it is a possible future session-level option.
New follows are not accepted after a session reaches `ended`.

Late follow bootstrap (MVP) is snapshot-only, not history replay.
Snapshot-plus-history is a possible future behavior.

## Undo

Audience can undo a Follow relationship (unfollow semantics).
Session operators can also use `Undo` against a prior pause activity to unpause.

Deck follow may be supported by specific profiles, but is not required in the default model.

# 4. Relationships

## Slide → Deck
A deck contains slides in a defined order.  
Slides may belong to multiple decks.

## Slide → ContentItem
A slide is composed of one or more content items.

## Slide → SpeakerNotes
Notes are attached to a slide but not visible to the audience.

## Slide / ContentItem → Annotation
Annotations target either a slide or a specific content item.

## PresentationSession → Deck
A session presents a specific deck.

## PresentationSession → Audience
Audience members receive updates from the session.

# 5. ActivityPub Mapping

SlideFed uses ActivityStreams objects and ActivityPub activities:

* **Slides, Decks, ContentItems** → AS2 Objects  
* **Create, Update, Add, Present, Reveal, etc.** → AS2 Activities  
* **PresentationSession** → AS2 Object with state transitions  
* **Audience** → Followers / Collections  
* **Announcements** → `Announce` activities  
* **Publishing** → `Create` + delivery to inboxes/outboxes  

All custom types are valid AS2 extensions.

# 6. Design Principles

* **Federation-first** — Every slide, deck, and session is addressable and portable.
* **Extensible** — New content types and activities can be added without breaking compatibility.
* **Composable** — Decks, slides, and content items form a clean hierarchy.
* **Observable** — Presentation actions are ActivityPub activities.
* **Interoperable** — JSON-LD context ensures consistent semantics across implementations.

# 7. Status

This ubiquitous language is the foundation for:

* SlideFed JSON-LD vocabulary  
* SlideFed ActivityPub server  
* SlideFed client SDKs  
* Deck editing and presentation tools  

It will evolve as the protocol and implementation mature.
