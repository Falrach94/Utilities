using GameManagement;
using GameUtilities.GameMode;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameUtilities.ServerBackend.Implementations
{
    public class ModelBasedServerGameMode<TBackend, TModel> : IServerGameMode
        where TBackend : ModelBasedServerBackend<TModel>, new()
        where TModel : IGameModel, new()
    {
        public TBackend Backend { get; } = new TBackend();
        public TModel Model => Backend.Model;


        IGameModel IServerGameMode.Model => Model;
        IServerGameBackend IServerGameMode.Backend => Backend;

    }
}
