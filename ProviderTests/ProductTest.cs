using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using PactNet;
using PactNet.Infrastructure.Outputters;
using PactNet.Verifier;
using Xunit.Abstractions;

namespace ProviderTests
{
    public class ProductTest
    {
        private string _providerUri { get; }

        private string _pactServiceUrl { get; }

        private ITestOutputHelper _outputHelper { get; }

        private readonly IHost server;
        public ProductTest(ITestOutputHelper output)
        {
            _outputHelper = output;
            _providerUri = "http://localhost:9003";
            _pactServiceUrl = "https://tavisca.pactflow.io";

            this.server = Host.CreateDefaultBuilder()
                            .ConfigureWebHostDefaults(webBuilder =>
                            {
                                webBuilder.UseUrls(_providerUri.ToString());
                                webBuilder.UseStartup<TestStartup>();
                            })
                            .Build();

            this.server.Start();
        }

        [Fact]
        public void EnsureProviderApiHonoursPactWithConsumer()
        {
            // Arrange
            var config = new PactVerifierConfig
            {
                // NOTE: We default to using a ConsoleOutput, however xUnit 2 does not capture the console output,
                // so a custom outputter is required.
                Outputters = new List<IOutput>
                {
                    new XUnitOutput(_outputHelper),
                    new ConsoleOutput()
                },
                LogLevel = PactLogLevel.Debug
            };

            IPactVerifier pactVerifier = new PactVerifier(config);


            ////Act / Assert

            //var pactFile = new DirectoryInfo(Path.Join("..", "..", "..", "..", "Tests", "pacts"));
            //pactVerifier
            //    .ServiceProvider("ProductService", new Uri($"{_providerUri}"))
            //    .WithDirectorySource(pactFile, new string[] { "ApiClient" })
            //    .WithProviderStateUrl(new Uri($"{_providerUri}/provider-states"))
            //    .Verify();

            var pactFile = new DirectoryInfo(Path.Join("..", "..", "..", "..", "Tests", "pacts"));
            pactVerifier.ServiceProvider("ProductService", new Uri(_providerUri))
              .WithPactBrokerSource(new Uri(_pactServiceUrl), options =>
              {
                  options.ConsumerVersionSelectors();
                  options.TokenAuthentication("eQPTrz_9kbnWLDBKQ5Gttg");
                  // options.BasicAuthentication(System.Environment.GetEnvironmentVariable("PACT_BROKER_USERNAME"), System.Environment.GetEnvironmentVariable("PACT_BROKER_PASSWORD"));                  
                  options.PublishResults(true, "1.0.0", results =>
                  {
                  }).EnablePending();
              })             
              .WithProviderStateUrl(new Uri($"{_providerUri}/provider-states"))
              .Verify();

        }

        public void Dispose()
        {
            this.server.Dispose();
        }
    }
}