using System.Runtime.CompilerServices;

namespace ComponentTesting
{
    public static class ModuleInitializer
    {
        [ModuleInitializer]
        public static void Initialize()
        {
            CommonVerification.Configure(typeof(ModuleInitializer));
        }
    }
}
