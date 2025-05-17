namespace GenHub.Linux;

// This class plays the roll of Windows Registry 
public class SystemApps
{
    // there should be only 1 instance
    public static SystemApps Instance { get; private set; }

    static SystemApps()
    {
        Instance = new SystemApps();
    }

    private SystemApps()
    {
        
    }
}