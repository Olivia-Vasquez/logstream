# chgsmart v1.0 Scope Document

**chgsmart** is a command-line tool for git repositories that ingests commit history and generates structured changelogs. It enables developers to quickly create documentation from existing commit messages using heuristic analysis, even when messages are inconsistently formatted.

## Target Users

chgsmart is designed for solo developers and teams who use git and write commit messages that loosely follow best practices. It is especially helpful for projects where commit message quality varies.

## Core Features (v1.0)

- Simple installation via `go install` in any git repository
- Reads and synthesizes git commit history
- Robust handling of poorly written or inconsistently formatted commit messages
- Groups commits by type (e.g., Added, Fixed, Changed, etc.)
- Generates a clear, coherent, and helpful `CHANGELOG.md`
- Allows users to specify the output directory for changelogs
- Consistent flagging and command-line options

## Non-Goals (v1.0)

- No GitHub API integration
- No graphical user interface or web dashboard
- No automated version bumping
- No rewriting or modifying commit history
- No mandatory Homebrew publishing (optional stretch goal)

## Success Criteria

- Users can install chgsmart via `go install`
- Running chgsmart in any valid git repository produces a well-formatted changelog
- Project includes:
    - README documentation
    - Version tagging
    - Release notes
    - License file
    - Clean and organized folder structure
    - No open high-priority bugs

---

This scope document defines the goals and boundaries for chgsmart v1.0, ensuring a focused and reliable tool for changelog generation.