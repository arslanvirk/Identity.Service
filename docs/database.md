# Database & Migrations

The service uses EF Core 9 with Npgsql to target PostgreSQL. All objects are created in the `identity_base` schema via `EFDataContext`.

## Auto-Migrations on Startup
`Program.cs` applies pending migrations when the API starts:

## Manual Migrations (CLI)
From repository root:

Ensure the correct connection string is available to the startup project (environment variables or appsettings).

## Schema and Entities
- Schema: `identity_base`
- Identity tables: roles, users, claims, logins, user-roles, user-tokens
- Domain tables:
  - `users` (mapped to `User` entity)
  - `invite_user` (password reset/invite flow)
  - `configuration` (key-value configuration)

Seed data includes common roles and configuration entries as defined in migrations.

## Notes
- For production, prefer controlled migration execution (e.g., pre-deploy step) rather than auto-migration on app start.
- Keep migrations in source control; avoid editing generated code manually.