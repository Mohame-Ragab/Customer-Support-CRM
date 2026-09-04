namespace CustomerSupportCRM.Application.Features.Customers.Notes.Dtos;

/// <summary>POST /api/customers/{customerId}/notes request body. The customer id comes from the route, not this body.</summary>
public sealed record CreateCustomerNoteRequest(string Content);
