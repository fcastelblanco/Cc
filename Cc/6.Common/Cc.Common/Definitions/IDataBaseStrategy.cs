using System;
using System.Collections.Generic;

namespace Cc.Common.Definitions
{
    public interface IDataBaseStrategy
    {
        bool ExecuteQuery(string query, IDictionary<string, object> queryParameters, string connectionString, out Exception exception);

        List<T> ExcecuteSelect<T>(string query, IDictionary<string, object> queryParameters, string connectionString, out Exception exception);
    }
}
