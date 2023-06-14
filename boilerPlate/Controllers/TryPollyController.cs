using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using boilerPlate.Infra.ServiceContracts;
using Microsoft.AspNetCore.Mvc;
using Polly;
using Polly.Contrib.WaitAndRetry;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860


//Use the SampleServer.Py to emit the behaviour of a failing service this will 1st 3 times
namespace boilerPlate.Controllers
{

    [Route("api/[controller]/[action]")]
    public class TryPollyController : Controller
    {
        private readonly ILogger<TryPollyController> _logger;
        IConfigService _config;
        IHttpClientFactory _httpClientFactory;
        private readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy =
        Policy<HttpResponseMessage>
        .Handle<HttpRequestException>()
            .OrResult(x => x.StatusCode is >= System.Net.HttpStatusCode.InternalServerError || x.StatusCode == System.Net.HttpStatusCode.RequestTimeout || x.StatusCode == System.Net.HttpStatusCode.Forbidden)
            //.RetryAsync(5); //Burst Mode
            //.WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))); //Wait 2 Second Every Request
            .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1),5)); //Random Time Gap
        public TryPollyController(ILogger<TryPollyController> logger, IConfigService config,IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _config = config;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<string> TryPolly(string id)
        {
            var client = _httpClientFactory.CreateClient();

            var response = await _retryPolicy.ExecuteAsync(async () =>
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"http://localhost:8000/?name={id}");
                return await client.SendAsync(request);
            });
            var body = await response.Content.ReadAsStringAsync();
            return body;
        }
        
    }
}

