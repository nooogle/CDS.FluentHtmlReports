# Release Process Guide

This document describes how to create and publish new releases of CDS.FluentHtmlReports.

## Prerequisites

### First-Time Setup

1. **NuGet API Key**
   - Go to https://www.nuget.org/account/apikeys
   - Create a new API key with "Push" permissions for this package
   - Add it as a GitHub secret named `NUGET_API_KEY`:
     - Go to: https://github.com/nooogle/CDS.FluentHtmlReports/settings/secrets/actions
     - Click "New repository secret"
     - Name: `NUGET_API_KEY`
     - Value: Your NuGet API key

2. **Codecov Token (Optional for code coverage)**
   - Go to https://codecov.io and sign in with GitHub
   - Add your repository
   - Copy the upload token
   - Add it as a GitHub secret named `CODECOV_TOKEN`

## Creating a Release

The project uses [MinVer](https://github.com/adamralph/minver) for automatic semantic versioning based on git tags.

### Version Format

Follow [Semantic Versioning](https://semver.org/):
- `v{MAJOR}.{MINOR}.{PATCH}` (e.g., `v1.0.0`, `v2.1.3`)

**When to increment:**
- **MAJOR**: Breaking API changes
- **MINOR**: New features (backward compatible)
- **PATCH**: Bug fixes (backward compatible)

### Step-by-Step Release Process

1. **Ensure all changes are committed and pushed**
   ```bash
   git status
   git push origin master
   ```

2. **Wait for CI to pass**
   - Check: https://github.com/nooogle/CDS.FluentHtmlReports/actions
   - Ensure all tests pass on all platforms

3. **Update CHANGELOG.md**
   ```bash
   # Move items from [Unreleased] to new version section
   # Add date and update comparison links
   ```

4. **Commit the CHANGELOG update**
   ```bash
   git add CHANGELOG.md
   git commit -m "Prepare v1.2.3 release"
   git push origin master
   ```

5. **Create and push the version tag**
   ```bash
   # For version 1.2.3:
   git tag v1.2.3
   git push origin v1.2.3
   ```

6. **GitHub Actions will automatically:**
   - Build the project
   - Run all tests
   - Create NuGet package
   - Create GitHub release with auto-generated notes
   - Publish to NuGet.org
   - Attach the `.nupkg` file to the GitHub release

7. **Verify the release**
   - Check GitHub releases: https://github.com/nooogle/CDS.FluentHtmlReports/releases
   - Check NuGet.org: https://www.nuget.org/packages/CDS.FluentHtmlReports
   - The package may take 5-10 minutes to appear in search after publishing

## Pre-release Versions

For pre-release versions (alpha, beta, RC):

```bash
# Create a tag with pre-release identifier
git tag v1.2.3-alpha.1
git push origin v1.2.3-alpha.1

# Or
git tag v1.2.3-beta.1
git push origin v1.2.3-beta.1

# Or
git tag v1.2.3-rc.1
git push origin v1.2.3-rc.1
```

MinVer will automatically mark these as pre-release versions in NuGet.

## Manual Release (Emergency)

If the automatic workflow fails, you can manually release:

```bash
# Build and pack
dotnet build --configuration Release
dotnet pack --configuration Release --output ./artifacts

# Push to NuGet
dotnet nuget push ./artifacts/CDS.FluentHtmlReports.*.nupkg \
  --api-key YOUR_NUGET_API_KEY \
  --source https://api.nuget.org/v3/index.json
```

## Rolling Back a Release

**NuGet packages cannot be deleted**, but you can unlist them:

1. Go to https://www.nuget.org/packages/CDS.FluentHtmlReports
2. Click "Manage Package"
3. Select the version and click "Unlist"
4. The version will no longer appear in search or package manager UIs
5. Create a new patch version with the fix

## Checking Current Version

The version is calculated from git tags by MinVer:

```bash
# See the current calculated version
dotnet build --verbosity normal | grep MinVer

# Or use MinVer CLI
dotnet tool install --global minver-cli
minver
```

## Troubleshooting

### Build fails on release
- Check the GitHub Actions log
- Ensure all dependencies are properly referenced
- Verify the project builds locally: `dotnet build --configuration Release`

### NuGet push fails
- Verify your API key is correctly set in GitHub secrets
- Check if the version already exists on NuGet.org
- Ensure the API key has "Push" permissions

### Version is not what you expected
- MinVer calculates version from git tags
- Ensure you've fetched all tags: `git fetch --tags`
- Check tag format follows `v{major}.{minor}.{patch}`
- View all tags: `git tag -l`

### Tests fail during release
- Fix the tests first!
- Never bypass failing tests to create a release
- If urgent, fix and create a patch release

## Additional Resources

- [Semantic Versioning](https://semver.org/)
- [Keep a Changelog](https://keepachangelog.com/)
- [MinVer Documentation](https://github.com/adamralph/minver)
- [GitHub Actions Documentation](https://docs.github.com/en/actions)
