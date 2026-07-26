# SlideFed Projection and Snapshot Pipeline Detail

This view expands the read-model path that powers fast client rendering and late-join bootstrap behavior.

```mermaid
architecture-beta
    group callers(cloud)[Callers]
    group projection_boundary(cloud)[Projection and Snapshot Pipeline]
    group backing(cloud)[Backing Stores and Services]

    service proj_publication(server)[Publication Application Service] in callers
    service proj_session(server)[Session Lifecycle Service] in callers
    service proj_follow(server)[Follow and Audience Service] in callers
    service proj_worker(server)[Scheduler and Delivery Worker] in callers

    service proj_ingest(server)[Projection Event Ingestor] in projection_boundary
    service proj_transform(server)[Read Model Transformer] in projection_boundary
    service proj_snapshot(server)[Snapshot Builder] in projection_boundary
    service proj_latejoin(server)[Late Join Bootstrap Resolver] in projection_boundary
    service proj_consistency(server)[Projection Consistency Policy] in projection_boundary
    service proj_publish(server)[Read Model Publisher] in projection_boundary

    service proj_readstore(database)[Read Model and Snapshot Projection] in backing
    service proj_store(database)[Persistent Model Store] in backing
    service proj_client(server)[Presentation Client] in backing
    service proj_api(server)[WebAPI and ActivityPub Endpoints] in backing

    proj_publication:R --> L:proj_ingest
    proj_session:R --> L:proj_ingest
    proj_follow:R --> L:proj_ingest
    proj_worker:R --> L:proj_ingest

    proj_ingest:B --> T:proj_transform
    proj_transform:B --> T:proj_consistency
    proj_transform:B --> T:proj_snapshot
    proj_snapshot:B --> T:proj_latejoin
    proj_latejoin:B --> T:proj_publish

    proj_transform:R --> L:proj_store
    proj_publish:B --> T:proj_readstore
    proj_publish:R --> L:proj_api
    proj_client:R --> L:proj_api
```

## Components

- **Projection Event Ingestor** accepts domain events that should affect read models or snapshots.
- **Read Model Transformer** updates query-optimized session and deck views.
- **Snapshot Builder** produces current-state snapshots for bootstrap and render continuity.
- **Late Join Bootstrap Resolver** applies MVP bootstrap behavior using snapshot-only responses.
- **Projection Consistency Policy** keeps projections coherent with canonical writes despite retries or reordering.
- **Read Model Publisher** exposes projection updates to API-facing read endpoints.

## Main Flows

- Write-side events from publication, lifecycle, follow, and worker processes feed the projection pipeline.
- The transformer and snapshot builder update read models and current session views.
- Late-join bootstrap is served from snapshot artifacts in line with MVP rules.
- The API exposes projection-backed reads that presentation clients consume.
