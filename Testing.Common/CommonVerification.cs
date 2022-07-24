using ImageMagick;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Testing.Common
{
    public static class CommonVerification
    {
        private static Assembly _assembly;

        public static void Configure(Type parentType)
        {
            _assembly = parentType.Assembly;

            VerifierSettings.DerivePathInfo(
                (sourceFile, projectDirectory, type, method) =>
                {
                    return new(
                        directory: Path.Combine(projectDirectory, type.Name),
                        typeName: type.Name,
                        methodName: method.Name);
                });

            RegisterComparers();
        }

        public static bool GetUseMask(this IReadOnlyDictionary<string, object> context, [NotNullWhen(true)] out string? maskPath)
        {
            if (context.TryGetValue("Verification.UseMask", out var value))
            {
                maskPath = (string)value;
                return true;
            }

            maskPath = null;
            return false;
        }

        public static void UseMask(this VerifySettings settings, string maskPath) =>
                            settings.Context["Verification.UseMask"] = maskPath;

        public static SettingsTask UseMask(this SettingsTask settings, string maskPath)
        {
            settings.CurrentSettings.UseMask(maskPath);
            return settings;
        }
        static void AddMask(IReadOnlyDictionary<string, object> context, IMagickImage image, int index)
        {
            context.GetUseMask(out var mask);
            if (mask == null)
            {
                return;
            }

            var projectDirectory = AttributeReader.GetProjectDirectory(_assembly);

            var maskPath = Path.IsPathFullyQualified(mask)
                ? mask
                : Path.Combine(projectDirectory, mask);

            maskPath = maskPath.Replace("*", index.ToString("D2"));
            if (!File.Exists(maskPath))
            {
                return;
            }

            using var maskImage = new MagickImage(maskPath);

            image.Composite(maskImage, CompositeOperator.Multiply);
        }

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

        private static void RegisterComparers()
        {
            RegisterPdfToPngWithMaskConverter();
            VerifyImageMagick.RegisterComparers(0.01, ErrorMetric.PerceptualHash);
        }

        private static void RegisterPdfToPngWithMaskConverter() =>
            VerifierSettings.RegisterFileConverter(
                "pdf",
                (stream, context) => Convert(stream, context, MagickFormat.Pdf));
    }
}
