# Behaviors

Reserved for cross-cutting request pipeline behaviors (e.g. validation, logging,
performance) if/when a mediator pattern (such as MediatR) is introduced for
use-case dispatch. Not implemented yet — no such pipeline exists in this
architecture change; validation is currently applied at the API boundary via
`API/Filters/ValidationFilter`.
