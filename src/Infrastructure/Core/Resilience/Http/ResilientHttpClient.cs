using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Wrap;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Massena.Infrastructure.Core.Resilience.Http
{
    /// <summary>
    /// HttpClient wrapper that integrates Retry and Circuit
    /// breaker policies when invoking HTTP services. 
    /// Based on Polly library: https://github.com/App-vNext/Polly
    /// </summary>
    public class ResilientHttpClient : IHttpClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<ResilientHttpClient> _logger;
        private readonly Func<string, IEnumerable<Policy>> _policyCreator;
        private ConcurrentDictionary<string, PolicyWrap> _policyWrappers;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ResilientHttpClient(Func<string, IEnumerable<Policy>> policyCreator, ILogger<ResilientHttpClient> logger, IHttpContextAccessor httpContextAccessor)
        {
            _client = new HttpClient();
            _logger = logger;
            _policyCreator = policyCreator;
            _policyWrappers = new ConcurrentDictionary<string, PolicyWrap>();
            _httpContextAccessor = httpContextAccessor;
        }


        public Task<TResponse> PostAsync<TRequest, TResponse>(string uri, TRequest item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {
            return DoPostPutAsync<TRequest, TResponse>(HttpMethod.Post, uri, item, authorizationToken, requestId, authorizationMethod);
        }

        public Task<TResponse> PutAsync<TRequest, TResponse>(string uri, TRequest item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {
            return DoPostPutAsync<TRequest, TResponse>(HttpMethod.Put, uri, item, authorizationToken, requestId, authorizationMethod);
        }

        private Task<TResponse> DoPostPutAsync<TRequest, TResponse>(HttpMethod method, string uri, TRequest item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {
            if (method != HttpMethod.Post && method != HttpMethod.Put)
                throw new ArgumentException("Value must be either post or put.", nameof(method));

            // a new StringContent must be created for each retry 
            // as it is disposed after each call
            var origin = GetOriginFromUri(uri);

            return HttpInvoker(origin, async () =>
            {
                var requestMessage = new HttpRequestMessage(method, uri);

                SetAuthorizationHeader(requestMessage);

                requestMessage.Content = new StringContent(JsonConvert.SerializeObject(item), System.Text.Encoding.UTF8, "application/json");

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
            });
        }

        public Task<TResponse> DeleteAsync<TResponse>(string uri, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {
            var origin = GetOriginFromUri(uri);

            return HttpInvoker(origin, async () =>
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

                SetAuthorizationHeader(requestMessage);

                if (authorizationToken != null)
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);

                if (requestId != null)
                    requestMessage.Headers.Add("x-requestid", requestId);

                var response = await _client.SendAsync(requestMessage);
                var content = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<TResponse>(content);
            });
        }


        public Task<string> GetStringAsync(string uri, string authorizationToken = null, string authorizationMethod = "Bearer")
        {
            var origin = GetOriginFromUri(uri);

            return HttpInvoker(origin, async () =>
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

                SetAuthorizationHeader(requestMessage);

                if (authorizationToken != null)
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);

                var response = await _client.SendAsync(requestMessage);

                // raise exception if HttpResponseCode 500 needed for circuit breaker to track fails

                if (response.StatusCode == HttpStatusCode.InternalServerError)
                    throw new HttpRequestException();

                return await response.Content.ReadAsStringAsync();
            });
        }

        private async Task<T> HttpInvoker<T>(string origin, Func<Task<T>> action)
        {
            var normalizedOrigin = NormalizeOrigin(origin);

            if (!_policyWrappers.TryGetValue(normalizedOrigin, out PolicyWrap policyWrap))
            {
                policyWrap = Policy.WrapAsync(_policyCreator(normalizedOrigin).ToArray());
                _policyWrappers.TryAdd(normalizedOrigin, policyWrap);
            }

            // Executes the action applying all the policies defined in the wrapper
            return await policyWrap.ExecuteAsync((ct) => action(), new Context(normalizedOrigin));
        }


        private static string NormalizeOrigin(string origin)
        {
            return origin?.Trim()?.ToLower();
        }

        private static string GetOriginFromUri(string uri)
        {
            var url = new Uri(uri);

            var origin = $"{url.Scheme}://{url.DnsSafeHost}:{url.Port}";

            return origin;
        }

        private void SetAuthorizationHeader(HttpRequestMessage requestMessage)
        {
            var authorizationHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                requestMessage.Headers.Add("Authorization", new List<string>() { authorizationHeader });
            }
        }

        public Task<TResponse> PostForm<TResponse>(string uri, Dictionary<string, string> form, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {
            // a new StringContent must be created for each retry 
            // as it is disposed after each call
            var origin = GetOriginFromUri(uri);

            return HttpInvoker(origin, async () =>
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
            });
        }
    }
}