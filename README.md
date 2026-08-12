# BreweryApi
This is a .Net Core Web API that consumes Open Brewery DB and exposes a simplified brewery listing API.

**Features**

- RESTFul API
- In-Memory Caching for 10 minutes
- Search by name, city, phone
- Sort by name, city or distance
- Source data transformation to generic DTO
- Dependency Injection
- Middleware for error handling
- Solid based architecture.
- API Versioning

  ## Endpoint
  GET /api/v1/breweries

  Added new feature for Autocomplete.
  
  GET /api/v1/suggetions
