using Microsoft.AspNetCore.Mvc;

namespace CustomerSupportCRM.API.Controllers;

/// <summary>
/// Common base for feature controllers: standard routing convention and
/// <c>[ApiController]</c> behavior (automatic 400 on model-binding failure,
/// attribute routing requirement, etc.). No CRM controllers derive from this yet.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
}
