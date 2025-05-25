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