# SlideFed Federation Gateway Detail

This view expands the Federation Gateway and shows how internal SlideFed events are translated into outbound ActivityPub delivery and how inbound federation traffic is normalized.

```mermaid
architecture-beta
    group callers(cloud)[Callers]
    group federation_boundary(cloud)[Federation Gateway]
    group backing(cloud)[Backing Services]
    group remote_net(cloud)[Fediverse Network]

    service fed_edge(server)[HTTP and ActivityPub Edge] in callers
    service fed_publication(server)[Publication Application Service] in callers
    service fed_session(server)[Session Lifecycle Service] in callers
    service fed_follow(server)[Follow and Audience Service] in callers

    service fed_ingress(server)[Inbound Activity Handler] in federation_boundary
    service fed_egress(server)[Outbound Activity Builder] in federation_boundary
    service fed_map(server)[Activity Mapping Policy] in federation_boundary
    service fed_auth(server)[Inbox Verification Policy] in federation_boundary
    service fed_targets(server)[Audience and Inbox Resolver] in federation_boundary
    service fed_queue(server)[Delivery Command Builder] in federation_boundary
    service fed_errors(server)[Delivery Failure Policy] in federation_boundary

    service fed_repo(server)[Domain Repository] in backing
    service fed_worker(server)[Scheduler and Delivery Worker] in backing
    service fed_remote(server)[Remote Fediverse Servers] in remote_net

    fed_edge:B --> T:fed_ingress
    fed_publication:R --> L:fed_egress
    fed_session:R --> L:fed_egress
    fed_follow:R --> L:fed_egress

    fed_ingress:B --> T:fed_auth
    fed_ingress:B --> T:fed_map
    fed_ingress:R --> L:fed_repo

    fed_egress:B --> T:fed_map
    fed_egress:B --> T:fed_targets
    fed_egress:B --> T:fed_queue

    fed_queue:R --> L:fed_errors
    fed_queue:R --> L:fed_worker
    fed_worker:R --> L:fed_remote
```

## Components

- **Inbound Activity Handler** receives remote inbox traffic and normalizes it into internal commands or events.
- **Outbound Activity Builder** translates internal SlideFed events into standard ActivityPub messages.
- **Activity Mapping Policy** keeps SlideFed’s internal vocabulary aligned with AS2 and ActivityPub message shapes.
- **Inbox Verification Policy** validates remote sender identity, signature, and protocol-level trust checks.
- **Audience and Inbox Resolver** determines which remote inboxes should receive each outbound activity.
- **Delivery Command Builder** creates the work items that the worker will execute for fanout.
- **Delivery Failure Policy** governs retry, backoff, and failure recording behavior.

## Main Flows

- Internal publication, session, and follow events are translated into outbound ActivityPub messages.
- The gateway resolves target inboxes and hands delivery commands to the worker.
- Remote inbox traffic is authenticated, mapped into internal semantics, and persisted or forwarded as needed.
- Delivery failures are handled through explicit retry policy rather than being hidden inside controller logic.
