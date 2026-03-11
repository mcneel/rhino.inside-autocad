using Rhino.Inside.AutoCAD.Core.Interfaces;

namespace Rhino.Inside.AutoCAD.Services;

/// <inheritdoc cref="IApplicationVersionHistory"/>
public class ApplicationVersionHistory : IApplicationVersionHistory
{
    private readonly Version _noVersion = new();
    private readonly List<Version> _versions = new List<Version>();

    /// <summary>
    /// Constructs a new <see cref="ApplicationVersionHistory"/>.
    /// </summary>
    public ApplicationVersionHistory(string rootInstallDirectory)
    {

        if (Directory.Exists(rootInstallDirectory))
        {
            var directories = Directory.GetDirectories(rootInstallDirectory);

            foreach (var directory in directories)
            {
                var folderName = Path.GetFileName(directory);

                Version.TryParse(folderName, out var version);

                if (version is not null)
                    _versions.Add(version);
            }

            _versions.Sort();
        }

        _versions.Reverse();

    }

    /// <inheritdoc />
    public Version GetCurrentVersion() => _versions.FirstOrDefault() ?? _noVersion;

    /// <inheritdoc />
    public Version GetVersion(int index) => _versions.ElementAt(index) ?? _noVersion;

}