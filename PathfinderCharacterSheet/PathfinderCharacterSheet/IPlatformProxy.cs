namespace PathfinderCharacterSheet
{
    public interface IPlatformProxy
    {
        string PersistentDataPath { get; }
        string GetVersionNumber { get; }
        string GetBuildNumber { get; }
    }
}
