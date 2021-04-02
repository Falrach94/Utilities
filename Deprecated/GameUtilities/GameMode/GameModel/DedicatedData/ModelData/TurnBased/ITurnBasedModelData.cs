using System;
using System.Collections.Generic;
using System.Text;

namespace GameUtilities.GameMode.TurnBased
{
    public interface ITurnBasedModelData
    {
        int ActivePlayer { get; }
        int TurnNumber { get; }

        void AssertActivePlayer(int player);

    }
}
