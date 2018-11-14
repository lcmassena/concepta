using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Massena.Infrastructure.Core.Resilience.Http
{
    public class StandardHttpClient : IHttpClient
    {
        private HttpClient _client;

        public StandardHttpClient()
        {
            _client = new HttpClient();
        }

        public async Task<string> GetStringAsync(string uri, string authorizationToken = null, string authorizationMethod = "Bearer")
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            if (authorizationToken != null)
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);

            var response = await _client.SendAsync(requestMessage);

            return await response.Content.ReadAsStringAsync();
        }

        public T Get<T>(string uri, string authorizationToken = null, string authorizationMethod = "Bearer")
        {
            string result = GetStringAsync(uri, authorizationToken, authorizationMethod).Result;

            return JsonConvert.DeserializeObject<T>(result);
        }

        public async Task<T> GetAsync<T>(string uri, string authorizationToken = null, string authorizationMethod = "Bearer")
        {
            string result = await GetStringAsync(uri, authorizationToken, authorizationMethod);

            return JsonConvert.DeserializeObject<T>(result);
        }

        public async Task<byte[]> DownloadFile(string uri, string authorizationToken = null, string authorizationMethod = "Bearer")
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            if (authorizationToken != null)
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);

            var response = await _client.SendAsync(requestMessage);

            return await response.Content.ReadAsByteArrayAsync();
        }
        public async Task<TResponse> DeleteAsync<TResponse>(string uri, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            if (authorizationToken != null)
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);

            if (requestId != null)
                requestMessage.Headers.Add("x-requestid", requestId);

            var response = await _client.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TResponse>(content);
        }

        public Task<TResponse> PutAsync<TInput, TResponse>(string uri, TInput item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {
            return DoPostPutAsync<TInput, TResponse>(HttpMethod.Put, uri, item, authorizationToken, requestId, authorizationToken);
        }

        public Task<TResponse> PostAsync<TInput, TResponse>(string uri, TInput item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {
            return DoPostPutAsync<TInput, TResponse>(HttpMethod.Post, uri, item, authorizationToken, requestId, authorizationMethod);
        }


        private async Task<TResponse> DoPostPutAsync<TInput, TResponse>(HttpMethod method, string uri, TInput item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {
            if (method != HttpMethod.Post && method != HttpMethod.Put)
                throw new ArgumentException("Value must be either post or put.", nameof(method));

            // a new StringContent must be created for each retry 
            // as it is disposed after each call

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            requestMessage.Content = new StringContent(JsonConvert.SerializeObject(item), System.Text.Encoding.UTF8, "application/json");

            if (authorizationToken != null)
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);

            if (requestId != null)
                requestMessage.Headers.Add("x-requestid", requestId);

            var response = await _client.SendAsync(requestMessage);

            // raise exception if HttpResponseCode 500 
            // needed for circuit breaker to track fails

            if (response.StatusCode == HttpStatusCode.InternalServerError)
                throw new HttpRequestException();

            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TResponse>(content);
        }

        public async Task<TResponse> PostForm<TResponse>(string uri, Dictionary<string, string> form, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri);
            requestMessage.Content = new FormUrlEncodedContent(form.Select(x => new KeyValuePair<string, string>(x.Key, x.Value)));   

            if (authorizationToken != null)
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);

            if (requestId != null)
                requestMessage.Headers.Add("x-requestid", requestId);

            var response = await _client.SendAsync(requestMessage);

            // raise exception if HttpResponseCode 500 needed for circuit breaker to track fails
            if (response.StatusCode == HttpStatusCode.InternalServerError)
                throw new HttpRequestException();

            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TResponse>(content);
        }
    }
}

