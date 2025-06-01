namespace GenHub.Core;

public interface IGameRunner
{
    public IGameInstallation GameInstallation { get; }
    public List<IMod> Mods { get; } // mods should be implemented later and ignored for now
    
    public void Run(string[] parameters);
}