using Isn.Upt.Domain.Dto;
using Isn.Upt.Domain.Enumerations;

namespace Isn.Upt.Business.Definitions
{
    public interface IDataBaseProviderService
    {
        string GetParameterValue(IsoluctionParameterDto isoluctionParameterDto, DataBaseProvider baseProvider, string connectionString, out ParameterStatus exists);

        bool ExecuteCommand(IsoluctionParameterDto isoluctionParameterDto, DataBaseProvider baseProvider, string connectionString, SqlTask sqlTask);
    }
}