# Sumapap CI/CD Architecture for Multi-Platform MAUI Packages

## Problem Statement

**Challenge**: Create a NuGet package (`Sumapap.Reporting.Maui`) that supports Android, iOS, and macOS Catalyst, while using GitHub Actions runners that have platform limitations.

**Platform Constraints**:
- ✅ **Linux runners**: Can build `net10.0-android` only (no iOS/Catalyst workloads)
- ✅ **macOS runners**: Can build `net10.0-android`, `net10.0-ios`, `net10.0-maccatalyst`
- ✅ **Windows runners**: Can build `net10.0-android`, `net10.0-windows`

## Solution Architecture

### Two-Stage Pipeline Approach (Cost-Optimized)

```
┌─────────────────────────────────────────────────────────────┐
│  STAGE 1: CI - Pull Request Validation (Linux, Fast)       │
│  ────────────────────────────────────────────────────────   │
│  • Removes MAUI projects from solution before restore       │
│  • Builds all 14 non-MAUI projects                          │
│  • Runs all non-MAUI tests                                  │
│  • Generates code coverage                                  │
│  • Fast feedback (~3-5 minutes)                             │
│  • Cost: ~5 billable minutes per run                        │
└─────────────────────────────────────────────────────────────┘
							↓
┌─────────────────────────────────────────────────────────────┐
│  STAGE 2: CD - NuGet Publishing (macOS, Complete)          │
│  ────────────────────────────────────────────────────────   │
│  • Builds ALL 15 projects including MAUI                    │
│  • Sumapap.Reporting.Maui built with all targets:          │
│    - net10.0-android                                        │
│    - net10.0-ios                                            │
│    - net10.0-maccatalyst                                    │
│  • Packs complete multi-platform NuGet packages             │
│  • Publishes to NuGet.org                                   │
│  • Cost: ~100-200 billable minutes per run (10x macOS rate)│
└─────────────────────────────────────────────────────────────┘
```

## Project Configuration

### Sumapap.Reporting.Maui.csproj

```xml
<PropertyGroup>
  <!-- Target all MAUI platforms for complete NuGet package -->
  <TargetFrameworks>net10.0-android;net10.0-ios;net10.0-maccatalyst</TargetFrameworks>

  <!-- Optionally add Windows when building on Windows -->
  <TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('windows'))">
	$(TargetFrameworks);net10.0-windows10.0.19041.0
  </TargetFrameworks>
</PropertyGroup>
```

**Key Points**:
- ✅ Always targets Android, iOS, and macOS Catalyst
- ✅ No Linux-specific conditions (would break NuGet package completeness)
- ✅ Windows target added conditionally for local development
- ⚠️ **Cannot be built on Linux CI** (intentional, handled by workflow)

## Workflow Details

### CI Workflow (`.github/workflows/ci-test-coverage.yaml`)

**Runner**: `ubuntu-latest` (Linux)

**Purpose**: Fast validation of non-MAUI code

**Strategy**:
```yaml
- name: Remove MAUI Projects from Solution
  run: |
	for csproj in src/**/*.csproj; do
	  if grep -q '<UseMaui>true</UseMaui>' "$csproj"; then
		dotnet sln src/Sumapap.slnx remove "$csproj" || true
	  fi
	done

- name: Restore Dependencies
  run: |
	dotnet restore src/Sumapap.slnx

- name: Build Source Projects
  run: |
	dotnet build src/Sumapap.slnx -c Release --no-restore
```

**What Runs**:
- ✅ Builds: 14 non-MAUI projects
- ⏭️ Excluded: `Sumapap.Reporting.Maui` (removed from solution)
- ✅ Tests: All non-MAUI unit tests
- ✅ Coverage: Code coverage for non-MAUI projects

**Why Remove from Solution?**
1. **Workload Requirement**: iOS/Catalyst workloads not available on Linux
2. **Restore Failure**: MAUI projects fail during restore without workloads
3. **Clean Approach**: Removing before restore avoids all errors
4. **Cost Efficiency**: Linux is 10x cheaper than macOS (~$5 vs ~$100/month)

### CD Workflow (`.github/workflows/cd-publish-nuget.yaml`)

**Runner**: `macos-latest` (macOS)

**Purpose**: Create complete multi-platform NuGet packages

**Strategy**:
```yaml
jobs:
  publish-nuget:
	runs-on: macos-latest  # Required for iOS/Catalyst

	steps:
	  - name: Install .NET MAUI Workloads
		run: |
		  dotnet workload install maui-android
		  dotnet workload install maui-ios
		  dotnet workload install maui-maccatalyst

	  - name: Build Projects
		run: dotnet build src/ -c Release --no-restore

	  - name: Process and Pack Projects
		run: |
		  for file in src/**/*.csproj; do
			dotnet pack "$file" -c Release --no-build -o .nuget/
		  done
```

**What Runs**:
- ✅ Builds: ALL 15 projects including MAUI
- ✅ Packs: Multi-platform NuGet packages
  - `Sumapap.Reporting.Maui.nupkg` contains:
	- `lib/net10.0-android/Sumapap.Reporting.Maui.dll`
	- `lib/net10.0-ios/Sumapap.Reporting.Maui.dll`
	- `lib/net10.0-maccatalyst/Sumapap.Reporting.Maui.dll`
- ✅ Publishes: To NuGet.org

**Why macOS for CD?**
1. **Complete Package**: Only macOS can build all MAUI targets
2. **User Experience**: Consumers can use the package on any platform
3. **One Source of Truth**: Single workflow creates production artifacts
4. **Infrequent Runs**: Only runs on merge to main or manual trigger

