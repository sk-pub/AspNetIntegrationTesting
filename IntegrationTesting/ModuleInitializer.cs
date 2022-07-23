using System.Runtime.CompilerServices;

namespace IntegrationTesting
{
    public static class ModuleInitializer
    {
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
                        directory: Path.Combine(projectDirectory, "Samples"),
                        typeName: type.Name,
                        methodName: method.Name);
                });
        }

        private static void RegisterImageMagick()
        {
            VerifyImageMagick.RegisterPdfToPngConverter();
            VerifyImageMagick.RegisterComparers(0.001, ImageMagick.ErrorMetric.PerceptualHash);
        }
    }
}
