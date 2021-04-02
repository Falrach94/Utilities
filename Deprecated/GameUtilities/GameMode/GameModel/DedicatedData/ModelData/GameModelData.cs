using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameUtilities.GameMode
{
    public abstract class GameModelData : IGameModelData
    {
        protected GameModelData(int playerNum)
        {
            PlayerNum = playerNum;
        }

        public int PlayerNum { get; }
        public bool GameRunning { get; private set; } = true;

        public void AssertGameRunning()
        {
            if(!GameRunning)
            {
                throw new GameOverException();
            }
        }

    }
}
