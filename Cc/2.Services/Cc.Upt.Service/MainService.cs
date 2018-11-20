using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using Cc.Upt.Business.Definitions;
using Cc.Upt.Common.LogHelper;
using Cc.Upt.Domain.DataTransferObject;
using Cc.Upt.Ioc;


namespace Cc.Upt.Service
{
    public partial class MainService : ServiceBase
    {
        private readonly CancellationToken _cancellationToken;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly IUpdaterService _updaterService;
        public readonly List<ManualResetEvent> ResetEvents;

        private IpmConfiguratorDto _ipmConfiguratorDto;

        public MainService()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            ResetEvents = new List<ManualResetEvent>();
            _updaterService = CcContainer.Resolve<IUpdaterService>();
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Log.Instance.Info("Service started");
            StartService();
        }

        public void StartService()
        {
            //Log.Instance.Info("Tarea de actualización ejecutandose");
            //_updaterService.Execute();
            //Log.Instance.Info("Tarea de actualización ejecutada");
            //Thread.Sleep(_ipmConfiguratorDto.IntervalExecution * 1000);

            var resetEvent = new ManualResetEvent(false);
            ResetEvents.Add(resetEvent);
            StartWorker(resetEvent);
        }

        public void StartWorker(ManualResetEvent resetEvent)
        {
            Task.Run(() =>
            {
                try
                {
                    while (!_cancellationToken.IsCancellationRequested)
                    {
                        _ipmConfiguratorDto =
                            _updaterService.GetConfigurationFromXml<IpmConfiguratorDto>(
                                AppDomain.CurrentDomain.BaseDirectory + @"\IpmConfigurator.xml");
                        Log.Instance.Info("Tarea de actualización ejecutandose");
                        _updaterService.Execute();
                        Log.Instance.Info("Tarea de actualización ejecutada");
                        Thread.Sleep(TimeSpan.FromMinutes(_ipmConfiguratorDto.IntervalExecution));
                    }
                }
                catch (Exception ex)
                {
                    Log.Instance.Error(ex);
                }

                resetEvent.Set();
            }, _cancellationToken);
        }

        protected override void OnStop()
        {
            _cancellationTokenSource.Cancel();
            WaitHandle.WaitAll(ResetEvents.Select(e => e as WaitHandle).ToArray(), 3000);
            Log.Instance.Info("Windows service stoped");
        }
    }
}