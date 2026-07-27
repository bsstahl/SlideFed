# SlideFed Internal Press Release

SlideFed is a new federated presentation platform built on ActivityPub and ActivityStreams. It transforms presentations from static, siloed files into first‑class, addressable, remixable Fediverse objects. Every slide, deck, and content item receives its own URI, making presentations decentralized, linkable, and observable across servers.

## Problem Statement

Presentation tools today are locked inside proprietary formats and closed ecosystems. They cannot be shared across platforms, followed in real time without centralized infrastructure, or annotated, reused, or versioned in a federated way. The Fediverse lacks a standard for structured presentation content, resulting in no interoperability, no portability, and no decentralized presentation model.

## Vision

SlideFed brings presentations into the Fediverse by treating them as ActivityStreams objects and using ActivityPub to publish, update, and present them. Slides become collections of ContentItems; decks become ordered Collections; sessions become observable Activities. Presentation actions—Present, Reveal, Advance, Update—are lightweight ActivityPub messages referencing already‑published objects.

This enables presentations that are:

- **Federated** — shared across servers, not locked in apps  
- **Composable** — slides and content items can be reused anywhere  
- **Observable** — audiences follow sessions in real time  
- **Extensible** — new content types and activities can be added without breaking compatibility  
- **Open** — aligned with Fediverse norms and AGPL‑friendly  

## Customer Value

SlideFed unlocks new capabilities for creators, educators, conferences, and distributed teams:

- Publish decks that anyone can follow from any ActivityPub server  
- Present live without centralized infrastructure  
- Allow audiences to annotate slides or content items  
- Version and update slides without re‑sending entire decks  
- Reuse diagrams, code blocks, and text fragments across multiple presentations  
- Enable federated collaboration on structured content  

## Technical Approach

SlideFed defines a clear ubiquitous language and JSON‑LD vocabulary:

- **Slide** — ActivityStreams Object  
- **ContentItem** — atomic content unit with its own URI  
- **Deck** — ActivityStreams Collection  
- **PresentationSession** — live session object  
- **Activities** — Create, Update, Add, Present, Reveal, Advance, Rewind, Annotate, StartSession, EndSession  

Publishing pipeline:

1. Publish ContentItems  
2. Publish Slides referencing those items  
3. Publish Decks referencing those slides  

Presentation pipeline:

- Present → Reveal → Advance → Rewind → EndSession  

This architecture ensures interoperability, decentralization, and long‑term compatibility with the broader Fediverse.

## Launch Narrative

SlideFed is the first platform to bring structured presentations into the decentralized social web. It establishes a foundation for interoperable presentation tools, federated conferences, distributed classrooms, and collaborative content creation. By aligning with ActivityPub and ActivityStreams, SlideFed becomes a natural extension of the Fediverse—treating presentations as open, linkable, remixable objects rather than proprietary files.

SlideFed is entering early development. Vocabulary, JSON‑LD context, and ActivityPub endpoints are underway, with server and client SDKs following next.
