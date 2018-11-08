using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq.Expressions;
using Isn.Common.Definitions;
using Isn.Common.LogHelper;

namespace Isn.Common.Implementations.DataBaseHelper
{
    public class SqlServerManagement : IDataBaseStrategy
    {
        public bool ExecuteQuery(string query, IDictionary<string, object> queryParameters, string connectionString,
            out Exception exception)
        {
            exception = null;
            var excecutedCorrectly = false;
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    if (queryParameters != null)
                        foreach (var parameter in queryParameters)
                            command.Parameters.AddWithValue($"{parameter.Key}", parameter.Value);
                    Console.WriteLine(command.CommandText);
                    try
                    {
                        if (command.ExecuteNonQuery() > 0) excecutedCorrectly = true;
                    }
                    catch (SqlException ex)
                    {
                        Log.Instance.Error(ex.Message);
                        exception = ex;
                    }
                }
            }

            return excecutedCorrectly;
        }

        public List<T> ExcecuteSelect<T>(string query, IDictionary<string, object> queryParameters,
            string connectionString, out Exception exception)
        {
            exception = null;
            var dataRetrieved = new List<T>();
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    if (queryParameters != null)
                        foreach (var parameter in queryParameters)
                            command.Parameters.AddWithValue($"{parameter.Key}", parameter.Value);
                    try
                    {
                        var reader = command.ExecuteReader();
                        dataRetrieved = CreateList<T>(reader);
                    }
                    catch (SqlException ex)
                    {
                        Log.Instance.Error(ex.Message);
                        exception = ex;
                    }
                }
            }

            return dataRetrieved;
        }

        public virtual List<T> CreateList<T>(DbDataReader reader)
        {
            var results = new List<T>();
            var readRow = GetReader<T>(reader);

            while (reader.Read())
                results.Add(readRow(reader));

            return results;
        }

        private static Func<DbDataReader, T> GetReader<T>(IDataRecord reader)
        {
            var readerColumns = new List<string>();
            for (var index = 0; index < reader.FieldCount; index++)
                readerColumns.Add(reader.GetName(index));

            // determine the information about the reader
            var readerParam = Expression.Parameter(typeof(DbDataReader), "reader");
            var readerGetValue = typeof(DbDataReader).GetMethod("GetValue");

            // create a Constant expression of DBNull.Value to compare values to in reader
            var dbNullValue = typeof(DBNull).GetField("Value");
            var dbNullExp = Expression.Field(null, typeof(DBNull), "Value");

            // loop through the properties and create MemberBinding expressions for each property
            var memberBindings = new List<MemberBinding>();
            foreach (var prop in typeof(T).GetProperties())
            {
                // determine the default value of the property
                object defaultValue = null;
                if (prop.PropertyType.IsValueType)
                    defaultValue = Activator.CreateInstance(prop.PropertyType);
                else if (prop.PropertyType.Name.ToLower().Equals("string"))
                    defaultValue = string.Empty;

                if (readerColumns.Contains(prop.Name))
                {
                    // build the Call expression to retrieve the data value from the reader
                    var indexExpression = Expression.Constant(reader.GetOrdinal(prop.Name));
                    var getValueExp = Expression.Call(readerParam, readerGetValue, indexExpression);

                    // create the conditional expression to make sure the reader value != DBNull.Value
                    var testExp = Expression.NotEqual(dbNullExp, getValueExp);
                    var ifTrue = Expression.Convert(getValueExp, prop.PropertyType);
                    var ifFalse = Expression.Convert(Expression.Constant(defaultValue), prop.PropertyType);

                    // create the actual Bind expression to bind the value from the reader to the property value
                    var mi = typeof(T).GetMember(prop.Name)[0];
                    MemberBinding mb = Expression.Bind(mi, Expression.Condition(testExp, ifTrue, ifFalse));
                    memberBindings.Add(mb);
                }
            }

            // create a MemberInit expression for the item with the member bindings
            var newItem = Expression.New(typeof(T));
            var memberInit = Expression.MemberInit(newItem, memberBindings);


            var lambda = Expression.Lambda<Func<DbDataReader, T>>(memberInit, readerParam);
            Delegate resDelegate = lambda.Compile();

            return (Func<DbDataReader, T>) resDelegate;
        }
    }
}