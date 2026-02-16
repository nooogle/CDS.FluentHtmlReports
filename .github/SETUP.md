# GitHub Actions Setup - Quick Start

This repository now has automated CI/CD! Here's what you need to know.

## 🚀 What's Automated

### ✅ Continuous Integration (CI)
**Triggers:** Push to master/main/develop, Pull Requests
**Workflow:** `.github/workflows/ci.yml`

On every push and PR, GitHub Actions will:
- Build the project on **Ubuntu, Windows, and macOS**
- Run all tests on **.NET 8 and .NET 10**
- Upload test results
- Generate code coverage reports
- Upload coverage to Codecov (optional)

**Status Badge:** 
```markdown
[![CI](https://github.com/nooogle/CDS.FluentHtmlReports/actions/workflows/ci.yml/badge.svg)](https://github.com/nooogle/CDS.FluentHtmlReports/actions/workflows/ci.yml)
```

### 🔒 Security Scanning (CodeQL)
**Triggers:** Push to master/main, PRs, Weekly schedule
**Workflow:** `.github/workflows/codeql.yml`

Automatically scans code for security vulnerabilities using GitHub's CodeQL.

**Status Badge:**
```markdown
[![CodeQL](https://github.com/nooogle/CDS.FluentHtmlReports/actions/workflows/codeql.yml/badge.svg)](https://github.com/nooogle/CDS.FluentHtmlReports/actions/workflows/codeql.yml)
```

### 📦 Release Automation
**Triggers:** Git tags matching `v*.*.*`
**Workflow:** `.github/workflows/release.yml`

When you push a version tag (e.g., `v1.2.3`), GitHub Actions will:
1. Build and test the project
2. Create a NuGet package
3. Create a GitHub Release with auto-generated notes
4. Publish the package to NuGet.org
5. Attach the `.nupkg` file to the release

## 🔧 Initial Setup

### 1. Add NuGet API Key (Required for releases)

1. Get your API key from https://www.nuget.org/account/apikeys
2. Go to: https://github.com/nooogle/CDS.FluentHtmlReports/settings/secrets/actions
3. Click **"New repository secret"**
4. Name: `NUGET_API_KEY`
5. Value: Your NuGet API key
6. Click **"Add secret"**

### 2. Enable Codecov (Optional)

If you want code coverage reporting:

1. Go to https://codecov.io
2. Sign in with GitHub
3. Add your repository
4. Copy the upload token
5. Add as GitHub secret: `CODECOV_TOKEN`
6. Badge will appear in README

### 3. Enable GitHub Discussions (Optional)

For community questions:

1. Go to: https://github.com/nooogle/CDS.FluentHtmlReports/settings
2. Scroll to "Features"
3. Check "Discussions"
4. Click "Set up discussions"

## 📋 Daily Usage

### Working on a Feature

```bash
# Create a branch
git checkout -b feature/my-new-feature

# Make changes and commit
git add .
git commit -m "Add new feature"

# Push to GitHub
git push origin feature/my-new-feature
```

**What happens:**
- CI runs automatically on your branch
- You can see results in the GitHub Actions tab
- Create a PR when ready

### Creating a Pull Request

1. Push your branch to GitHub
2. Go to the repository and click "Pull Request"
3. Fill in the PR template
4. CI will run automatically
5. Reviewers can see test results before merging

### Releasing a New Version

See [RELEASE.md](RELEASE.md) for detailed instructions.

**Quick version:**
```bash
# Commit all changes
git add .
git commit -m "Prepare v1.2.3 release"
git push origin master

# Wait for CI to pass
# Then create and push the tag
git tag v1.2.3
git push origin v1.2.3

# GitHub Actions does the rest!
```

## 📊 Monitoring

### View CI Results
https://github.com/nooogle/CDS.FluentHtmlReports/actions

### View Releases
https://github.com/nooogle/CDS.FluentHtmlReports/releases

### View on NuGet
https://www.nuget.org/packages/CDS.FluentHtmlReports

### View Code Coverage (if enabled)
https://codecov.io/gh/nooogle/CDS.FluentHtmlReports

## 🔍 Troubleshooting

### CI Failing?
1. Check the Actions tab for error details
2. Look at the specific job that failed
3. Tests must pass on **all platforms** (Ubuntu, Windows, macOS)
4. Tests must pass on **both** .NET 8 and .NET 10

### Release Not Publishing?
1. Verify `NUGET_API_KEY` is set in repository secrets
2. Check the Release workflow in Actions tab
3. Ensure tag format is correct: `v{major}.{minor}.{patch}`
4. Check NuGet.org for any issues

### Badge Not Showing?
- Badges may take a few minutes to update after the first run
- Ensure workflows have run at least once
- Check badge URL matches your repository name

## 📚 Additional Files

- **CHANGELOG.md** - Track all changes by version
- **CONTRIBUTING.md** - Guidelines for contributors
- **RELEASE.md** - Detailed release process guide
- **.github/ISSUE_TEMPLATE/** - Bug report and feature request templates
- **.github/pull_request_template.md** - PR template

## 🎯 Best Practices

1. **Never commit directly to master** - Use branches and PRs
2. **Wait for CI to pass** - Don't merge failing PRs
3. **Update CHANGELOG.md** - Document all changes
4. **Write tests** - All new features need tests
5. **Follow semantic versioning** - MAJOR.MINOR.PATCH
6. **Review generated release notes** - Edit if needed

## 🆘 Need Help?

- Open an issue with the "question" label
- Check existing issues and discussions
- Review GitHub Actions logs for error details

---

**Pro Tip:** Add branch protection rules to require CI to pass before merging PRs:
1. Go to Settings → Branches
2. Add rule for `master` branch
3. Require status checks to pass: `Build & Test`