## NuGet Package Completeness

### What Goes into the NuGet Package?

When `dotnet pack` runs on **macOS** with all workloads:

```
Sumapap.Reporting.Maui.1.0.0.nupkg
├── lib/
│   ├── net10.0-android/
│   │   └── Sumapap.Reporting.Maui.dll       (Android binary)
│   ├── net10.0-ios/
│   │   └── Sumapap.Reporting.Maui.dll       (iOS binary)
│   └── net10.0-maccatalyst/
│       └── Sumapap.Reporting.Maui.dll       (Mac Catalyst binary)
├── docs/
│   └── Sumapap.Reporting.Maui.md
└── [package metadata]
```

### Consumer Experience

When a developer installs the package:

```xml
<!-- Android MAUI App -->
<TargetFramework>net10.0-android</TargetFramework>
<!-- ✅ Uses: lib/net10.0-android/Sumapap.Reporting.Maui.dll -->

<!-- iOS MAUI App -->
<TargetFramework>net10.0-ios</TargetFramework>
<!-- ✅ Uses: lib/net10.0-ios/Sumapap.Reporting.Maui.dll -->

<!-- macOS MAUI App -->
<TargetFramework>net10.0-maccatalyst</TargetFramework>
<!-- ✅ Uses: lib/net10.0-maccatalyst/Sumapap.Reporting.Maui.dll -->
```

**Result**: ✅ Package works on all platforms seamlessly!

## Cost Analysis

### Current Architecture

**Per PR (multiple commits)**:
- CI runs: 3-10 times per PR × 5 minutes = 15-50 billable minutes
- Total cost per PR: ~$0.01-0.03 (at $0.008/minute for Linux)

**Per Release**:
- CD runs: 1 time × 15 minutes × 10x (macOS rate) = 150 billable minutes
- Total cost per release: ~$1.20 (at $0.008/minute × 10 for macOS)

**Monthly Estimate** (20 PRs, 4 releases):
- CI: 20 PRs × ~30 min avg = 600 minutes → ~$5/month
- CD: 4 releases × 150 min = 600 minutes → ~$5/month
- **Total: ~$10/month**

### Alternative: All Builds on macOS

**Per PR**: 3-10 runs × 20 min × 10x = 600-2000 minutes → $5-16/PR
**Monthly**: 20 PRs + 4 releases = ~24,000 minutes → **~$192/month**

**Savings**: ~$182/month (95% cost reduction)

## Trade-offs

### ✅ Advantages

1. **Fast PR Feedback**: 3-5 minutes on Linux vs 15-20 minutes on macOS
2. **Cost Effective**: 95% cost savings vs all-macOS approach
3. **Complete Packages**: NuGet packages contain all platform binaries
4. **Developer Experience**: Local dev works on any platform
5. **Scalable**: Can handle many PRs without budget concerns

### ⚠️ Considerations

1. **MAUI-Specific Issues**: Won't be caught until CD stage
2. **Two-Stage Validation**: Platform issues discovered later in pipeline
3. **macOS Dependency**: CD requires macOS runners (vendor lock-in)

### 🎯 Mitigations

1. **Pre-commit Hooks**: Validate MAUI builds locally before pushing
2. **Draft PRs**: Test MAUI changes in CD before final PR
3. **Manual Triggers**: Run CD workflow manually for MAUI-specific PRs
4. **Local Testing**: Developers building MAUI locally catch issues early

## Best Practices

### For MAUI Changes

1. **Before Pushing**:
   ```bash
   # Build locally on macOS to verify all platforms
   dotnet build src/Sumapap.Reporting.Maui/Sumapap.Reporting.Maui.csproj
   ```

2. **Draft PR Workflow**:
   - Create draft PR
   - Manually trigger CD workflow
   - Verify macOS build passes
   - Convert to ready for review

3. **PR Description**:
   - Note if MAUI code changed
   - Confirm local build on macOS passed
   - List platforms tested

### For Non-MAUI Changes

1. **Standard Workflow**:
   - Push to PR
   - CI validates automatically
   - Merge when green

2. **No Special Actions Needed**:
   - Linux CI covers all non-MAUI code
   - Fast feedback loop
   - High confidence in changes

## Future Enhancements

### Option 1: Conditional macOS CI

Add a label-triggered macOS CI build:

```yaml
on:
  pull_request:
	types: [opened, synchronize, labeled]

jobs:
  maui-validation:
	if: contains(github.event.pull_request.labels.*.name, 'maui')
	runs-on: macos-latest
	# ... full MAUI build
```

**Use**: Add `maui` label to PRs that change MAUI code

### Option 2: Path-Based Triggers

Automatically trigger macOS build when MAUI files change:

```yaml
on:
  pull_request:
	paths:
	  - 'src/Sumapap.Reporting.Maui/**'

jobs:
  maui-validation:
	runs-on: macos-latest
	# ... full MAUI build
```

### Option 3: Hybrid Approach

- **Default**: Linux CI (fast)
- **MAUI Label**: Add macOS CI job
- **Release Branch**: Always run macOS CI

## Conclusion

This architecture provides:
- ✅ **Fast CI** for daily development (Linux)
- ✅ **Complete packages** for production (macOS)
- ✅ **Cost efficiency** (95% savings)
- ✅ **Developer flexibility** (any platform for local dev)
- ✅ **User satisfaction** (packages work everywhere)

The two-stage pipeline balances speed, cost, and completeness while ensuring high-quality, multi-platform NuGet packages.
