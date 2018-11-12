using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Cc.Common.Isolucion;
using Cc.Common.LogHelper;
using Cc.Upt.Business.Definitions;
using Cc.Upt.Data.Definitions;
using Cc.Upt.Data.Implementations;
using Cc.Upt.Domain;
using Cc.Upt.Domain.Dto;
using Cc.Upt.Domain.Enumerations;

namespace Cc.Upt.Business.Implementations
{
    public class ParameterService : EntityService<Parameter>, IParameterService
    {
        public ParameterService(IContext context) : base(context)
        {
            Dbset = context.Set<Parameter>();
        }

        public IEnumerable<Parameter> GetAllParameters()
        {
            return GetAll();
        }

        public Parameter GetById(Guid id)
        {
            return FindBy(x => x.Id == id).FirstOrDefault();
        }

        public Parameter GetByInternalIdentificator(ParameterInternalIdentificator parameterInternalIdentificator)
        {
            return FindBy(x => x.ParameterInternalIdentificator == parameterInternalIdentificator).FirstOrDefault();
        }

        public bool Save(Parameter parameter, bool isSingle = false)
        {
            try
            {
                var currentParameter =
                    FindBy(x => x.ParameterInternalIdentificator == parameter.ParameterInternalIdentificator)
                        .FirstOrDefault();

                if (currentParameter != null)
                {
                    if (!isSingle)
                        currentParameter.Value = string.IsNullOrEmpty(currentParameter.Value)
                            ? parameter.Value
                            : currentParameter.Value;
                    else
                        currentParameter.Value = parameter.Value;

                    currentParameter.ParameterType = parameter.ParameterType;
                    Update(currentParameter);
                    return true;
                }

                Create(parameter);
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        public void PrepareData()
        {
            var dataEnumerationList =
                Enum.GetValues(typeof(ParameterInternalIdentificator)).Cast<ParameterInternalIdentificator>();

            var currenDataParameters = GetAllParameters();
            var temporalParameter =
                currenDataParameters.Where(
                    currenDataParameter =>
                        dataEnumerationList.All(x => x != currenDataParameter.ParameterInternalIdentificator)).ToList();

            foreach (var parameter in temporalParameter)
                Delete(parameter);

            foreach (var parameterInternalIdentificator in dataEnumerationList)
                switch (parameterInternalIdentificator)
                {
                    case ParameterInternalIdentificator.SmtpServer:
                        Save(new Parameter
                        {
                            ParameterInternalIdentificator = parameterInternalIdentificator,
                            ParameterType = ParameterType.Text,
                            Value = string.Empty
                        });
                        break;
                    case ParameterInternalIdentificator.EmailSender:
                        Save(new Parameter
                        {
                            ParameterInternalIdentificator = parameterInternalIdentificator,
                            ParameterType = ParameterType.Email,
                            Value = string.Empty
                        });
                        break;
                    case ParameterInternalIdentificator.EmailReceiver:
                        Save(new Parameter
                        {
                            ParameterInternalIdentificator = parameterInternalIdentificator,
                            ParameterType = ParameterType.Email,
                            Value = string.Empty
                        });
                        break;
                    case ParameterInternalIdentificator.EnableSsl:
                        Save(new Parameter
                        {
                            ParameterInternalIdentificator = parameterInternalIdentificator,
                            ParameterType = ParameterType.CheckBox,
                            Value = string.Empty
                        });
                        break;
                    case ParameterInternalIdentificator.SendingPort:
                        Save(new Parameter
                        {
                            ParameterInternalIdentificator = parameterInternalIdentificator,
                            ParameterType = ParameterType.Number,
                            Value = string.Empty
                        });
                        break;
                    case ParameterInternalIdentificator.PasswordSender:
                        Save(new Parameter
                        {
                            ParameterInternalIdentificator = parameterInternalIdentificator,
                            ParameterType = ParameterType.Password,
                            Value = string.Empty
                        });
                        break;
                    case ParameterInternalIdentificator.FtpPath:
                        Save(new Parameter
                        {
                            ParameterInternalIdentificator = parameterInternalIdentificator,
                            ParameterType = ParameterType.Text,
                            Value = string.Empty
                        });
                        break;
                    case ParameterInternalIdentificator.FtpUser:
                        Save(new Parameter
                        {
                            ParameterInternalIdentificator = parameterInternalIdentificator,
                            ParameterType = ParameterType.Text,
                            Value = string.Empty
                        });
                        break;
                    case ParameterInternalIdentificator.FtpPassword:
                        Save(new Parameter
                        {
                            ParameterInternalIdentificator = parameterInternalIdentificator,
                            ParameterType = ParameterType.Password,
                            Value = string.Empty
                        });
                        break;
                    case ParameterInternalIdentificator.BufferSize:
                        Save(new Parameter
                        {
                            ParameterInternalIdentificator = parameterInternalIdentificator,
                            ParameterType = ParameterType.Number,
                            Value = string.Empty
                        });
                        break;
                    case ParameterInternalIdentificator.ValidDaysToken:
                        Save(new Parameter
                        {
                            ParameterInternalIdentificator = parameterInternalIdentificator,
                            ParameterType = ParameterType.Number,
                            Value = string.Empty
                        });
                        break;
                    case ParameterInternalIdentificator.VersionValue:
                        Save(new Parameter
                        {
                            ParameterInternalIdentificator = parameterInternalIdentificator,
                            ParameterType = ParameterType.Text,
                            Value = string.Empty
                        });
                        break;
                    case ParameterInternalIdentificator.NotesVersionMinLength:
                        Save(new Parameter
                        {
                            ParameterInternalIdentificator = parameterInternalIdentificator,
                            ParameterType = ParameterType.Number,
                            Value = string.Empty
                        });
                        break;
                    case ParameterInternalIdentificator.ReleaseDaysExpiration:
                        Save(new Parameter
                        {
                            ParameterInternalIdentificator = parameterInternalIdentificator,
                            ParameterType = ParameterType.Number,
                            Value = string.Empty
                        });
                        break;
                    case ParameterInternalIdentificator.IntervalExecutionInternalProcess:
                        Save(new Parameter
                        {
                            ParameterInternalIdentificator = parameterInternalIdentificator,
                            ParameterType = ParameterType.Number,
                            Value = string.Empty
                        });
                        break;
                    case ParameterInternalIdentificator.LocalPathForDownloadFiles:
                        Save(new Parameter
                        {
                            ParameterInternalIdentificator = parameterInternalIdentificator,
                            ParameterType = ParameterType.Text,
                            Value = string.Empty
                        });
                        break;
                    case ParameterInternalIdentificator.UserSmtpServer:
                        Save(new Parameter
                        {
                            ParameterInternalIdentificator = parameterInternalIdentificator,
                            ParameterType = ParameterType.Text,
                            Value = string.Empty
                        });
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
        }

        public T GetParameterValueByInternalIdentificator<T>(
            ParameterInternalIdentificator parameterInternalIdentificator)
        {
            var currentParameter = GetByInternalIdentificator(parameterInternalIdentificator);

            if (currentParameter == null)
                return default(T);

            if (string.IsNullOrEmpty(currentParameter.Value ))
            {
                return default(T);
            }

            try
            {
                var value = Convert.ChangeType(currentParameter.Value, typeof(T));
                return (T)value;
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex);
                return default(T);
            }
        }

        public IDictionary<LicenseParameter, string> GetIsolucionParameterValues(IEnumerable<IsoluctionParameterDto> parameters, string licensePath)
        {
            var currentDocument = GetIsolucionParameters(licensePath);
            var currentMetadata = XDocument.Parse(currentDocument.InnerXml);
            var dataToReturn = new Dictionary<LicenseParameter, string>();

            foreach (var parameter in parameters)
            {
                var currentElement = currentMetadata.Descendants("appSettings")
                    .Elements("Param")
                    .FirstOrDefault(x =>
                    {
                        var xAttribute = x.Attribute("Name");
                        return xAttribute != null && xAttribute.Value == parameter.ColumnOrField;
                    });

                if (currentElement != null)
                    dataToReturn.Add(parameter.LicenseParameter, currentElement.Value);
            }

            return dataToReturn;
        }

        private XmlDocument GetIsolucionParameters(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;

            var lNomArchivoClaves = path + @"\SFIXF.icn";
            var lNomArchivoLicencia = path + @"\SmartFlowIsol.icn";

            if (!File.Exists(lNomArchivoClaves))
                throw new Exception("No se encontró el archivo de claves de encripcion de licencia SFIXF.icn. Ruta: " +
                                    path);

            string strXml;
            using (var lReader = new StreamReader(lNomArchivoClaves, Encoding.Unicode))
            {
                strXml = lReader.ReadToEnd();
            }

            var cr = new Cripto();
            cr = GetKeys(strXml, cr);

            if (!File.Exists(lNomArchivoLicencia))
                throw new Exception("No se encontró el archivo de licencia SmartFlowIsol.icn. Ruta: " + path);

            using (var lReader = new StreamReader(lNomArchivoLicencia, Encoding.Unicode))
            {
                cr.CadenaEncriptada = lReader.ReadToEnd();
            }

            strXml = MassDecrypt(cr);

            try
            {
                var xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(strXml);
                return xmlDocument;
            }
            catch (Exception ex)
            {
                throw new Exception("[Parametro.LoadXML]: " + ex.Message + " [Stack]: " + ex.StackTrace +
                                    " [XML Entrada:]" + strXml);
            }
        }

        private static Cripto GetKeys(string texto, Cripto cripto)
        {
            var stringArray = new string[3];
            var index = texto.IndexOf("+*-|||||||||||||||||||||-*+", StringComparison.Ordinal);
            stringArray[0] = texto.Substring(0, index);
            stringArray[1] = texto.Substring(index + 27, texto.Length - (index + 27));
            cripto.LlaveEncripcion = stringArray[0];
            cripto.ViEncripcion = stringArray[1];
            return cripto;
        }

        public string MassDecrypt(Cripto cr)
        {
            var enc = new Encryptor();
            return enc.Decrypt(cr.CadenaEncriptada, cr.LlaveEncripcion, cr.ViEncripcion).CadenaDesencriptada;
        }
    }
}