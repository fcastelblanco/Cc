using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using Cc.Upt.Common.LogHelper;

namespace Cc.Upt.Web.ExceptionHandling
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