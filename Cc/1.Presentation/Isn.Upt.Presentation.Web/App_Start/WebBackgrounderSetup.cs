using Isn.Upt.Business.Definitions;
using Isn.Upt.Presentation;
using Isn.Upt.Presentation.Workers;
using System;
using System.Web.Mvc;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(WebBackgrounderSetup), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(WebBackgrounderSetup), "Shutdown")]

namespace Isn.Upt.Presentation
{
    public static class WebBackgrounderSetup
    {
        private static RequestReleaseCreator _requestReleaseCreator;

        public static void Start()
        {
            var parameterService = DependencyResolver.Current.GetService<IParameterService>();
            var parameterValue = parameterService.GetParameterValueByInternalIdentificator<int>(Domain.Enumerations.ParameterInternalIdentificator.IntervalExecutionInternalProcess);
            _requestReleaseCreator = new RequestReleaseCreator(parameterValue == 0? TimeSpan.FromMinutes(30) : TimeSpan.FromMinutes(parameterValue));
        }

        public static void Shutdown()
        {
            _requestReleaseCreator.Dispose();
        }
    }
}
