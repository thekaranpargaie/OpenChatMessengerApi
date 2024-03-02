using Consul;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using Shared.Constants;
using Shared.Enum;
using System.Net.Http.Headers;
using System.Text;

namespace Shared.Utility
{
    public class ServiceDicscoveryApiClient
    {
        private readonly List<Uri> _serverUrls;
        private readonly HttpClient _apiClient;
        private AsyncRetryPolicy _serverRetryPolicy;
        private readonly string _serviceDiscoveryAdress;
        private readonly string _requestedServiceName;
        private readonly int _serviceName;
        private int _currentConfigIndex;
        private int _retryCount;
        private const int MaxRetryCount = 8;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceDiscoveryAdress">Service discovery base address mentioned in docker-compose.override.yml file</param>
        /// <param name="requestedServiceName">Service Name to be requested </param>

        public ServiceDicscoveryApiClient(string serviceDiscoveryAdress, string requestedServiceName, string Token, Guid? userId = null)
        {
            _apiClient = new HttpClient();
            _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(CommonConstants.MediaTypeJson));
            _apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
            if (userId.HasValue)
                _apiClient.DefaultRequestHeaders.Add("Id", userId.Value.ToString());
            _serverUrls = new List<Uri>();
            _serviceDiscoveryAdress = serviceDiscoveryAdress;
            _requestedServiceName = requestedServiceName;
        }
        public async Task Initialize()
        {
            ConsulClient consulClient = new ConsulClient(c =>
            {
                var uri = new Uri(_serviceDiscoveryAdress);
                c.Address = uri;
            });


            QueryResult<Dictionary<string, AgentService>> services = await consulClient.Agent.Services();
            foreach (var service in services.Response)
            {
                bool isServiceApi = service.Value.Service == _requestedServiceName;
                if (isServiceApi)
                {
                    Uri serviceUri = new Uri($"{service.Value.Address}:{service.Value.Port}");
                    _serverUrls.Add(serviceUri);
                }
            }


            _serverRetryPolicy = Polly.Policy.Handle<HttpRequestException>()
                .WaitAndRetryAsync(MaxRetryCount, retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(2, retryAttempt)), onRetry: async (exception, timeSpan, retryCount) =>
                {
                    _retryCount++;
                    ChooseNextServer(_retryCount);
                });

        }
        private void ChooseNextServer(int retryCount)
        {
            if (retryCount % 2 == 0)
            {
                _currentConfigIndex++;

                if (_currentConfigIndex > _serverUrls.Count - 1)
                    _currentConfigIndex = 0;
            }
        }
        public async Task<T> GetResponse<T>(string apiPath, HttpMethodType httpMethodType, object postData = null)
        {
            this.Initialize().Wait();
            var result = await _serverRetryPolicy.ExecuteAsync(async () =>
            {
                Uri serverUrl = _serverUrls[_currentConfigIndex];
                T returnValue = default;
                HttpContent postDataContent = null;
                string requestPath = $"http://{serverUrl}{apiPath}";
                HttpResponseMessage response = null;
                switch (httpMethodType)
                {
                    case HttpMethodType.Get:
                        response = await _apiClient.GetAsync(requestPath).ConfigureAwait(false);
                        break;
                    case HttpMethodType.Post:
                        postDataContent = new StringContent(JsonConvert.SerializeObject(postData), Encoding.UTF8, CommonConstants.MediaTypeJson);
                        response = await _apiClient.PostAsync(requestPath, postDataContent).ConfigureAwait(false);
                        break;
                    case HttpMethodType.Delete:
                        response = await _apiClient.DeleteAsync(requestPath).ConfigureAwait(false);
                        break;
                    case HttpMethodType.Put:
                        postDataContent = new StringContent(JsonConvert.SerializeObject(postData), Encoding.UTF8, CommonConstants.MediaTypeJson);
                        response = await _apiClient.PutAsync(requestPath, postDataContent).ConfigureAwait(false);
                        break;
                    case HttpMethodType.FormUrlEncoded:
                        response = await _apiClient.PostAsync(requestPath, (FormUrlEncodedContent)postData).ConfigureAwait(false);
                        break;

                }
                if (response != null)
                {
                    string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    if (!string.IsNullOrEmpty(content))
                    {
                        try
                        {
                            returnValue = JsonConvert.DeserializeObject<T>(content);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                }
                else
                {
                    Console.Write($"error in response {response}");
                }
                return returnValue;
            });
            return result;
        }
        public void CallAsync(string apiPath, HttpMethodType httpMethodType, object postData = null)
        {
            this.Initialize().Wait();
            _serverRetryPolicy.ExecuteAsync(async () =>
            {
                Uri serverUrl = _serverUrls[_currentConfigIndex];
                HttpContent postDataContent = null;
                string requestPath = $"http://{serverUrl}/{apiPath}";
                HttpResponseMessage response = null;
                switch (httpMethodType)
                {
                    case HttpMethodType.Get:
                        response = await _apiClient.GetAsync(requestPath).ConfigureAwait(false);
                        break;
                    case HttpMethodType.Post:
                        postDataContent = new StringContent(JsonConvert.SerializeObject(postData), Encoding.UTF8, CommonConstants.MediaTypeJson);
                        response = await _apiClient.PostAsync(requestPath, postDataContent).ConfigureAwait(false);
                        break;
                    case HttpMethodType.Delete:
                        response = await _apiClient.DeleteAsync(requestPath).ConfigureAwait(false);
                        break;
                    case HttpMethodType.Put:
                        postDataContent = new StringContent(JsonConvert.SerializeObject(postData), Encoding.UTF8, CommonConstants.MediaTypeJson);
                        response = await _apiClient.PutAsync(requestPath, postDataContent).ConfigureAwait(false);
                        break;

                }
                if (response != null && response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
                else
                {
                    Console.Write($"error in response {response}");
                }
            });
        }
        public Task<byte[]> GetImageAsByteArray(string imageApiPath)
        {
            this.Initialize().Wait();
            return _serverRetryPolicy.ExecuteAsync(async () =>
             {
                 Uri serverUrl = _serverUrls[_currentConfigIndex];
                 string requestPath = $"http://{serverUrl}/{imageApiPath}";
                 HttpResponseMessage response = await _apiClient.GetAsync(requestPath).ConfigureAwait(false);
                 byte[] byteArray = default;
                 if (response != null && response.IsSuccessStatusCode)
                 {
                     var result = response.Content.ReadAsStringAsync().Result.Replace("\"", string.Empty);
                     byteArray = Convert.FromBase64String(result);
                 }
                 else
                 {
                     Console.Write($"error in response {response}");

                 }
                 return byteArray;
             });
        }
    }
}
