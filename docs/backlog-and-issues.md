# Backlog & Known Issues

Running list of ideas, planned work, and known limitations for the
GroupDocs.Markdown MCP server. Grouped by topic. Terse on purpose — each line is
a ticket, not an essay. `[ ]` = open, `[x]` = shipped (kept for context).

**Current surface (26.9.0):** `convert_to_markdown`, `get_document_info`.

---

## Confirmed defects — external audit, 2026-08-16

Source: black-box test round against `ghcr.io/groupdocs-markdown/markdown-net-mcp:latest`
(26.7.2, licensed), 46 family-wide defects reported and all 46 independently reproduced with
control calls. A later validation round found **zero false positives**.

`S#` = shared core (`GroupDocs.Mcp.Core`) · `M#` = this repo · `P#` = GroupDocs.Markdown library

**Verdict: conversion quality is good and Unicode-safe.** Headings, bold, lists and hyperlinks
preserved; `数字签名`, em-dash and ✔ all survive into valid UTF-8. Notably **the Signature
product's QR text-corruption bug does not exist here.** Two wrapper gaps around silent behaviour,
three converter-fidelity gaps.

### Shared core — fixed once in `GroupDocs.Mcp.Core`, lands here on the next bump

- [ ] **S1** Passing `fileName` crashes any tool — **High**. *Proof:*
      `get_document_info {"file":{"fileName":"03_pages_text.pdf"}}` → opaque error; unhandled
      `ArgumentException` at `FileResolver.ResolveAsync` → `GetDocumentInfoTool.cs:30`.
- [ ] **S2** Missing files return an opaque error — **High**. Applies to **both** tools (shared
      resolver); the `Available files:` listing stays in stderr.
- [ ] **S3** `isError` is set on crashes but not on real failures — **Med**.

Nothing to do in this repo for S1–S3 beyond re-testing after the Core bump.

### MCP wrapper — this repo

- [ ] **M1** Out-of-range `pages` silently returns the whole document — **Med**.
      *Proof:* `pages: "99"` on a 3-page PDF returned `Converted … to Markdown` and wrote the
      complete unfiltered document — byte-identical (4680 B) to the call with no `pages` at all.
      Control: `pages: "2"` wrote 2531 B, so filtering does work; only the invalid case is silent.
      *Cause:* `ConvertToMarkdownTool.ParsePages` drops invalid entries → `null` → whole document.
      *Fix:* validate the range against the page count and return errors-as-text.
      **P1** — silent wrong answers are the worst kind for an agent.
- [ ] **M2** Fixed output name silently overwrites previous results — **Low**.
      *Proof:* a full-document conversion wrote `15_pages_docx.md` (113 B); a later `pages:"1"`
      conversion of the same source rewrote the same path (71 B). The first output is gone, with no
      warning in either response. `rewrite:true` at `ConvertToMarkdownTool.cs:76`.
      *Fix:* adopt the `' (N)'` dedup suffix Merger, Watermark and Total already use — or let
      callers pass an output name. **P1**

### Product library — upstream

- [ ] **P1** `.md` files are rejected as input — **Low**.
      *Proof:* `get_document_info {"file":{"filePath":"sample-notes.md"}}` →
      `NotSupportedException: No converter available for file extension '.md'.` Same from
      `convert_to_markdown`.
      *Impact:* the Markdown product cannot inspect or re-process **its own format**, so round-trip
      and fidelity-check workflows are impossible. May well be by design — if so, say it in the
      tool description. **P2**
- [ ] **P2** Plain-text conversion wraps lines in stray backticks — **Low**.
      *Proof:* a TXT input produced the whole line wrapped in backticks plus an extra pair around
      the CJK run. Characters are intact, but a renderer shows broken code spans. **P2**
- [ ] **P3** PDF numbered outlines flatten to a single level — **Low**.
      *Proof:* a TOC with `1.1` / `1.1.1` numbering came out as twelve identical top-level `1.`
      items — numbering and nesting both lost. **P2**

P2/P3 are converter fidelity — batch them into the next engine pass. P1 may only need a
description sentence.

---

## Known issues & limitations

- `images: 'file'` extracts images to a `<name>_images/` subfolder with correct relative links.
- `frontMatter` emits correct YAML; `pages` filtering works for valid page numbers.
- `fileContent` (base64) + `fileName` input works here — a usable path for files outside storage.
- Engine is on **26.3.0**, the oldest in the family (others are 26.4.0–26.8.0). Worth a bump pass;
  P2/P3 may already be addressed upstream.

---

## Tools & functionality

- [ ] **M1** validate `pages` against the page count. **P1**
- [ ] **M2** align output naming with the family `' (N)'` convention, or accept an output name.
      **P1**
- [ ] `convert_to_markdown` — expose the image-extraction folder name. **P2**

## Testing & CI

- [ ] Add the two mandatory probes: the **`fileName`-only form**, and a **missing file** asserting
      the promised `Available files:` text. Today's oracle passes on the exact defect. **P1**
- [ ] Regression test for M1: out-of-range `pages` must error, not return the full document. **P1**
- [ ] Regression test for M2: convert twice, assert the first output still exists. **P1**
- [ ] Add a `channel: [dnx, docker]` axis — the current matrix is dnx-only. **P1**
- [ ] Per-tool Linux smoke test in image CI. **P1**
- [ ] Not covered today: password-protected conversion with a correct password;
      PPTX/HTML/EPUB/MOBI/CHM beyond negative probes. **P2**
- [ ] macOS integration leg hangs (family-wide) — `timeout-minutes: 20` is committed locally but
      unpushed here. Push it, and stream the `dnx` child's stderr to an uploaded file. **P1**

## Documentation & discoverability

- [ ] State the `.md`-input boundary in the tool description (P1 above). **P2**
- [ ] Document the output-naming policy once M2 lands. **P1**
- [ ] Licensing section covering the metered option once it ships. **P1**
- [ ] Refresh the MCP Registry description when the tool set changes.

## Platform & infra (longer-term)

- [ ] Metered licensing (`GROUPDOCS_METERED_PUBLIC_KEY` / `_PRIVATE_KEY`) via
      `GroupDocs.Mcp.Core`, plus the `get_license_status` tool. **P1**
- [ ] Engine bump from 26.3.0 — oldest in the family. **P1**
- [ ] HTTP/SSE transport for shared/team deploys (stdio stays default). **P2**

---

*Evidence: `TEMP_ThirdPartyAnalysis/markdown.md` (per-product findings),
`ALL-PRODUCTS-REPORT.md` (10-product sweep), `VALIDATION-REPORT.md` (why the green suites miss
these). Conventions: any behaviour change ships with a `changelog/NNN-*.md` entry and a CalVer
bump. Integration tests target the published NuGet via `dnx`, so new-tool tests only pass once the
matching version is live.*
