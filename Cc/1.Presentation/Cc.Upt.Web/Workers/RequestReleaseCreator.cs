using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Mvc;
using Cc.Upt.Business.Definitions;
using Cc.Upt.Common.LogHelper;

namespace Cc.Upt.Web.Workers
{
    public class RequestReleaseCreator : IDisposable, IRegisteredObject
    {
        private readonly TimeSpan _timeSpan;
        private readonly IDownloadRequestReleaseService _downloadRequestReleaseService;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isRunning;

        public RequestReleaseCreator(TimeSpan interval)
        {
            _downloadRequestReleaseService = DependencyResolver.Current.GetService<IDownloadRequestReleaseService>();
            _timeSpan = interval;
            _isRunning = true;            
            _cancellationTokenSource = new CancellationTokenSource();            
            Task.Run(() => TaskLoop(), _cancellationTokenSource.Token);
        }

        public void Dispose()
        {
            _isRunning = false;

            if (_cancellationTokenSource == null) return;

            try
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex);
            }
            finally
            {
                _cancellationTokenSource = null;
            }
        }

        public void Stop(bool immediate)
        {
            try
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex);
            }
            finally
            {
                _cancellationTokenSource = null;
            }

            HostingEnvironment.UnregisterObject(this);
        }

        private void TaskLoop()
        {
            while (_isRunning)
            {
                try
                {
                    _downloadRequestReleaseService.ExecuteRequestReleaseCreator();
                    Thread.Sleep(_timeSpan);
                }
                catch (Exception ex)
                {
                    Log.Instance.Error(ex);
                }
            }
        }
    }
}