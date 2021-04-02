using GameSettingUtils;
using GameUtilities.GameMode.DedicatedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameUtilities.GameMode.GameModel.DedicatedData
{
    public abstract class DedicatedDataModel<TData> : GameModel, IDedicatedDataModel<TData>
    {
        private TData _data;

        protected DedicatedDataModel(string name, int minPlayer, int maxPlayer) : base(name, minPlayer, maxPlayer)
        {
        }

        public TData Data => _data;

        public override void Reset(IReadOnlyDictionary<string, IGameSetting> settings)
        {
            ResetData(settings, ref _data);
        }

        protected abstract void ResetData(IReadOnlyDictionary<string, IGameSetting> settings, ref TData data);
    }
}
