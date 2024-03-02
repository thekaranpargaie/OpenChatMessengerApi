using System;
using System.Collections.Generic;
using System.Text;

namespace Base.Domain
{
    public interface IBusinessRule
    {
        bool IsBroken();

        string Message { get; }
    }
}
