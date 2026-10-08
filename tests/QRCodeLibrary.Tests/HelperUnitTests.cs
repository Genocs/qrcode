using System.Reflection;

namespace Genocs.QRCodeLibrary.UnitTests;

public static class HelperUnitTests
{
    public static string GetLocationOfExecutingAssembly()
    {
        string? directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (string.IsNullOrEmpty(directory))
        {
            throw new InvalidOperationException("Could not resolve the test assembly directory.");
        }

        return directory;
    }

    public static string GetDemoFileFolder()
    {
        return Path.Combine(GetLocationOfExecutingAssembly(), "DemoFiles");
    }

    public static string GetDemoFile(string filename)
    {
        return Path.Combine(GetDemoFileFolder(), filename);
    }
}
