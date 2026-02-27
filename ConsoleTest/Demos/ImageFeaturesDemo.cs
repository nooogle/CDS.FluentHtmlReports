using CDS.FluentHtmlReports;
using SkiaSharp;
using System.Text;

namespace ConsoleTest.Demos;

/// <summary>
/// Demonstrates all image-related features: inline SVG, base64 bytes, streams,
/// URL-referenced images, sizing, alignment, and captions.
/// All SVG content in this demo is generated inline — no external files or
/// image-processing libraries are required.
/// </summary>
public static class ImageFeaturesDemo
{
    /// <summary>Holds pre-computed SkiaSharp image bytes so they can be passed into the report sections.</summary>
    private sealed record SkiaImages(byte[] Original, byte[] Thumbnail, byte[] Watermarked, string Dimensions);

    // ── Entry point ──────────────────────────────────────────────────────────

    /// <summary>
    /// Builds the full Image Features demo report. The method intentionally reads
    /// as a table of contents — each private section method covers one feature area.
    /// </summary>
    public static string Generate()
    {
        var skia = LoadSkiaImages();

        var g = Generator
            .Create("Image Features Demo")
            .AddParagraph("This report demonstrates every image method in the library. All SVG is generated inline — no external files or image-processing libraries are required for the SVG sections.")
            .AddLabelValueRow([
                ("Generated", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                ("Methods", "AddSvg · AddImage(bytes) · AddImage(stream) · AddImageFromUrl"),
                ("Self-contained", "Yes (SVG & base64)")
            ]);

        AddInlineSvgSection(g);
        AddAlignmentSection(g);
        AddCaptionsSection(g);
        AddBytesAndStreamsSection(g);
        AddSkiaSharpSection(g, skia);
        AddUrlImagesSection(g);
        AddColumnsLayoutSection(g);

        return g.AddLine().AddFooter().Generate();
    }

    // ── Demo sections ────────────────────────────────────────────────────────

    /// <summary>Demonstrates AddSvg() with inline SVG markup at various widths.</summary>
    private static void AddInlineSvgSection(Generator g)
    {
        var pipeline = (string caption) => g.AddSvg(
            StatusIndicatorSvg("Build Pipeline",
            [
                ("Checkout",    "#4CAF50", "passed"),
                ("Build",       "#4CAF50", "passed"),
                ("Unit Tests",  "#4CAF50", "passed"),
                ("Integration", "#FF9800", "partial"),
                ("Deploy",      "#9E9E9E", "skipped")
            ]),
            caption: caption);

        g.AddLine()
         .AddHeading("Inline SVG  —  AddSvg()")
         .AddParagraph("The most efficient way to embed vector graphics. The SVG markup is written directly into the HTML — no encoding overhead, fully self-contained, and scales perfectly at every zoom level.")
         .AddSvg(BannerSvg(), caption: "A gradient banner generated in C# without any image library")
         .AddLine(LineType.Dashed)
         .AddHeading("Inline SVG: maxWidthPercent", HeadingLevel.H3)
         .AddParagraph("The same SVG constrained to different widths via the optional maxWidthPercent parameter:");

        pipeline("100% width (full container)");

        g.AddSvg(StatusIndicatorSvg("Build Pipeline",
            [
                ("Checkout",    "#4CAF50", "passed"),
                ("Build",       "#4CAF50", "passed"),
                ("Unit Tests",  "#4CAF50", "passed"),
                ("Integration", "#FF9800", "partial"),
                ("Deploy",      "#9E9E9E", "skipped")
            ]),
            maxWidthPercent: 60, alignment: ImageAlignment.Center, caption: "60% width, centered");

        g.AddSvg(StatusIndicatorSvg("Build Pipeline",
            [
                ("Checkout",    "#4CAF50", "passed"),
                ("Build",       "#4CAF50", "passed"),
                ("Unit Tests",  "#4CAF50", "passed"),
                ("Integration", "#FF9800", "partial"),
                ("Deploy",      "#9E9E9E", "skipped")
            ]),
            maxWidthPercent: 40, alignment: ImageAlignment.Left, caption: "40% width, left-aligned");
    }

    /// <summary>Demonstrates the alignment parameter: left, center, right.</summary>
    private static void AddAlignmentSection(Generator g)
    {
        g.AddLine()
         .AddHeading("Alignment — left, center, right")
         .AddParagraph("All image methods accept an optional alignment parameter. Here the same SVG badge is placed at each of the three positions:")
         .AddSvg(BadgeSvg("#1976D2", "Left-aligned image"),   maxWidthPercent: 35, alignment: ImageAlignment.Left)
         .AddSvg(BadgeSvg("#4CAF50", "Centered image"),        maxWidthPercent: 35, alignment: ImageAlignment.Center)
         .AddSvg(BadgeSvg("#F44336", "Right-aligned image"),   maxWidthPercent: 35, alignment: ImageAlignment.Right);
    }

    /// <summary>Demonstrates the caption parameter, which wraps the image in a figure/figcaption.</summary>
    private static void AddCaptionsSection(Generator g)
    {
        g.AddLine()
         .AddHeading("Captions — figure + figcaption")
         .AddParagraph("Pass a caption string to any image method to wrap the image in a semantic HTML5 <figure>/<figcaption> element:")
         .AddSvg(
             SparklineSvg("Weekly Active Users", [420, 380, 510, 630, 590, 720, 810]),
             maxWidthPercent: 70,
             alignment: ImageAlignment.Center,
             caption: "Figure 1 — Weekly active users for the past 7 days");
    }

    /// <summary>Demonstrates AddImage(byte[]) and AddImage(Stream), plus cross-platform library guidance.</summary>
    private static void AddBytesAndStreamsSection(Generator g)
    {
        g.AddLine()
         .AddHeading("Base64 Embedding — AddImage(byte[]) and AddImage(Stream)")
         .AddParagraph("When you have image data as a byte array or Stream — for example from a cross-platform image library — use these overloads. The image is base64-encoded into the HTML, keeping the report self-contained.")
         .AddImage(Encoding.UTF8.GetBytes(BadgeSvg("#9C27B0", "Embedded via AddImage(byte[])")), "image/svg+xml",
             alt: "Purple badge", maxWidthPercent: 45, alignment: ImageAlignment.Left,
             caption: "AddImage(byte[], mimeType) — SVG bytes embedded as base64")
         .AddImage(ToStream(BadgeSvg("#00BCD4", "Embedded via AddImage(Stream)")), "image/svg+xml",
             alt: "Teal badge", maxWidthPercent: 45, alignment: ImageAlignment.Right,
             caption: "AddImage(Stream, mimeType) — read from a MemoryStream")
         .AddLine(LineType.Blank)
         .AddHeading("Cross-platform image sources (macOS / Linux / Windows)", HeadingLevel.H3)
         .AddParagraph("This library has zero dependencies and does not ship an image-processing package. To produce bitmap data from code, use one of these cross-platform libraries:")
         .AddCodeBlock("""
             // ── SkiaSharp (recommended cross-platform) ──────────────────────────────
             // NuGet: SkiaSharp

             using SkiaSharp;

             using var surface = SKSurface.Create(new SKImageInfo(400, 200));
             var canvas = surface.Canvas;
             canvas.Clear(SKColors.White);
             // ... draw on canvas ...
             using var image = surface.Snapshot();
             using var data = image.Encode(SKEncodedImageFormat.Png, 100);

             // Option A — byte array
             report.AddImage(data.ToArray(), "image/png", "My chart");

             // Option B — stream (avoids the extra byte[] allocation)
             using var stream = data.AsStream();
             report.AddImage(stream, "image/png", "My chart");
             """, "csharp")
         .AddCodeBlock("""
             // ── SixLabors.ImageSharp (cross-platform) ──────────────────────────────
             // NuGet: SixLabors.ImageSharp

             using SixLabors.ImageSharp;
             using SixLabors.ImageSharp.Formats.Png;
             using SixLabors.ImageSharp.PixelFormats;

             using var img = new Image<Rgba32>(400, 200);
             // ... draw on img ...
             using var ms = new MemoryStream();
             img.Save(ms, new PngEncoder());
             ms.Position = 0;
             report.AddImage(ms, "image/png", caption: "Figure 1");
             """, "csharp")
         .AddCodeBlock("""
             // ── System.Drawing.Bitmap (Windows only) ───────────────────────────────
             // Note: System.Drawing.Common is Windows-only on .NET 6+.
             // Throws PlatformNotSupportedException on macOS and Linux.

             using var bmp = new System.Drawing.Bitmap(400, 200);
             // ... draw on bmp ...
             using var ms = new MemoryStream();
             bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
             report.AddImage(ms.ToArray(), "image/png");
             """, "csharp")
         .AddAlert(AlertLevel.Info, "Tip: for purely vector output, AddSvg() requires no image library at all. Build an SVG string in C# and embed it directly — fully self-contained and resolution-independent.");
    }

    /// <summary>
    /// Demonstrates real SkiaSharp image processing: load, resize, and watermark.
    /// ConsoleTest.csproj references SkiaSharp; the library itself has zero dependencies.
    /// </summary>
    private static void AddSkiaSharpSection(Generator g, SkiaImages skia)
    {
        g.AddLine()
         .AddHeading("SkiaSharp: Real Cross-Platform Image Processing")
         .AddParagraph("The ConsoleTest project references SkiaSharp (NuGet: SkiaSharp 3.x). The three images below are produced by the private helper methods in this class — they load the sample PNG from disk and apply real pixel operations.")
         .AddAlert(AlertLevel.Info, "The library itself (CDS.FluentHtmlReports) has zero dependencies. SkiaSharp is referenced only by this demo project, showing how any caller can integrate a cross-platform image library on Windows, macOS, and Linux.")
         .AddMetadata("Source image", "docs/images/example-report.png")
         .AddMetadata("Dimensions", skia.Dimensions)

         .AddHeading("1. Load and re-encode  —  SKBitmap.Decode + SKImage.Encode", HeadingLevel.H3)
         .AddParagraph("The original file is decoded into an SKBitmap, then re-encoded to PNG bytes and embedded via AddImage(byte[]):")
         .AddImage(skia.Original, "image/png",
             alt: "Original image loaded with SkiaSharp",
             maxWidthPercent: 90, alignment: ImageAlignment.Center,
             caption: "Original — SKBitmap.Decode() → SKImage.Encode() → AddImage(byte[], \"image/png\")")
         .AddCodeBlock("""
             using var bitmap = SKBitmap.Decode(sampleImagePath);
             using var image  = SKImage.FromBitmap(bitmap);
             using var data   = image.Encode(SKEncodedImageFormat.Png, quality: 90);

             report.AddImage(data.ToArray(), "image/png", alt: "My image");
             """, "csharp")

         .AddHeading("2. Resize to thumbnail  —  SKBitmap.Resize", HeadingLevel.H3)
         .AddParagraph("The image is scaled down to 280 px wide using Mitchell cubic resampling, then embedded at 35% of the report width:")
         .AddImage(skia.Thumbnail, "image/png",
             alt: "Thumbnail created with SkiaSharp",
             maxWidthPercent: 35, alignment: ImageAlignment.Left,
             caption: "Thumbnail (280 px wide) — SKBitmap.Resize() with SKCubicResampler.Mitchell")
         .AddCodeBlock("""
             using var original = SKBitmap.Decode(sampleImagePath);
             int targetWidth  = 280;
             int targetHeight = (int)((float)original.Height / original.Width * targetWidth);

             using var resized = original.Resize(
                 new SKImageInfo(targetWidth, targetHeight),
                 new SKSamplingOptions(SKCubicResampler.Mitchell));

             using var image = SKImage.FromBitmap(resized!);
             using var data  = image.Encode(SKEncodedImageFormat.Png, 90);

             report.AddImage(data.ToArray(), "image/png",
                 maxWidthPercent: 35, alignment: ImageAlignment.Left);
             """, "csharp")

         .AddHeading("3. Watermark  —  SKCanvas compositing", HeadingLevel.H3)
         .AddParagraph("An SKSurface is created at the original dimensions, the bitmap is drawn onto it, then a diagonal semi-transparent \"DRAFT\" text is composited using SKCanvas:")
         .AddImage(skia.Watermarked, "image/png",
             alt: "Watermarked image produced by SkiaSharp",
             maxWidthPercent: 90, alignment: ImageAlignment.Center,
             caption: "Watermarked — SKSurface + SKCanvas.DrawBitmap + SKCanvas.DrawText")
         .AddCodeBlock("""
             using var bitmap  = SKBitmap.Decode(sampleImagePath);
             using var surface = SKSurface.Create(new SKImageInfo(bitmap.Width, bitmap.Height));
             var canvas = surface.Canvas;

             canvas.DrawBitmap(bitmap, 0, 0);

             using var font  = new SKFont(SKTypeface.Default, size: 72);
             using var paint = new SKPaint { Color = new SKColor(200, 30, 30, 140), IsAntialias = true };
             float textWidth = font.MeasureText("DRAFT");

             canvas.Save();
             canvas.Translate(bitmap.Width / 2f, bitmap.Height / 2f);
             canvas.RotateDegrees(-35);
             canvas.DrawText("DRAFT", -textWidth / 2f, font.Size / 3f, font, paint);
             canvas.Restore();

             using var image = surface.Snapshot();
             using var data  = image.Encode(SKEncodedImageFormat.Png, 90);

             report.AddImage(data.ToArray(), "image/png");
             """, "csharp");
    }

    /// <summary>Demonstrates AddImageFromUrl(), including the self-contained caveat.</summary>
    private static void AddUrlImagesSection(Generator g)
    {
        g.AddLine()
         .AddHeading("URL-referenced Images — AddImageFromUrl()")
         .AddParagraph("Instead of embedding image data, this overload emits a standard <img src=\"url\"> tag. The report file stays small, but the image is only visible when the viewer has network access.")
         .AddAlert(AlertLevel.Warning, "URL images are NOT self-contained. They require network access at viewing time. Do not use them for emailed attachments or fully offline reports.")
         .AddImageFromUrl(
             "https://picsum.photos/640/480",
             alt: "Placeholder image loaded from an external URL",
             maxWidthPercent: 100,
             alignment: ImageAlignment.Center,
             caption: "Loaded from picsum.photos — requires internet access")
         .AddCodeBlock("""
             // Suitable for: intranet dashboards, stable CDN assets, live reports.
             // Not suitable for: email attachments, fully offline reports.

             report.AddImageFromUrl(
                 "https://example.com/company-logo.png",
                 alt: "Company logo",
                 maxWidthPercent: 30,
                 alignment: ImageAlignment.Left,
                 caption: "Our logo");
             """, "csharp");
    }

    /// <summary>Demonstrates image methods used inside BeginColumns/EndColumns.</summary>
    private static void AddColumnsLayoutSection(Generator g)
    {
        g.AddLine()
         .AddHeading("Images inside the Columns layout")
         .AddParagraph("Images work inside BeginColumns/EndColumns for side-by-side comparisons:")
         .BeginColumns()
             .BeginColumn()
                 .AddSvg(SparklineSvg("CPU Usage %", [45, 60, 55, 80, 72, 90, 68]),
                     caption: "CPU usage — last 7 intervals")
             .EndColumn()
             .BeginColumn()
                 .AddSvg(SparklineSvg("Memory MB", [210, 215, 220, 230, 225, 240, 235]),
                     caption: "Memory usage — last 7 intervals")
             .EndColumn()
         .EndColumns();
    }

    // ── SVG generators ───────────────────────────────────────────────────────

    /// <summary>A gradient banner — demonstrates a rich decorative image built purely in code.</summary>
    private static string BannerSvg()
    {
        return """
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 800 140" width="800" height="140">
              <defs>
                <linearGradient id="bg" x1="0%" y1="0%" x2="100%" y2="0%">
                  <stop offset="0%"   stop-color="#1976D2"/>
                  <stop offset="100%" stop-color="#42A5F5"/>
                </linearGradient>
              </defs>
              <rect width="800" height="140" fill="url(#bg)" rx="8"/>
              <text x="400" y="58" font-family="Segoe UI,sans-serif" font-size="28" font-weight="bold"
                    fill="white" text-anchor="middle">CDS.FluentHtmlReports</text>
              <text x="400" y="92" font-family="Segoe UI,sans-serif" font-size="16"
                    fill="rgba(255,255,255,0.85)" text-anchor="middle">Image Features Demo</text>
              <text x="400" y="118" font-family="Segoe UI,sans-serif" font-size="13"
                    fill="rgba(255,255,255,0.65)" text-anchor="middle">Inline SVG · Base64 · Stream · URL · Alignment · Captions</text>
            </svg>
            """;
    }

    /// <summary>A horizontal pipeline status strip — coloured circles with labels.</summary>
    private static string StatusIndicatorSvg(string title, (string label, string color, string status)[] stages)
    {
        const int circleR = 18;
        const int spacing = 140;
        const int startX = 70;
        const int cy = 55;

        int width = startX * 2 + spacing * (stages.Length - 1);
        int height = 100;

        var sb = new StringBuilder();
        sb.AppendLine($"""<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 {width} {height}" width="{width}" height="{height}">""");
        sb.AppendLine($"""  <text x="{width / 2}" y="20" font-family="Segoe UI,sans-serif" font-size="13" font-weight="600" fill="#555" text-anchor="middle">{title}</text>""");

        for (int i = 0; i < stages.Length; i++)
        {
            int cx = startX + i * spacing;
            var (label, color, status) = stages[i];

            // connector line (not before the first)
            if (i > 0)
            {
                int prevCx = startX + (i - 1) * spacing;
                sb.AppendLine($"""  <line x1="{prevCx + circleR}" y1="{cy}" x2="{cx - circleR}" y2="{cy}" stroke="#dee2e6" stroke-width="2"/>""");
            }

            sb.AppendLine($"""  <circle cx="{cx}" cy="{cy}" r="{circleR}" fill="{color}"/>""");
            // simple tick / dot symbol
            string symbol = status == "passed" ? "✓" : status == "partial" ? "~" : "–";
            sb.AppendLine($"""  <text x="{cx}" y="{cy + 5}" font-family="Segoe UI,sans-serif" font-size="14" font-weight="bold" fill="white" text-anchor="middle">{symbol}</text>""");
            sb.AppendLine($"""  <text x="{cx}" y="{cy + circleR + 14}" font-family="Segoe UI,sans-serif" font-size="11" fill="#555" text-anchor="middle">{label}</text>""");
        }

        sb.AppendLine("</svg>");
        return sb.ToString();
    }

    /// <summary>A coloured pill badge — used to demonstrate alignment.</summary>
    private static string BadgeSvg(string color, string text)
    {
        return $"""
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 300 50" width="300" height="50">
              <rect width="300" height="50" fill="{color}" rx="25"/>
              <text x="150" y="34" font-family="Segoe UI,sans-serif" font-size="16" font-weight="600"
                    fill="white" text-anchor="middle">{text}</text>
            </svg>
            """;
    }

    /// <summary>A simple sparkline chart — demonstrates data-driven SVG generation in C#.</summary>
    private static string SparklineSvg(string title, int[] values)
    {
        const int w = 340;
        const int h = 110;
        const int padL = 10;
        const int padR = 10;
        const int padT = 30;
        const int padB = 20;

        int chartW = w - padL - padR;
        int chartH = h - padT - padB;
        int min = values.Min();
        int max = values.Max();
        int range = max == min ? 1 : max - min;

        double xStep = (double)chartW / (values.Length - 1);

        var points = values
            .Select((v, i) => (
                x: padL + i * xStep,
                y: padT + chartH - (double)(v - min) / range * chartH
            ))
            .ToArray();

        var polyline = string.Join(" ", points.Select(p => $"{p.x:F1},{p.y:F1}"));
        var area = $"{padL},{padT + chartH} " + polyline + $" {padL + chartW},{padT + chartH}";

        var sb = new StringBuilder();
        sb.AppendLine($"""<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 {w} {h}" width="{w}" height="{h}">""");
        sb.AppendLine($"""  <rect width="{w}" height="{h}" fill="#f8f9fa" rx="6"/>""");
        sb.AppendLine($"""  <text x="{w / 2}" y="18" font-family="Segoe UI,sans-serif" font-size="12" font-weight="600" fill="#555" text-anchor="middle">{title}</text>""");
        sb.AppendLine($"""  <polygon points="{area}" fill="#1976D2" fill-opacity="0.15"/>""");
        sb.AppendLine($"""  <polyline points="{polyline}" fill="none" stroke="#1976D2" stroke-width="2" stroke-linejoin="round"/>""");

        // dots
        foreach (var (x, y) in points)
        {
            sb.AppendLine($"""  <circle cx="{x:F1}" cy="{y:F1}" r="3.5" fill="#1976D2"/>""");
        }

        // min/max labels
        var maxPt = points[Array.IndexOf(values, max)];
        var minPt = points[Array.IndexOf(values, min)];
        sb.AppendLine($"""  <text x="{maxPt.x:F1}" y="{maxPt.y - 6:F1}" font-family="Segoe UI,sans-serif" font-size="10" fill="#1976D2" text-anchor="middle">{max}</text>""");
        sb.AppendLine($"""  <text x="{minPt.x:F1}" y="{minPt.y + 13:F1}" font-family="Segoe UI,sans-serif" font-size="10" fill="#999" text-anchor="middle">{min}</text>""");

        sb.AppendLine("</svg>");
        return sb.ToString();
    }

    // ── SkiaSharp helpers ─────────────────────────────────────────────────────

    /// <summary>
    /// Loads and processes the sample image via SkiaSharp, returning all pre-computed
    /// variants as a <see cref="SkiaImages"/> record.
    /// The image is copied to the output directory via the .csproj Content item.
    /// </summary>
    private static SkiaImages LoadSkiaImages()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Assets", "example-report.png");
        return new SkiaImages(
            Original:    SkiaLoadPng(path),
            Thumbnail:   SkiaResizePng(path, targetWidth: 280),
            Watermarked: SkiaWatermarkPng(path, "DRAFT"),
            Dimensions:  SkiaDimensions(path));
    }

