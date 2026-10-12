using System.Reflection;

namespace FleetMaintenance.Core;

// Повертає версію збірки з Directory.Build.props.
public static class VersionInfo
{
    public static string Current()
    {
        var assembly = typeof(VersionInfo).Assembly;
        var attribute = assembly.GetCustomAttribute<
            AssemblyInformationalVersionAttribute>();
        var raw = attribute?.InformationalVersion ?? "0.0.0";
        var plus = raw.IndexOf('+');
        return plus < 0 ? raw : raw[..plus];
    }
}
