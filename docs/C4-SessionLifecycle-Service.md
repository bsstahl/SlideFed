# SlideFed Session Lifecycle Service Detail

This view expands the Session Lifecycle Service and shows the main internal handlers, policies, and infrastructure boundaries that govern manual session state changes.

```mermaid
architecture-beta
    group callers(cloud)[Callers]
    group session_boundary(cloud)[Session Lifecycle Service]
    group backing(cloud)[Backing Services]

    service sess_edge(server)[HTTP and ActivityPub Edge] in callers
    service sess_presenter(server)[Presenter and Authoring UI] in callers

    service sess_router(server)[Lifecycle Command Router] in session_boundary
    service sess_activate(server)[Activate Session Handler] in session_boundary
    service sess_pause(server)[Pause Session Handler] in session_boundary
    service sess_unpause(server)[Unpause Session Handler] in session_boundary
    service sess_cancel(server)[Cancel Session Handler] in session_boundary
    service sess_end(server)[End Session Handler] in session_boundary
    service sess_rules(server)[State Transition Policy] in session_boundary
    service sess_clock(server)[Manual Trigger Policy] in session_boundary
    service sess_events(server)[Session Event Builder] in session_boundary

    service sess_repo(server)[Domain Repository] in backing
    service sess_projection(server)[Projection Trigger] in backing
    service sess_worker(server)[Scheduler and Delivery Worker] in backing

    sess_presenter:R --> L:sess_edge
    sess_edge:B --> T:sess_router

    sess_router:R --> L:sess_activate
    sess_router:R --> L:sess_pause
    sess_router:R --> L:sess_unpause
    sess_router:R --> L:sess_cancel
    sess_router:R --> L:sess_end

    sess_activate:B --> T:sess_rules
    sess_pause:B --> T:sess_rules
    sess_unpause:B --> T:sess_rules
    sess_cancel:B --> T:sess_rules
    sess_end:B --> T:sess_rules

    sess_activate:R --> L:sess_clock
    sess_pause:R --> L:sess_clock
    sess_unpause:R --> L:sess_clock
    sess_cancel:R --> L:sess_clock
    sess_end:R --> L:sess_clock

    sess_activate:B --> T:sess_events
    sess_pause:B --> T:sess_events
    sess_unpause:B --> T:sess_events
    sess_cancel:B --> T:sess_events
    sess_end:B --> T:sess_events

    sess_events:R --> L:sess_repo
    sess_events:B --> T:sess_projection
    sess_events:R --> L:sess_worker
```

## Components

- **Lifecycle Command Router** maps incoming lifecycle intents to the correct handler.
- **Activate Session Handler** transitions a `draft` or `canceled` session into `live` when allowed.
- **Pause Session Handler** moves a `live` session into `paused`.
- **Unpause Session Handler** restores a paused session to `live`, potentially modeled as `Undo` of the prior pause activity.
- **Cancel Session Handler** applies the non-terminal MVP `canceled` state.
- **End Session Handler** closes the session with the terminal `ended` state.
- **State Transition Policy** enforces allowed source and target states.
- **Manual Trigger Policy** reflects the MVP rule that transitions are manually triggered rather than time-driven automatically.
- **Session Event Builder** records the canonical session update, emits projection work, and signals downstream delivery.

## Main Flows

- A presenter action enters through the edge and is routed to a lifecycle handler.
- The chosen handler checks whether the requested transition is valid under the state policy.
- The manual trigger policy prevents hidden automatic transitions from bypassing presenter intent in MVP.
- The event builder persists the state change, triggers projection refresh, and signals the worker for delivery.