    /// <summary>Returns a string describing the pixel dimensions of the image at <paramref name="path"/>.</summary>
    private static string SkiaDimensions(string path)    {
        using var bitmap = SKBitmap.Decode(path);
        return $"{bitmap.Width} × {bitmap.Height} px";
    }

    /// <summary>Loads the image and re-encodes it as a PNG byte array.</summary>
    private static byte[] SkiaLoadPng(string path)
    {
        using var bitmap = SKBitmap.Decode(path);
        using var image  = SKImage.FromBitmap(bitmap);
        using var data   = image.Encode(SKEncodedImageFormat.Png, quality: 90);
        return data.ToArray();
    }

    /// <summary>Resizes the image to <paramref name="targetWidth"/> pixels wide and returns PNG bytes.</summary>
    private static byte[] SkiaResizePng(string path, int targetWidth)
    {
        using var original = SKBitmap.Decode(path);
        int targetHeight = (int)((float)original.Height / original.Width * targetWidth);

        using var resized = original.Resize(
            new SKImageInfo(targetWidth, targetHeight),
            new SKSamplingOptions(SKCubicResampler.Mitchell));

        using var image = SKImage.FromBitmap(resized!);
        using var data  = image.Encode(SKEncodedImageFormat.Png, quality: 90);
        return data.ToArray();
    }

