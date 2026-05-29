# GitHub Actions Multi-Platform MAUI Builds

> **Note**: For a comprehensive explanation of Sumapap's CI/CD architecture including cost analysis and trade-offs, see [CI_CD_ARCHITECTURE.md](./CI_CD_ARCHITECTURE.md).

## Current Configuration

Sumapap uses a **two-stage pipeline approach**:

1. **CI (Pull Requests)**: Runs on **Linux** (`ubuntu-latest`) 
   - Builds all non-MAUI projects
   - Skips `Sumapap.Reporting.Maui` (requires macOS)
   - Fast feedback (~3-5 minutes)

2. **CD (Publishing)**: Runs on **macOS** (`macos-latest`)
   - Builds ALL projects including MAUI with all platforms
   - Creates complete multi-platform NuGet packages
   - Publishes to NuGet.org

This approach balances speed, cost, and completeness. See [CI_CD_ARCHITECTURE.md](./CI_CD_ARCHITECTURE.md) for details.

## Platform Workload Requirements

| Workload | Required Runner | Availability |
|----------|----------------|--------------|
| `maui-android` | Linux, macOS, Windows | ✅ All runners |
| `maui-ios` | macOS only | ❌ Not on Linux/Windows |
| `maui-maccatalyst` | macOS only | ❌ Not on Linux/Windows |
| `maui-windows` | Windows only | ❌ Not on Linux/macOS |

## Option 1: Current Setup (Android Only)

**Best for:** Most scenarios, especially if you're primarily targeting Android or building NuGet packages that don't require platform-specific compilation.

```yaml
jobs:
  build:
	runs-on: ubuntu-latest
	steps:
	  - name: Install .NET MAUI Workloads
		run: dotnet workload install maui-android
```

**Pros:**
- Fast execution (Linux runners are fastest)
- Simplest configuration
- Works for NuGet package publishing
- No additional costs (macOS runners cost 10x more minutes)

**Cons:**
- Cannot build iOS or macOS Catalyst apps
- Cannot run iOS/macOS-specific tests

## Option 2: Matrix Strategy (All Platforms)

**Best for:** Full cross-platform validation, platform-specific app builds, or comprehensive testing.

### Example Multi-Platform Workflow

```yaml
name: Multi-Platform Build

on: [push, pull_request]

jobs:
  build:
	strategy:
	  matrix:
		include:
		  - os: ubuntu-latest
			platform: android
			workload: maui-android
		  - os: macos-latest
			platform: ios
			workload: maui-ios
		  - os: macos-latest
			platform: maccatalyst
			workload: maui-maccatalyst
		  - os: windows-latest
			platform: windows
			workload: maui-windows

	runs-on: ${{ matrix.os }}

	steps:
	  - name: Checkout Code
		uses: actions/checkout@v4

	  - name: Setup .NET
		uses: actions/setup-dotnet@v4
		with:
		  dotnet-version: '10.0.x'

	  - name: Install MAUI Workload
		run: dotnet workload install ${{ matrix.workload }}

	  - name: Restore Dependencies
		run: dotnet restore

	  - name: Build for ${{ matrix.platform }}
		run: dotnet build -c Release
```

**Pros:**
- Validates all platforms
- Can run platform-specific tests
- Builds platform-specific apps

**Cons:**
- Longer execution time
- Higher GitHub Actions cost (macOS runners use 10x minutes)
- More complex to maintain

## Option 3: Conditional Platform Builds

**Best for:** When you need iOS/macOS builds occasionally but want to keep CI fast.

```yaml
name: Conditional Platform Build

on:
  push:
	branches: [main]
  pull_request:
	paths:
	  - 'src/Sumapap.Reporting.Maui/**'
	  - '.github/workflows/**'

jobs:
  android-build:
	runs-on: ubuntu-latest
	steps:
	  - name: Checkout
		uses: actions/checkout@v4

	  - name: Setup .NET
		uses: actions/setup-dotnet@v4
		with:
		  dotnet-version: '10.0.x'

	  - name: Install Android Workload
		run: dotnet workload install maui-android

	  - name: Build
		run: dotnet build -c Release

  ios-build:
	# Only run on manual trigger or release branches
	if: github.event_name == 'workflow_dispatch' || contains(github.ref, 'release')
	runs-on: macos-latest
	steps:
	  - name: Checkout
		uses: actions/checkout@v4

	  - name: Setup .NET
		uses: actions/setup-dotnet@v4
		with:
		  dotnet-version: '10.0.x'

	  - name: Install iOS Workload
		run: dotnet workload install maui-ios

	  - name: Build
		run: dotnet build -c Release
```

**Pros:**
- Fast CI for most PRs (Android only)
- iOS validation when needed
- Cost-effective

**Cons:**
- Requires manual triggering or specific branch patterns
- iOS issues might not be caught early

## Recommendation for Sumapap Project

For your current project structure (primarily NuGet packages with one MAUI component), I recommend:

**Stick with Option 1 (Current Setup)** for regular CI/CD because:
1. NuGet packages build fine with just Android workload
2. Most code is platform-agnostic
3. Faster feedback and lower costs
4. `Sumapap.Reporting.Maui` can be tested on Android

**Add Option 3 selectively** if you need iOS-specific validation:
- Create a separate workflow: `.github/workflows/ios-validation.yaml`
- Set it to `workflow_dispatch` (manual trigger)
- Run before releases or when MAUI code changes

## Cost Considerations

GitHub Actions minute consumption:
- Linux: 1x (1 minute = 1 minute)
- macOS: 10x (1 minute = 10 minutes)
- Windows: 2x (1 minute = 2 minutes)

**Example:** A 5-minute build across all 4 platforms:
- Linux (Android): 5 minutes
- macOS (iOS): 50 minutes
- macOS (Catalyst): 50 minutes  
- Windows: 10 minutes
- **Total: 115 billable minutes**

vs. Android-only: **5 billable minutes**

## Implementation

If you decide to switch to multi-platform builds later, let me know and I can create the appropriate workflow files for your needs.
