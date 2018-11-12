using System;
using System.Collections.Generic;
using Cc.Common.Definitions;

namespace Cc.Common.Implementations.DataBaseHelper
{
    public class StrategyToApply
    {
        private readonly IDataBaseStrategy _dataBaseStrategy;

        public StrategyToApply(IDataBaseStrategy dataBaseStrategy)
        {
            _dataBaseStrategy = dataBaseStrategy;
        }

        public bool ExcecuteQuery(string query, IDictionary<string, object> queryParameters, string connectionString, out Exception exception)
        {
            return _dataBaseStrategy.ExecuteQuery(query, queryParameters, connectionString, out exception);
        }

        public List<T> ExcecuteSelect<T>(string query, IDictionary<string, object> queryParameters,
            string connectionString, out Exception exception)
        {
            return _dataBaseStrategy.ExcecuteSelect<T>(query, queryParameters, connectionString, out exception);
        }
    }
}
