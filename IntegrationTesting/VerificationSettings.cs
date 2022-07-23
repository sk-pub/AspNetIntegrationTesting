using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTesting
{
    public static class VerificationSettings
    {
        public static void UseMask(this VerifySettings settings, string maskPath) =>
            settings.Context["Verification.UseMask"] = maskPath;

        public static SettingsTask UseMask(this SettingsTask settings, string maskPath)
        {
            settings.CurrentSettings.UseMask(maskPath);
            return settings;
        }

        internal static bool GetUseMask(this IReadOnlyDictionary<string, object> context, [NotNullWhen(true)] out string? maskPath)
        {
            if (context.TryGetValue("Verification.UseMask", out var value))
            {
                maskPath = (string)value;
                return true;
            }

            maskPath = null;
            return false;
        }
    }
}
