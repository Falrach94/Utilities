using System;
using System.Collections.Generic;
using System.Text;

namespace GameUtilities.ServerBackend.Implementations
{
    public abstract class ModelBasedServerBackend<TModel> : SimpleMessageServerBackend, IModelBasedServerBackend<TModel>
        where TModel : new()
    {
        public TModel Model { get; } = new TModel();

    }
}
