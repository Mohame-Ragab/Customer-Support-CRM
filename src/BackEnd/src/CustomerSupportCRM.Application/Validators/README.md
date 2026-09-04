# Validators

Shared/cross-feature FluentValidation validators go here. Feature-specific
validators live alongside their feature under `Features/<Feature>/`.

Validators are auto-registered by `DependencyInjection.AddApplication()` via
assembly scanning — no manual DI wiring is needed per validator.

Validation messages must not hard-code English/Arabic text; resolve them through
`IStringLocalizer<SharedResource>` (see `API/Resources/`) so messages localize
per the caller's `Accept-Language` header. Empty for now.

## Localization requirement (platform/multilingual-support-arabic-and-english)

Every new validator's `.WithMessage(...)` calls must:

- Inject `IStringLocalizer<SharedResource>` (or a feature-specific marker class
  in `API/Resources/`, following the same empty-marker-class pattern) rather
  than emitting English or Arabic prose inline.
- Reference the message via a `nameof`-style constant (mirror
  `SharedResourceKeys` in `API/Middlewares/GlobalExceptionHandler.cs`), never
  a hand-typed string literal, so a resource-key rename is a compile error.
- Add the corresponding key to **both** `SharedResource.resx` (en) and
  `SharedResource.ar.resx` (ar) in the same commit that introduces the
  validator.
