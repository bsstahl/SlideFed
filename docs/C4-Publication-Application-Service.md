# SlideFed Publication Application Service Detail

This view expands the Publication Application Service and shows how deck, slide, content item, and session publication is validated, persisted, and fanned out.

```mermaid
architecture-beta
    group callers(cloud)[Callers]
    group publication_boundary(cloud)[Publication Application Service]
    group backing(cloud)[Backing Services]

    service pub_edge(server)[HTTP and ActivityPub Edge] in callers
    service pub_cli(server)[Publishing CLI] in callers
    service pub_author(server)[Presenter and Authoring UI] in callers

    service pub_router(server)[Publication Command Router] in publication_boundary
    service pub_content(server)[Publish ContentItem Handler] in publication_boundary
    service pub_slide(server)[Publish Slide Handler] in publication_boundary
    service pub_deck(server)[Publish Deck Handler] in publication_boundary
    service pub_session(server)[Publish Session Handler] in publication_boundary
    service pub_refs(server)[Reference Integrity Policy] in publication_boundary
    service pub_version(server)[Versioning Policy] in publication_boundary
    service pub_events(server)[Publication Event Builder] in publication_boundary

    service pub_repo(server)[Domain Repository] in backing
    service pub_projection(server)[Projection Trigger] in backing
    service pub_federation(server)[Federation Gateway] in backing
    service pub_store(database)[Persistent Model Store] in backing

    pub_cli:R --> L:pub_edge
    pub_author:R --> L:pub_edge
    pub_edge:B --> T:pub_router

    pub_router:R --> L:pub_content
    pub_router:R --> L:pub_slide
    pub_router:R --> L:pub_deck
    pub_router:R --> L:pub_session

    pub_content:B --> T:pub_refs
    pub_slide:B --> T:pub_refs
    pub_deck:B --> T:pub_refs
    pub_session:B --> T:pub_refs

    pub_content:B --> T:pub_version
    pub_slide:B --> T:pub_version
    pub_deck:B --> T:pub_version
    pub_session:B --> T:pub_version

    pub_content:B --> T:pub_events
    pub_slide:B --> T:pub_events
    pub_deck:B --> T:pub_events
    pub_session:B --> T:pub_events

    pub_events:R --> L:pub_repo
    pub_repo:B --> T:pub_store
    pub_events:B --> T:pub_projection
    pub_events:B --> T:pub_federation
```

## Components

- **Publication Command Router** directs create and update publication commands to the correct object-specific handler.
- **Publish ContentItem Handler** publishes first-class content objects that slides reference by URI.
- **Publish Slide Handler** publishes slides that reference only ContentItem URIs.
- **Publish Deck Handler** publishes ordered deck objects that reference only Slide URIs.
- **Publish Session Handler** publishes session objects and initial metadata such as optional `startTime`.
- **Reference Integrity Policy** enforces URI-only references and checks that referenced resources exist.
- **Versioning Policy** applies write-side version semantics to published objects.
- **Publication Event Builder** constructs canonical update events and emits side effects for projection and federation.

## Main Flows

- The CLI or authoring UI sends a publication command through the edge.
- The selected handler validates references and applies versioning policy.
- Canonical changes are persisted via the repository into the persistent store.
- Publication events trigger both read-model projection updates and outbound federation work.
