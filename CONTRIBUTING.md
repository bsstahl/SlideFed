---
title: Contributing to SlideFed
description: Contribution expectations for the SlideFed SpeakerOps repository, including product scope, workflow, and logging conventions.
---

## Contributing to SlideFed

This repository is SlideFed, a fully functional product in the SpeakerOps family for federated presentation content and publication workflows. Contributions should keep the product focused on slides, decks, presentation sessions, content-item interoperability, and the relationships that help publish and present a deck without owning the wider speaker lifecycle.

## Purpose and scope

SlideFed owns:

* slide and deck identity and structure
* presentation-session lifecycle and reveal flow
* content-item modeling and schema for remixable presentation content
* ActivityPub and ActivityStreams integration semantics for publication and follow patterns
* local presentation-state concerns for delivery and replay

SlideFed does not own:

* talk catalog or CFP workflow management
* calendar or speaker scheduling business logic
* submission and acceptance decisions for conferences
* the full SpeakerOps family governance model outside the product boundary

When in doubt, keep the work aligned to the SlideFed product domain and the repository's existing design docs, while preserving the product's independence within the broader SpeakerOps family.

## Required repository guidance

Before making changes, read the repository guidance that applies to the work:

* [README.md](./README.md)
* [CONTEXT.md](./CONTEXT.md)
* [docs/UL.md](./docs/UL.md)
* [docs/SpeakerOps-Integration.md](./docs/SpeakerOps-Integration.md)
* [docs/C4-Index.md](./docs/C4-Index.md)
* [docs/PublicationProcess.md](./docs/PublicationProcess.md)
* [docs/Dual-ELO-Rating-System.md](./docs/Dual-ELO-Rating-System.md) (future enhancement)
* [.github/instructions/logging.instructions.md](./.github/instructions/logging.instructions.md)

## Tooling and environment

This repository is expected to target the latest .NET line with C# conventions consistent with the repo's existing .NET setup.

Use the standard .NET CLI from the repository root:

```bash
dotnet build
dotnet test
```

Keep the project runnable from the repo root with no hidden setup steps. When implementation work begins, follow the repo's local standards and keep all product docs aligned with the current domain model.

## Development workflow

### 1. Start from the design baseline

Use the canonical domain docs as the first source of truth before editing code or schema. Keep slides, decks, sessions, and publication contracts aligned to the repo docs.

### 2. Keep the task narrow and explicit

Prefer small, well-scoped changes. Avoid broad "cleanup" or "improve architecture" tasks without a concrete requirement or failing case.

### 3. Use TDD when behavior changes

For any code change that modifies behavior:

1. Write or update the failing test first
2. Confirm the test fails for the right reason
3. Implement only the minimum fix
4. Refactor carefully while keeping the relevant tests green
5. Re-run the relevant validation before continuing

### 4. Keep docs and implementation in sync

This repo is deliberately documentation-heavy. When a change affects a domain decision, schema, or boundary, update the relevant docs alongside the work.

### 5. Follow the logging convention

When adding logging:

* log activity and boundary transitions at informational levels
* keep payload bodies, record snapshots, and other verbose diagnostics at trace level
* avoid promoting payload detail to information unless the log message itself describes a meaningful product event rather than the data contents

## Branching and review expectations

* Use a short-lived feature branch for work
* Keep branch scope focused on one change or one cohesive task
* Prefer small commits over large batch edits
* Do not push to remote until the work is ready to share
* Use pull requests for merge review and discussion
* Accept pull requests from individual contributors
* Fully automated PRs are not accepted (those submitted by an agent instead of an individual)
* AI assistance is allowed when preparing a pull request, but the human contributor remains responsible for the content, accuracy, and intent of the submission

When the repo reaches its implementation stage, human review should remain required for changes that affect architecture, schema, or business rules.

## Documentation and coding conventions

Follow the repo's design and implementation guidance and keep documentation consistent with the product domain language. Avoid generic framework language or unrelated template wording.

## Boundaries and “do not touch” guidance

The repo has specific domain boundaries. Keep work inside these boundaries unless the user explicitly asks for a broader change.

When a source or file is external to this repo, treat it as authoritative only at its own boundary and keep SlideFed responsible for the lightweight relationship model, not for owning that external lifecycle.

## Pull requests and validation

Before opening or completing a pull request:

* ensure the change matches the repo's current design intent
* verify related docs are updated when required
* run the smallest relevant validation locally
* confirm the change is scoped and understandable
* include any assumptions or follow-up work explicitly in the PR description

## Questions and escalation

If a change is ambiguous, crosses an external system boundary, or could affect the SlideFed domain boundary, ask for clarification before making a broad implementation change.

The default stance is: stay narrow, stay domain-aligned, and keep the repo focused on the SpeakerOps federated presentation concept.
