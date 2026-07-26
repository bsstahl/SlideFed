# SlideFed WebAPI Component View

This view zooms into the WebAPI container and shows the main internal components that handle publication, session lifecycle, audience follow behavior, and federation.

```mermaid
architecture-beta
    group clients(cloud)[Clients and Tools]
    group api_boundary(cloud)[WebAPI Container]
    group platform(cloud)[Platform Containers]
    group fediverse(cloud)[Fediverse Network]

    service web_cli(server)[Publishing CLI] in clients
    service web_presenter(server)[Presenter and Authoring UI] in clients
    service web_client(server)[Presentation Client] in clients

    service edge_http(server)[HTTP and ActivityPub Edge] in api_boundary
    service comp_publication(server)[Publication Application Service] in api_boundary
    service comp_session(server)[Session Lifecycle Service] in api_boundary
    service comp_follow(server)[Follow and Audience Service] in api_boundary
    service comp_federation(server)[Federation Gateway] in api_boundary
    service comp_projection(server)[Projection Trigger] in api_boundary
    service comp_repo(server)[Domain Repository] in api_boundary

    service ext_store(database)[Persistent Model Store] in platform
    service ext_projection(database)[Read Model and Snapshot Projection] in platform
    service ext_worker(server)[Scheduler and Delivery Worker] in platform

    service ext_remote(server)[Remote Fediverse Servers] in fediverse

    web_cli:R --> L:edge_http
    web_presenter:R --> L:edge_http
    web_client:R --> L:edge_http

    edge_http:B --> T:comp_publication
    edge_http:B --> T:comp_session
    edge_http:B --> T:comp_follow
    edge_http:B --> T:comp_federation

    comp_publication:R --> L:comp_repo
    comp_session:R --> L:comp_repo
    comp_follow:R --> L:comp_repo
    comp_federation:R --> L:comp_repo

    comp_publication:B --> T:comp_projection
    comp_session:B --> T:comp_projection
    comp_follow:B --> T:comp_projection
    comp_session:R --> L:ext_worker
    comp_federation:R --> L:ext_worker

    comp_repo:B --> T:ext_store
    comp_projection:B --> T:ext_projection
    comp_federation:R --> L:ext_remote
```

## Components

- **HTTP and ActivityPub Edge** terminates HTTP requests, validates ActivityPub envelopes, and routes commands to the right internal service.
- **Publication Application Service** handles deck, slide, and session publication workflows initiated by the CLI or authoring UI.
- **Session Lifecycle Service** applies manual state transitions such as draft, live, paused, canceled, and ended.
- **Follow and Audience Service** handles follow requests, late-join bootstrap decisions, and audience membership rules.
- **Federation Gateway** translates internal actions into outbound ActivityPub delivery and accepts inbound federation messages.
- **Projection Trigger** emits the read-model and snapshot updates needed for fast client rendering and late joins.
- **Domain Repository** loads and persists the canonical domain model against the persistent store.

## Main Flows

- Publication requests enter through the edge and are handled by the publication service, which persists canonical changes through the repository.
- Session control requests enter through the edge and are applied by the session lifecycle service, which also signals scheduling and delivery work.
- Follow requests enter through the edge and are evaluated by the follow service, which decides whether to accept the follow and what snapshot bootstrap to expose.
- Outbound federation is coordinated by the federation gateway, which works with the worker and remote Fediverse servers.
- Read-model refresh and snapshot generation are triggered after publication, state changes, and follow-related events.

## Next View

- Component detail: expand one service, likely the Session Lifecycle Service or Follow and Audience Service, into handlers, policies, and persistence boundaries.
