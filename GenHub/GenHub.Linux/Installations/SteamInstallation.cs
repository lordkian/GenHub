using System.IO;
using GenHub.Core;

namespace GenHub.Linux.Installations;

public class SteamInstallation : IGameInstallation
{
    // IGameInstallation
    public GameInstallationType InstallationType => GameInstallationType.Steam;
    public bool IsVanillaInstalled { get; private set; }
    public string VanillaGamePath { get; private set; } = "";
    public bool IsZeroHourInstalled { get; private set; }
    public string ZeroHourGamePath { get; private set; } = "";

    // Linux specific
    private SystemApps systemApps = SystemApps.Instance; // for convince

    //Steam specific
    public bool IsSteamInstalled { get; private set; }

    public SteamInstallation(bool fetch)
    {
        if (fetch)
            Fetch();
    }

    public void Fetch()
    {
        // TODO add flatpack and lutris support

        IsSteamInstalled = DoesSteamPathExist();
    }

    /// <summary>
    /// Tries to fetch all steam library folders containing installed games.
    /// </summary>
    /// <param name="steamLibraryPaths">An array of full paths to the common directories for each valid library found.
    /// This will be null if the method returns <c>false</c>.</param>
    /// <returns><c>true</c> if at least one steam library path was found.</returns>
    /// <remarks>
    /// This method reads the "libraryfolders.vdf" file from the main steam installation directory.
    /// </remarks>
    private bool TryGetSteamLibraries(out string[]? steamLibraryPaths)
    {
        steamLibraryPaths = null;

        // Try to get the steam path in order to fetch the libraryfolders.vdf
        if (!TryGetSteamPath(out var steamPath))
            return false;

        // Find libraryfolders.vdf
        var libraryFile = Path.Combine(steamPath!, "steamapps", "libraryfolders.vdf");

        if(!File.Exists(libraryFile))
            return false;

        List<string> results = [];

        // Read all the paths in the vdf and already make them usable.
        // "C:\\Program Files (x86)\\Steam" to "C:\Program Files (x86)\Steam\steamapps\common"
        foreach (var line in File.ReadAllLines(libraryFile))
        {
            if (!line.Contains("\"path\""))
                continue;

            var parts = line.Split('"');
            if(parts.Length < 4)
                continue;

            var dir = parts[3].Trim();

            if (Directory.Exists(dir))
            {
                var path = Path.Combine(dir.Replace(@"\\", @"\"), "steamapps", "common");
                results.Add(path);
            }
        }

        if (results.Count == 0)
            return false;

        steamLibraryPaths = results.ToArray();

        return true;
    }
    
    /// <summary>
    /// Checks if steam is installed by looking up the installation path of steam.
    /// on Linux there must be a reference in .steam directory in HOME dir.
    /// </summary>
    /// <returns><c>true</c> if the steam directory was found</returns>
    private bool DoesSteamPathExist()
    {
        if (!systemApps.Apps.ContainsKey("steam") || systemApps.Apps["steam"].Length == 0)
            return false;
        if (string.IsNullOrEmpty(systemApps.HomeAddr))
            return false;
        if (!Directory.Exists(Path.Combine(systemApps.HomeAddr, ".steam")))
            return false;
        // in this case either steam has not been run for the first time of main dir is corrupted
        return Directory.Exists(Path.Combine(systemApps.HomeAddr, ".steam", "steam")) &&
               Directory.GetFiles(Path.Combine(systemApps.HomeAddr, ".steam", "steam")).Length != 0;
        // TODO for 100% acc we need more testing and finding edge cases
    }
    
    /// <summary>
    /// returns installation path of steam on linux.
    /// this method exist for compatibility and similarity between Linux and Windows version of this class 
    /// </summary>
    /// <param name="path">Returns the installation path if successful; otherwise, an empty string</param>
    /// <returns><c>true</c> if the installation path of steam was found</returns>
    private bool TryGetSteamPath(out string? path)
    {
        path = string.Empty;
        if (!DoesSteamPathExist())
            return false;
        var homePath = Path.Combine(systemApps.HomeAddr, ".steam");
        
        // this depends on softlink inside .steam dir in user's home
        path = Path.Combine(homePath, "steam");
        
        return true;
    }
}