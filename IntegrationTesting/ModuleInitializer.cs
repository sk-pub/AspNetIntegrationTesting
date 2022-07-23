using ImageMagick;
using System.Runtime.CompilerServices;

namespace IntegrationTesting
{
    public static class ModuleInitializer
    {
        private const string SamplesDirectory = "Samples";

        [ModuleInitializer]
        public static void Initialize()
        {
            ConfigureVerifier();

            RegisterImageMagick();
        }

        private static void ConfigureVerifier()
        {
            VerifierSettings.DerivePathInfo(
                (sourceFile, projectDirectory, type, method) =>
                {
                    return new(
                        directory: Path.Combine(projectDirectory, SamplesDirectory),
                        typeName: type.Name,
                        methodName: method.Name);
                });
        }

        private static void RegisterImageMagick()
        {
            //VerifyImageMagick.RegisterPdfToPngConverter();
            RegisterPdfToPngWithMaskConverter();
            VerifyImageMagick.RegisterComparers(0.001, ErrorMetric.PerceptualHash);
        }

        private static void RegisterPdfToPngWithMaskConverter() =>
        VerifierSettings.RegisterFileConverter(
            "pdf",
            (stream, context) => Convert(stream, context, MagickFormat.Pdf));

        static ConversionResult Convert(Stream stream, IReadOnlyDictionary<string, object> context, MagickFormat magickFormat)
        {
            var streams = new List<Stream>();

            MagickReadSettings magickSettings;
            if (context.TryGetValue("ImageMagick.MagickReadSettings", out var magickSettingsObj))
            {
                magickSettings = magickSettingsObj as MagickReadSettings;
            }
            else
            {
                magickSettings = new()
                {
                    Density = new(100, 100)
                };
            }

            magickSettings.Format = magickFormat;
            using var images = new MagickImageCollection();
            images.Read(stream, magickSettings);
            var count = images.Count;
            if (context.TryGetValue("ImageMagick.PagesToInclude", out var pagesToInclude))
            {
                count = Math.Min(count, (int)pagesToInclude);
            }

            for (var index = 0; index < count; index++)
            {
                var image = images[index];

                AddMask(context, image, index);

                var memoryStream = new MemoryStream();
                image.Write(memoryStream, MagickFormat.Png);
                streams.Add(memoryStream);
            }

            return new(null, streams.Select(x => new Target("png", x)));
        }

        static void AddMask(IReadOnlyDictionary<string, object> context, IMagickImage image, int index)
        {
            context.GetUseMask(out var mask);
            if (mask == null)
            {
                return;
            }

            var projectDirectory = AttributeReader.GetProjectDirectory();

            var maskPath = Path.IsPathFullyQualified(mask)
                ? mask
                : Path.Combine(projectDirectory, SamplesDirectory, mask);

            maskPath = maskPath.Replace("*", index.ToString("D2"));
            if (!File.Exists(maskPath))
            {
                return;
            }

            using var maskImage = new MagickImage(maskPath);

            image.Composite(maskImage, CompositeOperator.Multiply);
        }
    }
}
