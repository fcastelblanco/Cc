using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Threading;
using Cc.Common.LogHelper;
using Cc.Ioc;
using Cc.Upt.Business.Definitions;
using Cc.Upt.Domain.Dto;
using Cc.Upt.Domain.Enumerations;

using Newtonsoft.Json;
using Button = System.Windows.Controls.Button;
using MessageBox = System.Windows.MessageBox;

namespace Cc.Upt.Configurator
{
    public partial class CompanyMapper : INotifyPropertyChanged
    {
        private readonly IDataBaseProviderService _dataBaseProviderService;
        private readonly IParameterService _parameterService;
        private ObservableCollection<CompanyMapperDto> _companyMappers;
        private CompanyMapperDto _theCompanyMapperDto;
        private Dictionary<ParameterIsolucion, ParameterStatus> _dataParametersNotExisting;
        private DataBaseProvider _dataBaseProvider;
        private string _connectionString;

        public CompanyMapper()
        {
            InitializeComponent();
            _parameterService = IsnContainer.Resolve<IParameterService>();
            _dataBaseProviderService = IsnContainer.Resolve<IDataBaseProviderService>();
        }

        public ObservableCollection<CompanyMapperDto> CompanyMappers
        {
            get { return _companyMappers; }
            set
            {
                _companyMappers = value;
                OnPropertyChanged(nameof(CompanyMappers));
            }
        }

