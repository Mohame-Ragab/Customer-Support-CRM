# Services

External-service integrations (e.g. email, SMS, file storage) implemented against
Application-defined interfaces go here as they're introduced. `ICurrentUserService`
is intentionally implemented in `API/` instead (it needs `IHttpContextAccessor`,
available without an extra package only in a Web SDK project — see
docs/architecture.md, "Current user abstraction"). Empty for now.
