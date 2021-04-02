using GameManagement;

namespace GameUtilities.GameMode
{
    public interface IGameModeInfo
    {
        string Name { get; }
        int MinPlayer { get; }
        int MaxPlayer { get; }
    }
}