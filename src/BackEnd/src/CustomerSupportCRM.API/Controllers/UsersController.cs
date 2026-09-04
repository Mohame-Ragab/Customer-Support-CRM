using CustomerSupportCRM.Application.Common.Models;
using CustomerSupportCRM.Application.Features.Users.Commands.CreateUser;
using CustomerSupportCRM.Application.Features.Users.Commands.UpdateUser;
using CustomerSupportCRM.Application.Features.Users.DTOs;
using CustomerSupportCRM.Application.Features.Users.Queries.GetUserById;
using CustomerSupportCRM.Application.Features.Users.Queries.ListUsers;
using CustomerSupportCRM.Application.Features.Users.Services;
using CustomerSupportCRM.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerSupportCRM.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public sealed class UsersController : BaseApiController
{
    private readonly IUserProfileService _userProfileService;
    private readonly IMediator _mediator;

    public UsersController(IUserProfileService userProfileService, IMediator mediator)
    {
        _userProfileService = userProfileService;
        _mediator = mediator;
    }

    /// <summary>
    /// GET /api/users/me
    ///
    /// Retrieves the authenticated user's profile information (email, phone, etc).
    /// Does not include sensitive data like password, security stamps, or role assignments.
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserProfileDto>> GetMe(CancellationToken ct)
        => Ok(await _userProfileService.GetCurrentAsync(ct));

    /// <summary>
    /// PUT /api/users/me
    ///
    /// Updates the authenticated user's profile fields (email, phone number).
    /// Validation errors are returned as field-level errors (HTTP 400).
    /// </summary>
    [HttpPut("me")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserProfileDto>> UpdateMe(
        [FromBody] UpdateUserProfileRequest request,
        CancellationToken ct)
        => Ok(await _userProfileService.UpdateCurrentAsync(request, ct));

    // ---- Admin staff-account management (security-admin/manage-users) --------
    // Endpoints below require the Admin role; the "/me" endpoints above remain
    // open to any authenticated user via the class-level [Authorize].

    /// <summary>
    /// POST /api/users
    ///
    /// Creates a staff account (Admin/Supervisor/Manager/Agent). Administrator-only.
    /// </summary>
    [Authorize(Roles = Roles.Admin)]
    [HttpPost]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetUser), new { id = result.Id }, result);
    }

    /// <summary>
    /// GET /api/users
    ///
    /// Lists staff accounts, paginated and optionally filtered by search text / role. Administrator-only.
    /// </summary>
    [Authorize(Roles = Roles.Admin)]
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedResult<UserDto>>> ListUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? role = null,
        CancellationToken ct = default)
        => Ok(await _mediator.Send(new ListUsersQuery(page, pageSize, search, role), ct));

    /// <summary>
    /// GET /api/users/{id}
    ///
    /// Returns a single staff account by id. Administrator-only.
    /// </summary>
    [Authorize(Roles = Roles.Admin)]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetUser(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetUserByIdQuery(id), ct));

    /// <summary>
    /// PUT /api/users/{id}
    ///
    /// Updates a staff account's full name, role, and active flag. Administrator-only.
    /// </summary>
    [Authorize(Roles = Roles.Admin)]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserDto>> UpdateUser(
        Guid id, [FromBody] UpdateUserRequest body, CancellationToken ct)
    {
        var command = new UpdateUserCommand(id, body.FullName, body.Role, body.IsActive);
        return Ok(await _mediator.Send(command, ct));
    }
}
