using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Threading;
using Cc.Common.Enumerations;
using Cc.Common.ExtensionMethods;
using Cc.Ioc;
using Cc.Upt.Business.Definitions;
using Cc.Upt.Business.Implementations;
using Cc.Upt.Business.Implementations.Singleton;
using Cc.Upt.Domain;
using Cc.Upt.Domain.Dto;
using Cc.Upt.Domain.Enumerations;

namespace Cc.Upt.Configurator
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private readonly IGenericClientService _genericClientService;
        private CompanyMapper _companyMapper;
        private ObservableCollection<CompanyMapperDto> _companyMapperDtos;
        private bool _isOffLine;
        private bool _isOnLine;

        private ItemDto _itemDto;
        private ObservableCollection<ItemDto> _itemDtos;
        private Timer _timerConfigurator;

        public MainWindow()
        {
            _genericClientService = IsnContainer.Resolve<IGenericClientService>();
            _companyMapperDtos = new ObservableCollection<CompanyMapperDto>();
            InitializeComponent();
        }

        public ObservableCollection<ItemDto> ItemDtos
        {
            get => _itemDtos;
            set
            {
                _itemDtos = value;
                OnPropertyChanged(nameof(ItemDtos));
            }
        }

        public bool IsOnline
        {
            get => _isOnLine;
            set
            {
                _isOnLine = value;
                if (_isOnLine)
                {
                    IsOffLine = false;
                    ChangeMode();
                }

                OnPropertyChanged(nameof(_isOnLine));
            }
        }

        public bool IsOffLine
        {
            get => _isOffLine;
            set
            {
                _isOffLine = value;
                if (_isOffLine)
                {
                    IsOnline = false;
                    ChangeMode();
                }

                OnPropertyChanged(nameof(_isOffLine));
            }
        }

        public ItemDto ItemDto
        {
            get => _itemDto;
            set
            {
                _itemDto = value;
                ChangeMode();
                OnPropertyChanged(nameof(ItemDto));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Error.Content = string.Empty;
            DataContext = this;

            ItemDtos = new ObservableCollection<ItemDto>(
                Enum.GetValues(typeof(UpdateMode)).Cast<UpdateMode>().Select(x => new ItemDto
                {
                    Id = (int)x,
                    Description = x.GetDescription()
                }).ToList());

            _timerConfigurator = new Timer
            {
                Interval = 2000
            };
            _timerConfigurator.Tick += TimerConfiguratorOnTick;
            _timerConfigurator.Start();
        }

        private void TimerConfiguratorOnTick(object sender, EventArgs eventArgs)
        {
            var existConfig = ValidateConfiguration(false);
            _timerConfigurator.Stop();

            NoExisteConfiguracion.Visibility = existConfig ? Visibility.Hidden : Visibility.Visible;
            ExisteConfiguracion.Visibility = existConfig ? Visibility.Visible : Visibility.Hidden;
        }

        private bool ValidateConfiguration(bool reload)
        {
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\Ipm service\IpmConfigurator.xml"))
                return false;

            var dataRetrievedConfigurator = XmlExtension.GetDataFromXml<IpmConfiguratorDto>(
                AppDomain.CurrentDomain.BaseDirectory +
                @"\Ipm service\IpmConfigurator.xml");

            var dataRetrievedCompanyMapper = XmlExtension.GetDataFromXml<ObservableCollection<CompanyMapperDto>>(
                AppDomain.CurrentDomain.BaseDirectory +
                @"\Ipm service\CompanyMapper.xml");

            if (dataRetrievedConfigurator == null || dataRetrievedCompanyMapper == null)
                return false;

            if (!reload) return true;

            ReleaseLocation.Text = dataRetrievedConfigurator.ReleasePath;
            IntervalExecution.Text = dataRetrievedConfigurator.IntervalExecution.ToString();
            ReleaseLocation.Text = dataRetrievedConfigurator.ReleasePath;
            UserNameFtp.Text = dataRetrievedConfigurator.FtpUserName;
            PasswordUserFtp.Password = dataRetrievedConfigurator.FtpPassword;
            FtpPath.Text = dataRetrievedConfigurator.FtpPath;
            UserName.Text = dataRetrievedConfigurator.UserName;
            UserPassword.Password = dataRetrievedConfigurator.Password;
            IsOnline = dataRetrievedConfigurator.UpdateMode == UpdateMode.OnLine;
            IsOffLine = dataRetrievedConfigurator.UpdateMode == UpdateMode.OffLine;
            ItemDto = ItemDtos.FirstOrDefault(x => x.Id == (int)dataRetrievedConfigurator.UpdateMode);
            _companyMapperDtos = dataRetrievedCompanyMapper;

            return true;
        }

        private void ChangeMode()
        {
            Error.Content = string.Empty;
            ModeOffLine.Visibility = _isOffLine ? Visibility.Visible : Visibility.Hidden;
            ModeOnLine.Visibility = !_isOffLine ? Visibility.Visible : Visibility.Hidden;

            if (!_isOnLine) return;
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\Ipm service\IpmConfigurator.xml"))
                return;

            var dataRetrievedConfigurator = XmlExtension.GetDataFromXml<IpmConfiguratorDto>(
                AppDomain.CurrentDomain.BaseDirectory + @"\Ipm service\IpmConfigurator.xml");
            
            UserName.Text = dataRetrievedConfigurator.UserName;
            UserPassword.Password = dataRetrievedConfigurator.Password;
        }

        private void StartProcessModeOffLine_Click(object sender, RoutedEventArgs e)
        {
            StartProcess();
        }

        private void StartProcessModeOnLine_Click(object sender, RoutedEventArgs e)
        {
            StartProcess();
        }

        private void StartProcess()
        {
            Error.Content = string.Empty;
            if (IsOffLine)
                OffLineMode();
            else
                OnLineMode();
            //switch ((UpdateMode)ItemDto.Id)
            //{
            //    case UpdateMode.OffLine:
            //        OffLineMode();
            //        break;
            //    case UpdateMode.OnLine:
            //        OnLineMode();
            //        break;
            //    default:
            //        throw new ArgumentOutOfRangeException();
            //}
        }

        private void ValidateIntervalExecution()
        {
            Error.Content = string.Empty;

            if (string.IsNullOrEmpty(IntervalExecution.Text))
            {
                Error.Content = "Ingrese el periodo de ejecución del servicio";
                return;
            }

            if (!int.TryParse(IntervalExecution.Text, out _))
            {
                Error.Content = "El periodo de ejecución del servicio debe ser un numero entero";
                return;
            }

            if (Convert.ToInt32(IntervalExecution.Text) < 0)
            {
                Error.Content = "El periodo de ejecución del servicio debe ser un numero mayor a cero";
                return;
            }

            if (_companyMapperDtos.Count == 0)
            {
                Error.Content = "Es necesario mapear empresas";
            }
        }

        private void OffLineMode()
        {
            if (string.IsNullOrEmpty(ReleaseLocation.Text))
            {
                Error.Content = "Seleccione la ubicación de las actualizaciones que descargue de IPM";
                return;
            }

            ValidateIntervalExecution();
            SaveConfiguration();
        }

        private void OnLineMode()
        {
            ValidateIntervalExecution();
            SaveConfiguration();
        }

        private void SaveConfiguration()
        {
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\Ipm service"))
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\Ipm service");

            var ipmConfiguratorDto = new IpmConfiguratorDto
            {
                UpdateMode = IsOffLine ? UpdateMode.OffLine : UpdateMode.OnLine,
                ReleasePath = IsOffLine
                    ? ReleaseLocation.Text
                    : AppDomain.CurrentDomain.BaseDirectory + @"Ipm service\Releases\",
                IntervalExecution = Convert.ToInt32(IntervalExecution.Text),
                ApiUrl = ApiUrl.Text,
                FtpPassword = IsOffLine ? string.Empty : ConfigurationUpdaterManager.Instance.FtpUserPassword,
                FtpPath = IsOffLine ? string.Empty : ConfigurationUpdaterManager.Instance.FtpPath,
                FtpUserName = IsOffLine ? string.Empty : ConfigurationUpdaterManager.Instance.FtpUserName,
                Password = UserPassword.Password,
                UserName = UserName.Text,
                CompanyId = IsOffLine ? Guid.Empty : ConfigurationUpdaterManager.Instance.CompanyId,
                BufferSize = IsOffLine ? 0 : ConfigurationUpdaterManager.Instance.BufferSize
            };

            ipmConfiguratorDto.SaveClassAsXml(AppDomain.CurrentDomain.BaseDirectory +
                                              @"\Ipm service\IpmConfigurator.xml");
            _companyMapperDtos.SaveClassAsXml(AppDomain.CurrentDomain.BaseDirectory +
                                              @"\Ipm service\CompanyMapper.xml");

            WindowsExtension.StartService(UpdaterService.UpdaterServiceName);
            var startInfo = new ProcessStartInfo
            {
                FileName = AppDomain.CurrentDomain.BaseDirectory +
                           @"\Ipm notificator\Isn.Upt.Presentation.Notificator.exe"
            };
            Process.Start(startInfo);

            Close();
        }

        private void AutenticateUser_OnClick(object sender, RoutedEventArgs e)
        {
            Error.Content = string.Empty;

            if (string.IsNullOrEmpty(UserName.Text))
            {
                Error.Content = "Ingrese su nombre de usuario";
                return;
            }

            if (string.IsNullOrEmpty(UserPassword.Password))
            {
                Error.Content = "Ingrese la clave de usuario";
                return;
            }

            BusyIndicator.IsBusy = true;
            var processOk = false;
            Task.Factory.StartNew(() =>
                {
                    ExecuteActionByDispatcher(() =>
                    {
                        ConfigurationUpdaterManager.Instance = new ConfigurationUpdaterManager
                        {
                            ApiUrl = ApiUrl.Text,
                            UserName = UserName.Text,
                            Password = UserPassword.Password
                        };
                    });

                    Dispatcher.Invoke(DispatcherPriority.Normal,
                        new Action(() => { BusyIndicator.BusyContent = "Autenticando usuario ..."; }));

                    var currentUserData = _genericClientService.Get<User>("api/UserApi/GetCurrentUserData");

                    if (currentUserData.Data == null)
                    {
                        SetContentControlMessage(Error,
                            "Imposible obtener los datos del usuario. " + currentUserData.Exception);
                        return;
                    }

                    SetContentControlMessage(UserLoggedIn,
                        currentUserData.Data.Name + " " + currentUserData.Data.LastName);
                    ConfigurationUpdaterManager.Instance.CompanyId = currentUserData.Data.CompanyId;

                    Dispatcher.Invoke(DispatcherPriority.Normal,
                        new Action(() => { BusyIndicator.BusyContent = "Obteniendo parámetros ..."; }));

                    var dataParameters =
                        _genericClientService.Get<List<Parameter>>("api/ParameterApi/GetAllParameterList",
                            new Dictionary<string, string>
                            {
                                {"CompanyId", ConfigurationUpdaterManager.Instance.CompanyId.ToString().Base64Encode()}
                            });

                    if (dataParameters.Data == null)
                    {
                        SetContentControlMessage(Error,
                            "Imposible obtener los parámetros. " + currentUserData.Exception);
                        return;
                    }

                    var ftpUserName = dataParameters.Data.FirstOrDefault(x =>
                        x.ParameterInternalIdentificator == ParameterInternalIdentificator.FtpUser);
                    if (ftpUserName != null && !string.IsNullOrEmpty(ftpUserName.Value))
                        ConfigurationUpdaterManager.Instance.FtpUserName = ftpUserName.Value;

                    var ftpPassword = dataParameters.Data.FirstOrDefault(x =>
                        x.ParameterInternalIdentificator == ParameterInternalIdentificator.FtpPassword);
                    if (ftpPassword != null && !string.IsNullOrEmpty(ftpPassword.Value))
                        ConfigurationUpdaterManager.Instance.FtpUserPassword = ftpPassword.Value;

                    var ftpPath = dataParameters.Data.FirstOrDefault(x =>
                        x.ParameterInternalIdentificator == ParameterInternalIdentificator.FtpPath);
                    if (ftpPath != null && !string.IsNullOrEmpty(ftpPath.Value))
                        ConfigurationUpdaterManager.Instance.FtpPath = ftpPath.Value;

                    switch (currentUserData.Data.Profile)
                    {
                        case Profile.CompanyContact:
                        case Profile.Administrator:
                            ExecuteActionByDispatcher(() =>
                            {
                                var dataBufferSize = dataParameters.Data.FirstOrDefault(
                                    x => x.ParameterInternalIdentificator == ParameterInternalIdentificator.BufferSize);

                                if (dataBufferSize != null)
                                    if (!string.IsNullOrEmpty(dataBufferSize.Value))
                                        if (int.TryParse(dataBufferSize.Value, out var realBufferSizeValue))
                                            ConfigurationUpdaterManager.Instance.BufferSize = realBufferSizeValue;
                            });

                            processOk = true;
                            break;
                        default:
                            SetContentControlMessage(Error,
                                $"No es posible ingresar con el perfil {currentUserData.Data.Profile}");
                            break;
                    }
                })
                .ContinueWith(task =>
                    {
                        BusyIndicator.IsBusy = false;
                        if (processOk)
                            OpenCompanyMapper();
                    },
                    TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void SetContentControlMessage(ContentControl label, string message)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal,
                new Action(() => { label.Content = message; }));
        }

        private void ExecuteActionByDispatcher(Action action)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal,
                new Action(action));
        }

        private void MapCompaniesModeOffLine_OnClick(object sender, RoutedEventArgs e)
        {
            OpenCompanyMapper();
        }

        private void CompanyMapperOnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            _companyMapperDtos = _companyMapper.CompanyMappers;
        }

        private void MapCompaniesModeOnLine_OnClick(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal,
                new Action(() => { BusyIndicator.BusyContent = "Obteniendo información de la empresa ..."; }));
            var currentUserData = _genericClientService.Get<User>("api/UserApi/GetCurrentUserData");
            var dataCompanyUser = _genericClientService.Get<Company>(
                "api/CompanyApi/GetCompany/" + currentUserData.Data.CompanyId,
                new Dictionary<string, string>
                {
                    {
                        "CompanyId",
                        ConfigurationUpdaterManager.Instance.CompanyId.ToString().Base64Encode()
                    }
                });
            if (dataCompanyUser.Data.DateEndSupport.Date >= DateTime.Now.Date)
            {
                if (!_companyMapperDtos.Any())
                    ExecuteActionByDispatcher(() =>
                    {
                        _companyMapperDtos.Add(new CompanyMapperDto
                        {
                            Name = dataCompanyUser.Data.Name,
                            Id = Guid.NewGuid()
                        });
                    });
                OpenCompanyMapper();
            }
            else
            {
                SetContentControlMessage(Error,
                    $"La garantía de soporte para la empresa {dataCompanyUser.Data.Name} se encuentra vencida");
            }
        }

        private void OpenCompanyMapper()
        {
            _companyMapper = new CompanyMapper();
            if (_companyMapperDtos.Count > 0)
                _companyMapper.CompanyMappers = _companyMapperDtos;
            _companyMapper.Closing += CompanyMapperOnClosing;
            var currentData = _companyMapperDtos.FirstOrDefault();
            _companyMapper.ChargeCompanyMapper(currentData != null ? currentData.LicencePath : string.Empty);
            _companyMapper.ShowDialog();
        }

        private void ReleaseLocationButton_OnClick(object sender, RoutedEventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog
            {
                Description = @"Seleccione la unidad"
            };

            var theFolderBorwserDialog = folderBrowserDialog.ShowDialog();

            if (theFolderBorwserDialog != System.Windows.Forms.DialogResult.OK) return;

            ReleaseLocation.Text = folderBrowserDialog.SelectedPath;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void YesReload_Click(object sender, RoutedEventArgs e)
        {
            ValidateConfiguration(true);
            NoExisteConfiguracion.Visibility = Visibility.Visible;
            ExisteConfiguracion.Visibility = Visibility.Hidden;
        }

        private void NoReload_Click(object sender, RoutedEventArgs e)
        {
            NoExisteConfiguracion.Visibility = Visibility.Visible;
            ExisteConfiguracion.Visibility = Visibility.Hidden;
        }
    }
}