        public CompanyMapperDto TheCompanyMapperDto
        {
            get { return _theCompanyMapperDto; }
            set
            {
                _theCompanyMapperDto = value;
                OnPropertyChanged(nameof(TheCompanyMapperDto));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void CompanyMapper_OnLoaded(object sender, RoutedEventArgs e)
        {
            FileError.Content = string.Empty;
            if (CompanyMappers == null)
                CompanyMappers = new ObservableCollection<CompanyMapperDto>();

            DataContext = this;
            TheCompanyMapperDto = new CompanyMapperDto();
        }

        private void AddCompany_OnClick(object sender, RoutedEventArgs e)
        {
            Error.Content = string.Empty;

            var folderBrowserDialog = new FolderBrowserDialog
            {
                Description = @"Seleccione la ruta de la licencia"
            };

            var theFolderBrowserDialog = folderBrowserDialog.ShowDialog();

            if (theFolderBrowserDialog != System.Windows.Forms.DialogResult.OK) return;

            ChargeCompanyMapper(folderBrowserDialog.SelectedPath);
        }

        public void ChargeCompanyMapper(string selectedPath)
        {
            List<IsoluctionParameterDto> dataParametersRetrieved;

            using (var streamReader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"\LicenseParametersRequired.json"))
            {
                var jsonLicenseParameters = streamReader.ReadToEnd();
                dataParametersRetrieved =
                    JsonConvert.DeserializeObject<List<IsoluctionParameterDto>>(jsonLicenseParameters);
            }
            try
            {
                var dataRetrieved = _parameterService.GetIsolucionParameterValues(dataParametersRetrieved, selectedPath);
                _dataBaseProvider =
                dataRetrieved.FirstOrDefault(x => x.Key == LicenseParameter.DataBaseEngine).Value.ToUpper() == "ORACLE"
                    ? DataBaseProvider.Oracle
                    : DataBaseProvider.SqlServer;
                _connectionString = dataRetrieved.FirstOrDefault(
                    x => x.Key == LicenseParameter.ConnectionStringSf).Value;

                using (var streamReader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"\IsolucionParameter.json"))
                {
                    var jsonLicenseParameters = streamReader.ReadToEnd();
                    dataParametersRetrieved =
                        JsonConvert.DeserializeObject<List<IsoluctionParameterDto>>(jsonLicenseParameters);
                }

                var dataCompany = new Dictionary<ParameterIsolucion, string>();
                _dataParametersNotExisting = new Dictionary<ParameterIsolucion, ParameterStatus>();

                foreach (var item in dataParametersRetrieved)
                {
                    var data = _dataBaseProviderService.GetParameterValue(item, _dataBaseProvider, _connectionString, out var existParameter);

                    dataCompany.Add(item.ParameterIsolucion, data);
                    _dataParametersNotExisting.Add(item.ParameterIsolucion, existParameter);

                    switch (item.ParameterIsolucion)
                    {
                        case ParameterIsolucion.RutaAnexo:
                            BancoAnexo.CreateParameterAndFillItValue = existParameter == ParameterStatus.DoesNotExist;
                            BancoAnexo.FillParameterValue = existParameter == ParameterStatus.NullOrEmpty;
                            break;
                        case ParameterIsolucion.RutaWebSite:
                            Web.CreateParameterAndFillItValue = existParameter == ParameterStatus.DoesNotExist;
                            Web.FillParameterValue = existParameter == ParameterStatus.NullOrEmpty;
                            break;
                        case ParameterIsolucion.RutaArticulo:
                            BancoConocimiento.CreateParameterAndFillItValue = existParameter == ParameterStatus.DoesNotExist;
                            BancoConocimiento.FillParameterValue = existParameter == ParameterStatus.NullOrEmpty;
                            break;
                        case ParameterIsolucion.RutaLibreria:
                            Libreria.CreateParameterAndFillItValue = existParameter == ParameterStatus.DoesNotExist;
                            Libreria.FillParameterValue = existParameter == ParameterStatus.NullOrEmpty;
                            break;
                        case ParameterIsolucion.RutaHttpServicios:
                            IsolucionServicio.CreateParameterAndFillItValue = existParameter == ParameterStatus.DoesNotExist;
                            IsolucionServicio.FillParameterValue = existParameter == ParameterStatus.NullOrEmpty;
                            break;
                        case ParameterIsolucion.RutaBaseCacheDatos:
                            Template.CreateParameterAndFillItValue = existParameter == ParameterStatus.DoesNotExist;
                            Template.FillParameterValue = existParameter == ParameterStatus.NullOrEmpty;
                            break;
                        case ParameterIsolucion.AppRutaServicioWebSf:
                            SmartFlow.CreateParameterAndFillItValue = existParameter == ParameterStatus.DoesNotExist;
                            SmartFlow.FillParameterValue = existParameter == ParameterStatus.NullOrEmpty;
                            break;
                        case ParameterIsolucion.WebSite:
                            SmartFlow.CreateParameterAndFillItValue = existParameter == ParameterStatus.DoesNotExist;
                            SmartFlow.FillParameterValue = existParameter == ParameterStatus.NullOrEmpty;
                            break;
                        case ParameterIsolucion.GenericHandler:
                            SmartFlow.CreateParameterAndFillItValue = existParameter == ParameterStatus.DoesNotExist;
                            SmartFlow.FillParameterValue = existParameter == ParameterStatus.NullOrEmpty;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                TheCompanyMapperDto = new CompanyMapperDto
                {
                    Name = dataCompany[ParameterIsolucion.WebSite],
                    SmartFlowPath = dataCompany[ParameterIsolucion.AppRutaServicioWebSf],
                    IsolucionServicioPath = dataCompany[ParameterIsolucion.RutaHttpServicios],
                    NewsLetterPath = string.Empty,
                    Id = Guid.NewGuid(),
                    BancoAnexoPath = dataCompany[ParameterIsolucion.RutaAnexo],
                    BancoConocimientoPath = dataCompany[ParameterIsolucion.RutaArticulo],
                    LibreriaPath = dataCompany[ParameterIsolucion.RutaLibreria],
                    LicencePath = selectedPath,
                    TemplatePath = dataCompany[ParameterIsolucion.RutaBaseCacheDatos],
                    WebPath = dataCompany[ParameterIsolucion.RutaWebSite],
                    RootPath = selectedPath,
                    GenericHandlerPath = dataCompany[ParameterIsolucion.GenericHandler],
                };

                PathData.Visibility = Visibility.Visible;
                PanelGrid.Visibility = PanelClose.Visibility = LicenseView.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                SetContentControlMessage(Error, $"La ruta seleccionada no es correcta");
                Log.Instance.Error($"Se presentó un error al obtener la lista con la ruta seleccionada en el equipo {ex.Message}");
            }
        }

        private void SaveCompanyMapper_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void FileChooser_OnClick(object sender, RoutedEventArgs e)
        {
            var currentButton = (Button)sender;

            if (currentButton == null) return;

            if (!(currentButton.DataContext is CompanyMapperDto dataContext)) return;

            var folderBrowserDialog = new FolderBrowserDialog
            {
                Description = @"Seleccione la unidad"
            };

            var theFolderBorwserDialog = folderBrowserDialog.ShowDialog();

            if (theFolderBorwserDialog != System.Windows.Forms.DialogResult.OK) return;

            foreach (var propertyInfo in dataContext.GetType().GetProperties())
            {
                if (propertyInfo.Name != currentButton.Name) continue;

                propertyInfo.SetValue(dataContext, folderBrowserDialog.SelectedPath);
                break;
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var currentButton = (Button)sender;

            if (currentButton == null) return;

            var dataContext = currentButton.DataContext as CompanyMapperDto;

            if (dataContext == null) return;

            var currentCompany = CompanyMappers.FirstOrDefault(x => x.Id == dataContext.Id);

            if (currentCompany == null)
                return;

            CompanyMappers.Remove(currentCompany);
        }

        private void SaveCompanyInGrid_OnClick(object sender, RoutedEventArgs e)
        {
            Error.Content = string.Empty;

            if (_dataParametersNotExisting.Any(x => x.Value != ParameterStatus.Exist))
            {
                List<IsoluctionParameterDto> dataParametersRetrieved;
                using (
               var streamReader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"\IsolucionParameter.json")
           )
                {
                    var jsonLicenseParameters = streamReader.ReadToEnd();
                    dataParametersRetrieved =
                        JsonConvert.DeserializeObject<List<IsoluctionParameterDto>>(jsonLicenseParameters);
                }

                foreach (var parameterStatus in _dataParametersNotExisting)
                {
                    switch (parameterStatus.Key)
                    {
                        case ParameterIsolucion.RutaAnexo:
                            ManageParameter(parameterStatus, dataParametersRetrieved);
                            break;
                        case ParameterIsolucion.RutaWebSite:
                            ManageParameter(parameterStatus, dataParametersRetrieved);
                            break;
                        case ParameterIsolucion.RutaArticulo:
                            ManageParameter(parameterStatus, dataParametersRetrieved);
                            break;
                        case ParameterIsolucion.RutaLibreria:
                            ManageParameter(parameterStatus, dataParametersRetrieved);
                            break;
                        case ParameterIsolucion.RutaHttpServicios:
                            ManageParameter(parameterStatus, dataParametersRetrieved);
                            break;
                        case ParameterIsolucion.RutaBaseCacheDatos:
                            ManageParameter(parameterStatus, dataParametersRetrieved);
                            break;
                        case ParameterIsolucion.AppRutaServicioWebSf:
                            ManageParameter(parameterStatus, dataParametersRetrieved);
                            break;
                        case ParameterIsolucion.WebSite:
                            ManageParameter(parameterStatus, dataParametersRetrieved);
                            break;
                        case ParameterIsolucion.GenericHandler:
                            ManageParameter(parameterStatus, dataParametersRetrieved);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                var dialogResult =
                    MessageBox.Show("Se guardara la configuración de la empresa ¿desea continuar?", "Isolucion",
                        MessageBoxButton.YesNo);

                switch (dialogResult)
                {
                    case MessageBoxResult.None:
                        break;
                    case MessageBoxResult.OK:
                        
                        CompanyMappers.Add(TheCompanyMapperDto);
                        PathData.Visibility = Visibility.Collapsed;
                        LicenseView.Visibility = Visibility.Collapsed;
                        break;
                    case MessageBoxResult.Cancel:
                        Error.Content = "Seleccione las rutas de los parámetros faltantes o revisa la configuración del Isolucion instalado";
                        break;
                    case MessageBoxResult.Yes:
                        var companyMapped = CompanyMappers.FirstOrDefault(x => x.Name == TheCompanyMapperDto.Name);
                        if (companyMapped != null)
                            CompanyMappers.Remove(companyMapped);
                        CompanyMappers.Add(TheCompanyMapperDto);
                        PathData.Visibility = Visibility.Collapsed;
                        LicenseView.Visibility = Visibility.Collapsed;
                        break;
                    case MessageBoxResult.No:
                        Error.Content = "Seleccione las rutas de los parámetros faltantes o revisa la configuración del Isolucion instalado";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return;
            }

            CompanyMappers.Add(TheCompanyMapperDto);
            PathData.Visibility = Visibility.Collapsed;
        }

        private void ManageParameter(KeyValuePair<ParameterIsolucion, ParameterStatus> parameterStatus, IEnumerable<IsoluctionParameterDto> dataParametersRetrieved)
        {
            switch (parameterStatus.Value)
            {
                case ParameterStatus.DoesNotExist:
                    _dataBaseProviderService.ExecuteCommand(
                        dataParametersRetrieved.FirstOrDefault(
                            x => x.ParameterIsolucion == parameterStatus.Key), _dataBaseProvider,
                        _connectionString, SqlTask.CreateColumn);
                    break;
                case ParameterStatus.NullOrEmpty:
                    _dataBaseProviderService.ExecuteCommand(
                        dataParametersRetrieved.FirstOrDefault(
                            x => x.ParameterIsolucion == parameterStatus.Key), _dataBaseProvider,
                        _connectionString, SqlTask.InsertRecord);
                    break;
                case ParameterStatus.Exist:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SetContentControlMessage(ContentControl label, string message)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal,
                new Action(() => { label.Content = message; }));
        }
    }
}