# SlideFed Follow and Audience Service Detail

This view expands the Follow and Audience Service and shows how SlideFed accepts follows, applies join rules, and prepares late-join snapshots for presentation clients.

```mermaid
architecture-beta
    group callers(cloud)[Callers]
    group follow_boundary(cloud)[Follow and Audience Service]
    group backing(cloud)[Backing Services]

    service follow_edge(server)[HTTP and ActivityPub Edge] in callers
    service follow_client(server)[Presentation Client] in callers
    service follow_remote(server)[Remote Fediverse Servers] in callers

    service follow_router(server)[Follow Command Router] in follow_boundary
    service follow_accept(server)[Accept Follow Handler] in follow_boundary
    service follow_undo(server)[Undo Follow Handler] in follow_boundary
    service follow_join(server)[Join Policy] in follow_boundary
    service follow_bootstrap(server)[Late Join Bootstrap Policy] in follow_boundary
    service follow_audience(server)[Audience Membership Policy] in follow_boundary
    service follow_response(server)[Follow Response Builder] in follow_boundary

    service follow_repo(server)[Domain Repository] in backing
    service follow_projection(database)[Read Model and Snapshot Projection] in backing
    service follow_delivery(server)[Scheduler and Delivery Worker] in backing

    follow_client:R --> L:follow_edge
    follow_remote:R --> L:follow_edge
    follow_edge:B --> T:follow_router

    follow_router:R --> L:follow_accept
    follow_router:R --> L:follow_undo

    follow_accept:B --> T:follow_join
    follow_accept:B --> T:follow_audience
    follow_accept:B --> T:follow_bootstrap
    follow_undo:B --> T:follow_audience

    follow_accept:R --> L:follow_response
    follow_undo:R --> L:follow_response

    follow_response:B --> T:follow_repo
    follow_response:B --> T:follow_projection
    follow_response:R --> L:follow_delivery
```

## Components

- **Follow Command Router** directs inbound `Follow` and `Undo(Follow)` activities to the appropriate handler.
- **Accept Follow Handler** validates and accepts new follows for presenters or sessions.
- **Undo Follow Handler** removes an existing audience relationship.
- **Join Policy** enforces whether following is allowed before start, mid-stream, or after end.
- **Late Join Bootstrap Policy** chooses the MVP snapshot-only bootstrap behavior for new followers.
- **Audience Membership Policy** maintains the session audience collection and associated invariants.
- **Follow Response Builder** persists the relationship change and prepares acceptance, bootstrap, and delivery side effects.

## Main Flows

- A presentation client or remote server submits a `Follow` request through the edge.
- The accept-follow handler evaluates whether the session state permits joining under the join policy.
- If accepted, the service updates audience membership and prepares a snapshot-oriented bootstrap response.
- `Undo(Follow)` removes the relationship and stops future session updates from being delivered.
