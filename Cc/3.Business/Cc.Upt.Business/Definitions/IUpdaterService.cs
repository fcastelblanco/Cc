namespace Cc.Upt.Business.Definitions
{
    public interface IUpdaterService
    {
        void Execute();
        T GetConfigurationFromXml<T>( string path); 
    }
}