    /// <summary>
    /// Composites a diagonal semi-transparent watermark text over the image
    /// using <see cref="SKCanvas"/> and returns PNG bytes.
    /// </summary>
    private static byte[] SkiaWatermarkPng(string path, string text)
    {
        using var bitmap  = SKBitmap.Decode(path);
        using var surface = SKSurface.Create(new SKImageInfo(bitmap.Width, bitmap.Height));
        var canvas = surface.Canvas;

        // Draw the original image as the background
        canvas.DrawBitmap(bitmap, 0, 0);

        // Composite a diagonal text watermark
        using var font  = new SKFont(SKTypeface.Default, size: 72);
        using var paint = new SKPaint { Color = new SKColor(200, 30, 30, 140), IsAntialias = true };
        float textWidth = font.MeasureText(text);

        canvas.Save();
        canvas.Translate(bitmap.Width / 2f, bitmap.Height / 2f);
        canvas.RotateDegrees(-35);
        canvas.DrawText(text, -textWidth / 2f, font.Size / 3f, font, paint);
        canvas.Restore();

        using var image = surface.Snapshot();
        using var data  = image.Encode(SKEncodedImageFormat.Png, quality: 90);
        return data.ToArray();
    }

    // ── General helpers ───────────────────────────────────────────────────────

    private static Stream ToStream(string text)
    {
        var ms = new MemoryStream(Encoding.UTF8.GetBytes(text));
        return ms;
    }
}
