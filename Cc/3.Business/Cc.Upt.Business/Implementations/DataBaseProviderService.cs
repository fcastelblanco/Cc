using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Cc.Upt.Business.Definitions;
using Cc.Upt.Common.LogHelper;
using Cc.Upt.Domain.DataTransferObject;

using Cc.Upt.Domain.Enumerations;
using Oracle.ManagedDataAccess.Client;

namespace Cc.Upt.Business.Implementations
{
    public class DataBaseProviderService : IDataBaseProviderService
    {
        public string GetParameterValue(IsoluctionParameterDto isoluctionParameterDto, DataBaseProvider baseProvider,
            string connectionString, out ParameterStatus exists)
        {
            IDbConnection dbConnection = null;
            exists = ParameterStatus.DoesNotExist;

            try
            {
                var theSelect = "select " + isoluctionParameterDto.ColumnOrField + " from " +
                                isoluctionParameterDto.Table +
                                (string.IsNullOrEmpty(isoluctionParameterDto.Where)
                                    ? ""
                                    : " where " + isoluctionParameterDto.Where);

                DbCommand dbCommand;
                switch (baseProvider)
                {
                    case DataBaseProvider.Oracle:
                        dbConnection = new OracleConnection(connectionString);
                        dbConnection.Open();
                        dbCommand = new OracleCommand(theSelect,
                            (OracleConnection) dbConnection);
                        break;
                    case DataBaseProvider.SqlServer:
                        dbConnection = new SqlConnection(connectionString);
                        dbConnection.Open();
                        dbCommand = new SqlCommand(theSelect, (SqlConnection) dbConnection);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(baseProvider), baseProvider, null);
                }

                var dataTable = new DataTable();
                dataTable.Load(dbCommand.ExecuteReader());

                foreach (DataRow row in dataTable.Rows)
                {
                    if (DBNull.Value.Equals(row[isoluctionParameterDto.ColumnOrField]))
                    {
                        exists = ParameterStatus.NullOrEmpty;
                        return string.Empty;
                    }

                    if (row[isoluctionParameterDto.ColumnOrField].ToString() == " ")
                    {
                        exists = ParameterStatus.NullOrEmpty;
                        return string.Empty;
                    }

                    if (string.IsNullOrEmpty(row[isoluctionParameterDto.ColumnOrField].ToString()))
                    {
                        exists = ParameterStatus.NullOrEmpty;
                        return string.Empty;
                    }

                    exists = ParameterStatus.Exist;
                    return row[isoluctionParameterDto.ColumnOrField].ToString();
                }

                exists = ParameterStatus.DoesNotExist;
                return string.Empty;
            }
            catch (Exception e)
            {
                exists = ParameterStatus.DoesNotExist;
                return e.ToString();
            }
            finally
            {
                if (dbConnection != null && dbConnection.State == ConnectionState.Open)
                    dbConnection.Close();
            }
        }

        public bool ExecuteCommand(IsoluctionParameterDto isoluctionParameterDto, DataBaseProvider baseProvider,
            string connectionString, SqlTask sqlTask)
        {
            IDbConnection dbConnection = null;

            if (string.IsNullOrEmpty(isoluctionParameterDto.ScriptCreateColumnOracle)
                ||
                string.IsNullOrEmpty(isoluctionParameterDto.ScriptCreateColumnSql)
                ||
                string.IsNullOrEmpty(isoluctionParameterDto.ScriptInsertOracle)
                ||
                string.IsNullOrEmpty(isoluctionParameterDto.ScriptInsertSql))
                return false;

            try
            {
                var theCommand = sqlTask == SqlTask.CreateColumn
                    ? baseProvider == DataBaseProvider.Oracle
                        ? isoluctionParameterDto.ScriptCreateColumnOracle
                        : isoluctionParameterDto.ScriptCreateColumnSql
                    : baseProvider == DataBaseProvider.Oracle
                        ? isoluctionParameterDto.ScriptInsertOracle
                        : isoluctionParameterDto.ScriptInsertSql;

                DbCommand dbCommand;
                switch (baseProvider)
                {
                    case DataBaseProvider.Oracle:
                        dbConnection = new OracleConnection(connectionString);
                        dbConnection.Open();
                        dbCommand = new OracleCommand(theCommand,
                            (OracleConnection) dbConnection);
                        break;
                    case DataBaseProvider.SqlServer:
                        dbConnection = new SqlConnection(connectionString);
                        dbConnection.Open();
                        dbCommand = new SqlCommand(theCommand, (SqlConnection) dbConnection);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(baseProvider), baseProvider, null);
                }

                return Convert.ToInt32(dbCommand.ExecuteScalar()) > 0;
            }
            catch (Exception e)
            {
                Log.Instance.Error(e);
                throw;
            }
            finally
            {
                if (dbConnection != null && dbConnection.State == ConnectionState.Open)
                    dbConnection.Close();
            }
        }
    }
}