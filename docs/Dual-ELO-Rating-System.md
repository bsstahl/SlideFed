# SlideFed Enhancement Summary: Dual‑ELO Rating System for Speakers & Attendees

A future SlideFed enhancement can introduce a **dual‑ELO rating system** that evaluates both *speaker performance* and *attendee evaluation reliability*. This system is designed specifically for conference ecosystems, producing interpretable, manipulation‑resistant ratings that improve over time as more sessions occur.

This document provides a **complete, implementation‑ready specification** suitable for SlideFed’s domain documentation. It includes the conceptual model, formulas, data structures, and update workflow required for coding agents to implement the feature.

---

## Core Concept

SlideFed maintains **two linked rating systems**:

- **Speaker ELO** — measures how well a speaker performs relative to expected performance.  
- **Attendee ELO** — measures how reliable an attendee’s evaluations are compared to consensus.

Each session updates both systems, creating a self‑correcting feedback loop.

---

## Speaker ELO System

### Purpose

Quantify a speaker’s *expected impact* across clarity, engagement, usefulness, and delivery quality.

### Inputs

- Normalized attendee evaluations (0–1 scale)  
- Weighted by attendee A‑ELO  
- Session difficulty modifiers  
- Audience size modifiers  
- Speaker’s current S‑ELO

### Core Formula

\[
S_{\text{new}} = S_{\text{old}} + K_s \cdot (R_{\text{actual}} - R_{\text{expected}})
\]

Where:

- \(R_{\text{actual}}\): weighted average of attendee evaluations  
- \(R_{\text{expected}}\): logistic transform of rating difference  
- \(K_s\): sensitivity constant (default 12; keynote 16; lightning 8)

### Expected Score

\[
R_{\text{expected}} = \frac{1}{1 + 10^{(D/400)}}
\]

Where \(D = S_{\text{old}} - \text{SessionBaseline}\).  
SessionBaseline can be a fixed value (e.g., 1500) or dynamic based on conference tier.

### Modifiers

- **Difficulty**: keynote +10%, lightning –20%  
- **Audience size**: <20 attendees –30%; >200 attendees +20%  
- **Topic complexity**: advanced +5%

## Attendee ELO System

### Purpose

Measure how well an attendee’s evaluations align with consensus, rewarding thoughtful, calibrated feedback.

### Inputs

- Attendee’s rating  
- Consensus rating (weighted by other attendees’ A‑ELO)  
- Attendee’s current A‑ELO

### Core Formula

\[
A_{\text{new}} = A_{\text{old}} + K_a \cdot (E_{\text{consensus}} - E_{\text{user}})
\]

Where:

- \(E_{\text{consensus}}\): weighted average evaluation  
- \(E_{\text{user}}\): attendee’s evaluation  
- \(K_a\): sensitivity constant (default 6)

### Consensus Calculation

\[
E_{\text{consensus}} = \frac{\sum (E_i \cdot A_i)}{\sum A_i}
\]

This ensures high‑rated attendees influence consensus more.

---

## System Interaction Model

Speaker ELO and Attendee ELO form a **closed feedback loop**:

- Speaker ratings use **attendee ratings weighted by attendee A‑ELO**.  
- Attendee ratings update based on **how well their evaluations predict speaker performance**.

This produces:

- High‑quality speakers rising  
- High‑quality evaluators rising  
- Low‑signal evaluators losing influence  
- Resistance to manipulation or rating brigading

## Data Model (Implementation‑Ready)

### Entities

#### Speaker

- `speakerId`  
- `name`  
- `currentSElo` (default 1500)  
- `sessionHistory[]`

#### Attendee

- `attendeeId`  
- `name`  
- `currentAElo` (default 1500)  
- `evaluationHistory[]`

#### Session

- `sessionId`  
- `speakerId`  
- `attendeeEvaluations[]`  
  - `attendeeId`  
  - `rawScore` (1–5 or 1–10)  
  - `normalizedScore` (0–1)  
- `difficulty` (keynote/workshop/lightning)  
- `audienceSize`  
- `topicComplexity`

### Derived Fields

- `weightedActualScore`  
- `expectedScore`  
- `speakerEloDelta`  
- `attendeeEloDelta[]`

---

## Update Workflow

### 1. Normalize attendee scores

Convert raw scores to 0–1 scale.

### 2. Compute consensus

Weighted by attendee A‑ELO.

### 3. Compute speaker’s expected score

Using logistic transform.

### 4. Compute speaker’s actual score

Weighted average of normalized evaluations.

### 5. Apply modifiers

Difficulty, audience size, complexity.

### 6. Update speaker ELO

Apply formula and store delta.

### 7. Update attendee ELO

For each attendee, compute deviation from consensus and apply formula.

### 8. Persist results

Store deltas, updated ratings, and session history.

---

## Optional Future Component: Trail‑ELO

If SlideFed supports associative trails:

- Each trail receives a rating based on:
  - Average speaker ELO  
  - Attendee engagement  
  - Session outcomes along the trail

This allows SlideFed to surface the **highest‑value knowledge paths** automatically.
