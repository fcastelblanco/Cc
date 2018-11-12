using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Cc.Upt.Domain.Dto
{
    public class CompanyMapperDto : INotifyPropertyChanged
    {
        private string _bancoAnexoPath;
        private string _bancoConocimientoPath;
        private string _isolucionServicioPath;
        private string _libreriaPath;
        private string _name;
        private string _newsLetterPath;
        private string _smartFlowPath;
        private string _templatePath;
        private string _webPath;
        private string _licencePath;
        private string _rootPath;
        private string _genericHandlerPath;

        public string GenericHandlerPath
        {
            get { return _genericHandlerPath; }
            set
            {
                _genericHandlerPath = value;
                OnPropertyChanged(nameof(GenericHandlerPath));
            }
        }



        public string RootPath
        {
            get { return _rootPath; }
            set
            {
                _rootPath = value;
                OnPropertyChanged(nameof(RootPath));
            }
        }

        public string LicencePath
        {
            get { return _licencePath; }
            set
            {
                _licencePath = value;
                OnPropertyChanged(nameof(LicencePath));
            }
        }
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string SmartFlowPath
        {
            get { return _smartFlowPath; }
            set
            {
                _smartFlowPath = value;
                OnPropertyChanged(nameof(SmartFlowPath));
            }
        }

        public string NewsLetterPath
        {
            get { return _newsLetterPath; }
            set
            {
                _newsLetterPath = value;
                OnPropertyChanged(nameof(NewsLetterPath));
            }
        }

        public string IsolucionServicioPath
        {
            get { return _isolucionServicioPath; }
            set
            {
                _isolucionServicioPath = value;
                OnPropertyChanged(nameof(IsolucionServicioPath));
            }
        }

        public Guid Id { get; set; }

        public string BancoAnexoPath
        {
            get { return _bancoAnexoPath; }
            set
            {
                _bancoAnexoPath = value;
                OnPropertyChanged(nameof(BancoAnexoPath));
            }
        }

        public string BancoConocimientoPath
        {
            get { return _bancoConocimientoPath; }
            set
            {
                _bancoConocimientoPath = value;
                OnPropertyChanged(nameof(BancoConocimientoPath));
            }
        }

        public string LibreriaPath
        {
            get { return _libreriaPath; }
            set
            {
                _libreriaPath = value;
                OnPropertyChanged(nameof(LibreriaPath));
            }
        }

        public string TemplatePath
        {
            get { return _templatePath; }
            set
            {
                _templatePath = value;
                OnPropertyChanged(nameof(TemplatePath));
            }
        }

        public string WebPath
        {
            get { return _webPath; }
            set
            {
                _webPath = value;
                OnPropertyChanged(nameof(WebPath));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}