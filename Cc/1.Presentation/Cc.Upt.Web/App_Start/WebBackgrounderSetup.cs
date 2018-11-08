using System;
using System.Web.Mvc;
using Cc.Upt.Web;
using Cc.Upt.Web.Workers;
using Isn.Upt.Business.Definitions;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(WebBackgrounderSetup), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(WebBackgrounderSetup), "Shutdown")]

namespace Cc.Upt.Web
{
    public static class WebBackgrounderSetup
    {
        private static RequestReleaseCreator _requestReleaseCreator;

        public static void Start()
        {
            var parameterService = DependencyResolver.Current.GetService<IParameterService>();
            var parameterValue = parameterService.GetParameterValueByInternalIdentificator<int>(Isn.Upt.Domain.Enumerations.ParameterInternalIdentificator.IntervalExecutionInternalProcess);
            _requestReleaseCreator = new RequestReleaseCreator(parameterValue == 0? TimeSpan.FromMinutes(30) : TimeSpan.FromMinutes(parameterValue));
        }

        public static void Shutdown()
        {
            _requestReleaseCreator.Dispose();
        }
    }
}
