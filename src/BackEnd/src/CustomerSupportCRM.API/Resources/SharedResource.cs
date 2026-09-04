namespace CustomerSupportCRM.API.Resources;

/// <summary>
/// Marker type with no members, used only as a generic argument for
/// <c>IStringLocalizer&lt;SharedResource&gt;</c> so it resolves
/// <c>Resources/SharedResource.resx</c> (en, default) and
/// <c>Resources/SharedResource.ar.resx</c> (ar). Shared across the API for
/// common system/error messages; feature-specific resource files can be added
/// the same way once features exist.
/// </summary>
public sealed class SharedResource
{
    private SharedResource()
    {
    }
}
