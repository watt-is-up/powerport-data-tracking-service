# Data Tracking Service

The **Data Tracking Service** is responsible for storing, validating, and analyzing charging-related data and user-defined metrics. It enables users to track their EV usage over time and derive insights from charging sessions.

---

## Responsibilities

### MVP
- Allow users to input their own tracking data with basic validation:
  - Numeric values
  - Strings
  - Dates
- Consume charging session events from the **Station Management Service**
- Persist usage statistics related to charging sessions
- Generate summaries of a user’s charging history

### Planned / Advanced
- Increase limits on the amount of user-defined data that can be tracked (e.g. from 3 to 10 fields)
- Allow users to generate plots from tracked data:
  - Line charts
  - Scatter plots
  - Other basic visualizations
- Prompt users after a charging session:
  - “Do you want to add this session to data tracking?”

---

## Architecture Context

- Receives events from **Station Management Service**
- Stores user-defined and system-generated tracking data
- Exposes aggregated and summarized data to other services and the frontend
- Runs as an independent, containerized .NET microservice

---

## Technology Stack

- **.NET 8**
- **ASP.NET Core**
- **Docker**
- **Kubernetes** (deployment managed externally)

---

## Local Development

### Prerequisites
- .NET SDK (version pinned in `global.json`)
- Docker (optional)

### Run locally
```bash
dotnet restore
dotnet run --project src/DataTrackingService
```

---

## Build & Test

```bash
dotnet build
dotnet test
```

---

## Container Image

This service is built and published as a Docker image via CI/CD.  
Image building and deployment are handled outside this repository.

---

## Notes

- This service focuses on analytics and historical data, not real-time charging control.
- Data validation is intentionally lightweight in the MVP and can be expanded later.
- Deployment configuration lives in the infrastructure repository.
