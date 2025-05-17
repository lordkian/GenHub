using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace GenHub.Linux;

// This class plays the roll of Windows Registry 
public class SystemApps
{
    // there should be only 1 instance
    public static SystemApps Instance { get; private set; }
    // list of Apps (binaries) to look for
    public static readonly string[] AppnNames = ["steam", "flatpak", "lutris"];
    public List<string> Path { get; private set; } = new List<string>();
    public List<string> Apps { get; private set; } = new List<string>();
    public List<string> FlatpackApps { get; private set; } = new List<string>();
    static SystemApps()
    {
        Instance = new SystemApps();
    }

    private SystemApps()
    {
        var path = Environment.GetEnvironmentVariable("PATH");
        if (path != null)
            Path.AddRange(path.Split(":").Where(Directory.Exists));
        foreach (var item in AppnNames)
        {
            var res = Whereis(item);
            if (res.Length > 0)
                Apps.Add(res);
        }
    }

    private string Whereis(string item)
    {
        throw new NotImplementedException();
    }
}