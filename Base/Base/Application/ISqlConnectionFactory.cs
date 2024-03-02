using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Application
{
    public interface ISqlConnectionFactory
    {
        IDbConnection GetOpenConnection();
    }
}
