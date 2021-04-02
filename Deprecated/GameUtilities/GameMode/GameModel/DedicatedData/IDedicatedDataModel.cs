using System;
using System.Collections.Generic;
using System.Text;

namespace GameUtilities.GameMode.DedicatedData
{
    public interface IDedicatedDataModel<TData>
    {
        public TData Data { get; }
    }
}
