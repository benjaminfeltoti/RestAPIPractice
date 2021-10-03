using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestAPIPractice
{
    public static class Settings
    {
        private static string documentsFolderPath;

        public static string DocumentsFolderPath => documentsFolderPath ??= new SettingsReader().GetPath();
    }
}
