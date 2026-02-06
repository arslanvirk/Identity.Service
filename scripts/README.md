# Scripts Directory

This directory contains utility scripts for managing the Identity Service deployment.

## Available Scripts

### 1. `setup-secrets.ps1` (Windows PowerShell)
Creates Docker secrets files for production deployment on Windows.

**Usage:**
```powershell
.\scripts\setup-secrets.ps1
```

**What it does:**
- Creates `./secrets/` directory with restrictive permissions
- Prompts for all required secrets interactively
- Generates secure JWT signing key (optional)
- Creates individual `.txt` files for each secret
- Validates input and provides helpful defaults

**Requirements:**
- PowerShell 5.1 or later
- Docker Desktop for Windows

---

### 2. `setup-secrets.sh` (Linux/Mac Bash)
Creates Docker secrets files for production deployment on Linux/Mac.

**Usage:**
```bash
chmod +x scripts/setup-secrets.sh
./scripts/setup-secrets.sh
```

**What it does:**
- Creates `./secrets/` directory with `700` permissions
- Prompts for all required secrets interactively
- Generates secure JWT signing key using OpenSSL
- Creates individual `.txt` files with `600` permissions
- Validates input and provides helpful defaults

**Requirements:**
- Bash shell
- OpenSSL (for key generation)
- Docker Engine or Docker Desktop

---

## Output

Both scripts create the following structure:

```
secrets/
??? jwt_issuer.txt              # JWT token issuer URL
??? jwt_audience.txt            # JWT token audience
??? jwt_signing_key.txt         # JWT signing key (base64, 32+ bytes)
??? connection_string.txt       # PostgreSQL connection string
??? postgres_password.txt       # PostgreSQL password
??? pgadmin_email.txt          # pgAdmin login email
??? pgadmin_password.txt       # pgAdmin login password
```

**Permissions:**
- Directory: `700` (owner read/write/execute only)
- Files: `600` (owner read/write only)

---

## Security Notes

?? **IMPORTANT:**
1. The `secrets/` directory is gitignored
2. Never commit secrets to version control
3. Backup secrets securely (encrypted storage)
4. Use strong passwords (16+ characters)
5. Rotate secrets every 60-90 days

---

## Usage in Docker Compose

After running the setup script, deploy with:

```bash
docker compose -f docker-compose.prod.secrets.yml up -d
```

The secrets will be mounted as read-only files in:
```
/run/secrets/jwt_issuer
/run/secrets/jwt_audience
/run/secrets/jwt_signing_key
/run/secrets/connection_string
/run/secrets/postgres_password
/run/secrets/pgadmin_email
/run/secrets/pgadmin_password
```

---

## Future Scripts (Planned)

- `rotate-secrets.sh` - Automated secret rotation
- `backup-secrets.sh` - Encrypted backup of secrets
- `validate-secrets.sh` - Validate secret format and strength
- `deploy-azure.sh` - Deploy to Azure Container Instances

---

## Troubleshooting

### Script won't run (Linux/Mac)
```bash
# Make executable
chmod +x scripts/setup-secrets.sh

# Run with bash explicitly
bash scripts/setup-secrets.sh
```

### Permission denied (Windows)
```powershell
# Set execution policy
Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy RemoteSigned

# Or run with bypass
powershell -ExecutionPolicy Bypass -File .\scripts\setup-secrets.ps1
```

### Secrets directory already exists
The script will prompt you to remove it. Backup first:
```bash
mv secrets secrets.backup
```

---

## Contributing

When adding new scripts:
1. Follow existing naming convention
2. Add comprehensive error handling
3. Include help/usage information
4. Update this README
5. Test on target platforms

---

For more information, see [SECRETS-MANAGEMENT.md](../docs/SECRETS-MANAGEMENT.md)
