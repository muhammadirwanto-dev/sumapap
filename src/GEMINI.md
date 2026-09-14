# Repository Agent Rules - Sumapap

## File Operation Permissions

### 1. Repository Tracked Files (Autonomous Operations)
- Any **Create, Read, Update, or Delete (CRUD)** operation on files that are part of the codebase and tracked by the Git repository (or non-secret files not ignored by `.gitignore`) is **fully authorized without asking for user permission**.
- Execute necessary code edits, project updates, and file management directly.

### 2. Production Configuration & Secret Files (Permission Required)
- **ALWAYS ask for explicit user permission** before creating, reading, updating, or deleting production configuration or secret-containing files.
- This includes, but is not limited to:
  - `appsettings.json` and environment variant settings (e.g., `appsettings.Production.json`, `appsettings.Development.json`)
  - `.env` and `.env.*` files
  - Files containing API keys, credentials, connection strings, certificates, or secrets
