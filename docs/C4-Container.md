# SlideFed C4 Container View

This view breaks the SlideFed platform into its main deployable and runnable containers.

```mermaid
architecture-beta
    group tools(cloud)[Tooling]
    group platform(cloud)[SlideFed Platform]
    group fediverse(cloud)[Fediverse Network]

    service cli(server)[Publishing CLI] in tools
    service presenter(server)[Presenter and Authoring UI] in tools
    service client(server)[Presentation Client] in tools

    service api(server)[WebAPI and ActivityPub Endpoints] in platform
    service worker(server)[Scheduler and Delivery Worker] in platform
    service store(database)[Persistent Model Store] in platform
    service projection(database)[Read Model and Snapshot Projection] in platform

    service remote(server)[Remote Fediverse Servers] in fediverse

    cli:R --> L:api
    presenter:R --> L:api
    client:R --> L:api

    api:B --> T:store
    api:B --> T:projection
    api:B --> T:worker
    worker:R --> L:remote
    api:R --> L:remote
```

## Containers

- **Publishing CLI** prepares and publishes decks, slides, and sessions.
- **Presenter and Authoring UI** lets a human author and manage presentation content.
- **Presentation Client** follows a session and renders the active slide as updates arrive.
- **WebAPI and ActivityPub Endpoints** own HTTP publication, federation, and session state.
- **Scheduler and Delivery Worker** handles start-time activation, outbound fanout, and retries.
- **Persistent Model Store** keeps the canonical domain data.
- **Read Model and Snapshot Projection** supports fast session bootstrap and late-join rendering.
- **Remote Fediverse Servers** exchange ActivityPub traffic with SlideFed.

## Main Flows

- The CLI and authoring UI submit publication requests to the WebAPI.
- The WebAPI persists the canonical model and emits work for delivery and projection.
- The worker handles federation delivery and scheduled transitions.
- The presentation client subscribes to a session, receives updates, and renders the current slide.
- The projection store serves late-join snapshots and read-heavy session views.

## Next View

- Component view: split the WebAPI into publication, session lifecycle, follow/bootstrap, and federation components.
