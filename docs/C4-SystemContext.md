# SlideFed C4 System Context

This is the highest-level C4 view for SlideFed. It shows the people and external systems around the platform, plus the major internal parts that make publication and live presentation work.

```mermaid
architecture-beta
    group external(cloud)[External Users and Tools]
    group slidefed(cloud)[SlideFed Platform]
    group fediverse(cloud)[Fediverse Network]

    service presenter(server)[Presenter and Authoring UI] in external
    service cli(server)[Publishing CLI] in external
    service viewer(server)[Presentation Client] in external

    service api(server)[WebAPI and ActivityPub Endpoints] in slidefed
    service worker(server)[Scheduler and Delivery Worker] in slidefed
    service store(database)[Persistent Model Store] in slidefed

    service remote(server)[Remote Fediverse Servers] in fediverse

    presenter:R --> L:api
    cli:R --> L:api
    viewer:R --> L:api

    api:B --> T:store
    api:B --> T:worker
    worker:R --> L:remote
    api:R --> L:remote
```

## What This View Covers

- Presenters author and publish decks and sessions.
- The CLI prepares and publishes content into the platform.
- Presentation clients follow a session and render the current slide as updates arrive.
- The WebAPI owns publication, federation, and session state.
- The worker handles scheduled activation and outbound delivery.
- The persistent store keeps the canonical model and delivery state.
- Remote Fediverse servers receive and exchange ActivityPub traffic.

## Next Views

- Container view: break SlideFed into API, worker, store, and client-facing edges.
- Component view: split the WebAPI into publication, session, and federation components.
