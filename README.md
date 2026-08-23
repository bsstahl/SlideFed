# SlideFed

A federated presentation platform built on **ActivityPub** and **ActivityStreams**, where slides, decks, and content items are first‑class, addressable, remixable objects. SlideFed brings presentations into the Fediverse by treating every slide as an ActivityStreams object and every action—creating, updating, presenting, revealing—as an ActivityPub activity.

SlideFed is designed for interoperability, decentralization, and extensibility. It provides a shared vocabulary, a JSON‑LD context, and a set of ActivityPub endpoints for publishing and presenting content across servers.

## Code of Conduct

All contributors and users are expected to follow the [Strict Accountability Policy](CODE_OF_CONDUCT.md).

## Features

* **Federated Slides** — Each slide is an ActivityStreams object with its own URI.
* **Decks as Ordered Collections** — Ordered collections of slide URIs, fully addressable and versioned.
* **Content Items** — Text, images, diagrams, code blocks, and widgets represented as structured objects.
* **Presentation Sessions** — Live or asynchronous sessions, including scheduled sessions with a start time, modeled as ActivityPub activities.
* **Presentation Clients** — Clients that follow a session and display the appropriate slide as updates arrive.
* **Reveal & Advance Activities** — Fine‑grained control over slide fragments and navigation.
* **Annotations** — Audience or presenter markup stored as ActivityStreams objects.
* **Extensible Vocabulary** — Custom object and activity types built on AS2 and JSON‑LD.

## Vocabulary Overview

SlideFed defines a split ubiquitous language: LiquidVictor terminology for presentation modeling and Fediverse (ActivityStreams/ActivityPub) semantics for publication, interaction, and usage.

### Core Object Types

* **Actor** — Presenter or automated system.
* **Slide** — A single unit of presentation content.
* **Deck** — Ordered collection of slides.
* **ContentItem** — Text, image, video, diagram, code, or widget.
* **SpeakerNotes** — Presenter‑only notes.
* **PresentationSession** — Live, asynchronous, or scheduled presentation event.
* **PresentationClient** — Client application that follows a session and renders the active slide.
* **Annotation** — Markup or comments applied to slides or content items.

### Core Activity Types

* **Create** — Author a slide, deck, or content item.
* **Update** — Modify a slide or deck.
* **Add / Remove** — Insert or remove slides or content items.
* **Present** — Display a slide.
* **Advance / Rewind** — Navigate between slides.
* **Reveal** — Expose hidden content items.
* **Annotate** — Add markup or comments.
* **Publish / Announce** — Make content discoverable.
* **Session Lifecycle via Create / Update** — Manage session state transitions using AS2 verbs by default.
* **StartSession / EndSession (Optional Extensions)** — Used only when explicit event verbs are required.
* **Follow / Undo** — Audience follows presenters or presentation sessions and can undo that relationship.

## Goals

* Provide a **standard vocabulary** for federated presentation content.
* Enable **interoperable presentation tools** across the Fediverse.
* Support **real‑time presentation state** via ActivityPub.
* Allow **fine‑grained content manipulation** through structured objects.
* Encourage **decentralized publishing** of decks and slides.

## Architecture

See the C4 architecture set index at [docs/C4-Index.md](docs/C4-Index.md).

## SpeakerOps Integration

SlideFed is one independent product within the wider SpeakerOps ecosystem. Its local integration boundary is documented in [docs/SpeakerOps-Integration.md](docs/SpeakerOps-Integration.md).

## Implementation Standards

- All implementation code for this project is written in C#.
- .NET analyzers are enabled in their strictest mode.
- Compiler and analyzer warnings are treated as errors.
- TDD approval gate is mandatory: tests must be proposed and explicitly approved before any production code is implemented.
- Agents are managed using [apm](https://microsoft.github.io/apm)

## MVP Backlog

See [docs/MVP-Feature-List.md](docs/MVP-Feature-List.md) for MVP feature sequencing and acceptance criteria.

## Roadmap

* JSON‑LD context for SlideFed vocabulary  
* .NET model layer and serialization contracts  
* ActivityPub server implementation  
* Client SDKs for presentation tools  
* Live session protocol  
* Deck editing and versioning workflows  

## Contributing

Contributions are welcome. SlideFed aims to become a shared standard for federated presentations, and community input is essential. Please open issues for vocabulary proposals, protocol extensions, or implementation questions.

## License

This project is licensed under the GNU Affero General Public License v3.0.
If you run a modified version of SlideFed as a network service, you must make the corresponding source code available to users of that service.

## Status

Early development. Vocabulary and protocol design in progress.
