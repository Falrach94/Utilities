using GameManagement;
using GameUtilities.GameMode;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameUtilities.ServerBackend
{
    public interface IServerGameMode
    {
        IGameModel Model { get; }

        IServerGameBackend Backend { get; }

    }
}
