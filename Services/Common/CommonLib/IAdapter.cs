using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLib
{
    public interface IAdapter<IInput, TOutput>
    {
        TOutput Transform(IInput model);
    }
}
