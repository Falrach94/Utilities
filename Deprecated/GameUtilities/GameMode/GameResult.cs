using GameManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameUtilities.ServerBackend
{
    public class GameResult : IGameResult
    {
        public GameResult(bool aborted, object details)
        {
            Aborted = aborted;
            Details = details;
        }

        public bool Aborted { get; }

        public object Details { get; }
    }
}
