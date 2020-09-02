using System.IO;
using System.Reflection;

namespace Genocs.QRCodeLibrary.Tests
{
    public static class HelperUnitTests
    {
        public static string GetLocationOfExecutingAssembly()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        public static string GetDemoFilefolder()
        {
            string fullPath = GetLocationOfExecutingAssembly();
            return @$"{fullPath}\Demofiles";
        }

        public static string GetDemoFile(string filename)
        {
            string fullPath = GetLocationOfExecutingAssembly();
            return @$"{fullPath}\Demofiles\{filename}";
        }
    }
}
