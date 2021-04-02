using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameUtilities.GameMode.TurnBased
{
    public abstract class TurnBasedModelData : GameModelData, ITurnBasedModelData
    {
        protected TurnBasedModelData(int playerNum, int activePlayer)
            : base(playerNum)
        {
            ActivePlayer = activePlayer;
        }

        public int ActivePlayer { get; private set; } = 0;

        public int TurnNumber { get; private set; } = 0;

        public void AssertActivePlayer(int player)
        {
            if(player != ActivePlayer)
            {
                throw new NotActivePlayerException();
            }
        }


    }
}
