# Security

This document highlights key security considerations for running Identity.Service.

## JWT Signing Key
- Current code generates a random signing key per startup (tokens invalidate on restart).
- For non-dev, configure a stable signing key:
  - Setting: `Jwt:SigningKey` (env: `Jwt__SigningKey`)
  - Use a long, random secret (e.g., base64-encoded 256-bit key)
  - Rotate keys with care; plan token lifetimes and rollover

## Credentials and Secrets
- Do not commit secrets. Use environment variables, user-secrets (dev), or a secrets manager.
- Change default passwords from the examples (DB, pgAdmin).
- Limit privileges on the DB user in non-dev environments.

## Transport and Cookies
- Enforce HTTPS in non-dev (reverse proxy or Kestrel with certs).
- Configure `CookieAuthenticationOptions` appropriately (HttpOnly, SameSite, SecurePolicy).

## CORS and Headers
- Restrict CORS origins for frontends.
- Set security headers via middleware or reverse proxy (HSTS, X-Content-Type-Options, X-Frame-Options, CSP as needed).

## Health and Metadata
- Health endpoint: consider protecting or scoping in production.
- Swagger: limit to non-prod or secure behind auth if it exposes sensitive metadata.

## Logging and PII
- Sensitive fields (passwords) are masked in request logs.
- Avoid logging tokens, secrets, or full payloads in production logs.

## Containers and Supply Chain
- Pin image versions and update regularly.
- Use a minimal base image where possible and scan images for vulnerabilities.
- Restrict exposed ports and prefer private networks for inter-service traffic.

## Database
- Use TLS to the database where applicable.
- Separate schemas or databases per environment; avoid sharing credentials across environments.

## Backups and Recovery
- Implement regular DB backups and verify restore procedures.
- Secure backup storage and encrypt at rest.

## Least Privilege
- Limit app and infrastructure permissions to the minimum required.
- Audit and rotate credentials periodically.