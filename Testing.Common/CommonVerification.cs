using ImageMagick;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Testing.Common
{
    public static partial class CommonVerification
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

        private static void RegisterComparers()
        {
            VerifierSettings.RegisterFileConverter("pdf", Convert);
            VerifyImageMagick.RegisterComparers(0.005, ErrorMetric.Fuzz);
        }
    }
}
