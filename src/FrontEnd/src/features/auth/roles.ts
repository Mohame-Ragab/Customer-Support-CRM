/**
 * Role name constants, mirroring the backend's single source of truth at
 * src/BackEnd/src/CustomerSupportCRM.Domain/Constants/Roles.cs. Kept here so
 * route guards and nav-item visibility checks never hardcode role strings.
 */
export const Roles = {
  Admin: 'Admin',
  Supervisor: 'Supervisor',
  Manager: 'Manager',
  Agent: 'Agent',
  Customer: 'Customer',
} as const
