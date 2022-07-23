﻿using System.Runtime.CompilerServices;

namespace IntegrationTesting
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
