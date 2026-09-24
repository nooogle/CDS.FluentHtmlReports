# Release Process Guide

How to publish a new version of CDS.FluentHtmlReports.

## How it works

Pushing a tag matching `V*.*.*` runs [`.github/workflows/release.yml`](.github/workflows/release.yml), which:

1. Builds and runs all tests (Release configuration)
2. Packs the NuGet package — the version comes from the tag via [MinVer](https://github.com/adamralph/minver)
3. Generates a CycloneDX SBOM and attests build provenance and the SBOM
4. Creates a GitHub Release with generated notes, attaching the `.nupkg` and `bom.json`
5. Publishes to NuGet.org using [Trusted Publishing](https://learn.microsoft.com/nuget/nuget-org/trusted-publishing) (OIDC — no long-lived API key)

## One-time setup

Already done for this repo; listed so it can be checked or rebuilt.

- **nuget.org trusted publishing policy** for owner `nooogle`, repository `CDS.FluentHtmlReports`, workflow `release.yml`, environment `nuget`.
- **GitHub environment** `nuget` — the release job runs in it.
- **Repository secret** `NUGET_USER` — the nuget.org account name the policy belongs to. This is not an API key; the short-lived key is issued per run by `NuGet/login`.

## Versioning

[Semantic Versioning](https://semver.org/), tagged as `V{MAJOR}.{MINOR}.{PATCH}` with an **uppercase `V`** — a lowercase `v` tag does not trigger the workflow.

- **MAJOR** — breaking API or behaviour changes
- **MINOR** — new features, backward compatible
- **PATCH** — bug fixes, backward compatible

Between tags MinVer derives pre-release versions from commit height, so only tag when deliberately cutting a release.

## Release notes

There is no `CHANGELOG.md`. The GitHub Release is the release record: its notes list the PRs merged since the previous tag, by title, grouped by label using [`.github/release.yml`](.github/release.yml).

So before merging a PR that will ship:

- **Give it a descriptive title** — that title is the release note.
- **Label it** — `breaking-change`, `bug`, `enhancement` or `documentation`. Dependabot labels its own PRs `dependencies`. Unlabelled PRs appear under *Other changes*; `ignore-for-release` leaves a PR out.

If a release changes behaviour in a way users must act on, edit the GitHub Release after it is created and add a short note above the generated list.

## Creating a release

1. Merge the PRs to `master` and check CI is green.
2. Tag and push:
   ```bash
   git checkout master
   git pull
   git tag --list "V*" --sort=-v:refname   # find the latest
   git tag V1.2.3
   git push origin V1.2.3
   ```
3. Watch the run: https://github.com/nooogle/CDS.FluentHtmlReports/actions/workflows/release.yml
4. Verify:
   - GitHub Release: https://github.com/nooogle/CDS.FluentHtmlReports/releases
   - NuGet: https://www.nuget.org/packages/CDS.FluentHtmlReports (can take 5–10 minutes to be searchable)

Never delete and re-push a published tag — nuget.org will not accept the same version twice.

## Pre-release versions

```bash
git tag V1.2.3-beta.1
git push origin V1.2.3-beta.1
```

MinVer marks the package as a pre-release. The GitHub Release is still created as a normal release; edit it and tick *Set as a pre-release* if needed.

## Rolling back

NuGet packages cannot be deleted, only unlisted:

1. https://www.nuget.org/packages/CDS.FluentHtmlReports → **Manage Package**
2. Select the version → **Unlist**
3. Release a fixed patch version

## Troubleshooting

- **Workflow didn't start** — check the tag starts with an uppercase `V` and was pushed (`git push origin <tag>`; a plain `git push` does not push tags).
- **Version isn't what you expected** — MinVer needs the tags locally: `git fetch --tags`. CI checks out with `fetch-depth: 0` for the same reason.
- **NuGet login fails** — check the trusted publishing policy on nuget.org still matches the repository, workflow file name and environment, and that `NUGET_USER` is set.
- **NuGet push reports the version exists** — the push uses `--skip-duplicate`, so re-running an already-published tag succeeds without publishing anything. Cut a new version instead.
- **Tests fail during release** — fix them and release a new patch; never bypass failing tests.
