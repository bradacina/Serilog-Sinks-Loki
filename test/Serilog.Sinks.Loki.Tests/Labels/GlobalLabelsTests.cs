using System.Linq;
using Newtonsoft.Json;
using Serilog.Sinks.Loki.Tests.Infrastructure;
using Shouldly;
using Xunit;

namespace Serilog.Sinks.Loki.Tests.Labels
{
    public class GlobalLabelsTests : IClassFixture<HttpClientTestFixture>
    {
        private readonly HttpClientTestFixture _httpClientTestFixture;
        private readonly TestHttpClient _client;
        private readonly BasicAuthCredentials _credentials;

        public GlobalLabelsTests(HttpClientTestFixture httpClientTestFixture)
        {
            _httpClientTestFixture = httpClientTestFixture;
            _client = new TestHttpClient();
            _credentials = new BasicAuthCredentials("http://test:80", "Walter", "White");
        }
        
        [Fact]
        public void GlobalLabelsCanBeSet()
        {
            // Arrange
            var log = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.LokiHttp(_credentials, new TestLabelProvider(), _client)
                .CreateLogger();
            
            // Act
            log.Error("Something's wrong");
            log.Dispose();
            
            // Assert
            var response = JsonConvert.DeserializeObject<TestResponse>(_client.Content);
            response.Streams.First().Labels.ShouldBe("{level=\"error\",app=\"tests\"}");
        }
    }
}