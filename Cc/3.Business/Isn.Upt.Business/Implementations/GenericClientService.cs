using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Isn.Common.CustomResponse;
using Isn.Common.LogHelper;
using Isn.Upt.Business.Definitions;
using Isn.Upt.Business.Implementations.Singleton;
using Newtonsoft.Json;

namespace Isn.Upt.Business.Implementations
{
    public class GenericClientService : IGenericClientService
    {
        public ResponseWrapper<T> Get<T>(string requestUri, IDictionary<string, string> aditionalHeaders = null)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ConfigurationUpdaterManager.Instance.ApiUrl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var byteArray =
                        Encoding.ASCII.GetBytes(ConfigurationUpdaterManager.Instance.UserName + ":" +
                                                ConfigurationUpdaterManager.Instance.Password);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(byteArray));

                    if (aditionalHeaders != null && aditionalHeaders.Count > 0)
                        foreach (var aditionalHeader in aditionalHeaders)
                            client.DefaultRequestHeaders.Add(aditionalHeader.Key, aditionalHeader.Value);

                    var response = client.GetAsync(requestUri).Result;
                    response.EnsureSuccessStatusCode();

                    using (var content = response.Content)
                    {
                        var responseBody = content.ReadAsStringAsync().Result;
                        return new ResponseWrapper<T>
                        {
                            Data = JsonConvert.DeserializeObject<T>(responseBody),
                            Exception = null
                        };
                    }
                }
            }
            catch (Exception e)
            {
                Log.Instance.Info("Error");
                return new ResponseWrapper<T>
                {
                    Data = default(T),
                    Exception = e
                };
            }
        }

        public ResponseWrapper<T> Post<T>(object entity, string requestUri,
            IDictionary<string, string> aditionalHeaders = null)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ConfigurationUpdaterManager.Instance.ApiUrl);
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    var byteArray =
                        Encoding.ASCII.GetBytes(ConfigurationUpdaterManager.Instance.UserName + ":" +
                                                ConfigurationUpdaterManager.Instance.Password);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(byteArray));

                    if (aditionalHeaders != null && aditionalHeaders.Count > 0)
                        foreach (var aditionalHeader in aditionalHeaders)
                            client.DefaultRequestHeaders.Add(aditionalHeader.Key, aditionalHeader.Value);

                    var response = client.PostAsync(requestUri,
                        new StringContent(JsonConvert.SerializeObject(entity),
                            Encoding.UTF8, "application/json")).Result;

                    response.EnsureSuccessStatusCode();

                    using (var content = response.Content)
                    {
                        var responseBody = content.ReadAsStringAsync().Result;
                        return new ResponseWrapper<T>
                        {
                            Data = JsonConvert.DeserializeObject<T>(responseBody),
                            Exception = null
                        };
                    }
                }
            }
            catch (Exception e)
            {
                Log.Instance.Error(e, "Error");
                return new ResponseWrapper<T>
                {
                    Data = default(T),
                    Exception = e
                };
            }
        }
    }
}