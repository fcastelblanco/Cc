using Cc.Upt.Domain.DataTransferObject;

using Cc.Upt.Domain.Enumerations;

namespace Cc.Upt.Business.Definitions
{
    public interface IDataBaseProviderService
    {
        string GetParameterValue(IsoluctionParameterDto isoluctionParameterDto, DataBaseProvider baseProvider, string connectionString, out ParameterStatus exists);

        bool ExecuteCommand(IsoluctionParameterDto isoluctionParameterDto, DataBaseProvider baseProvider, string connectionString, SqlTask sqlTask);
    }
}