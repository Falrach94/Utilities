using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameManagement.GameManagementModule.Data
{
    [Serializable]
    public abstract class GameSettings
    {
        public string GameMode { get; set; }

        public Dictionary<string, object> SettingsDic { get; set; }
    }
}
