using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using Isn.Common.LogHelper;

namespace Isn.Upt.Presentation.ExceptionHandling
{
    public class IsolucionExceptionFilterAttribute : IExceptionFilter
    {
        public bool AllowMultiple { get; }
        public Task ExecuteExceptionFilterAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                Log.Instance.Error(actionExecutedContext.Exception, "Error");
                
            }, cancellationToken);
        }
    }
}