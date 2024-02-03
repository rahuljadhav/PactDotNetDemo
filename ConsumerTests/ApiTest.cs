using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PactNet;
using PactNet.Matchers;
using System.Net;
using Xunit.Abstractions;

namespace Tests
{
    public class ApiTest
    {
        private IPactBuilderV3 pact;
        private readonly Consumer.Consumer ApiClient;
        private readonly int port = 9000;
        private readonly List<object> products;

        public ApiTest(ITestOutputHelper output)
        {
            products = new List<object>()
            {
                new { id = 1, type = "Electronics", name = "Laptop", price = "$ 300" },
                new { id = 2, type = "Educational", name = "Notebook", price = "$ 3" },
                new { id = 3, type = "Educational", name = "PaintBox", price = "$ 5" },
            };

            var Config = new PactConfig
            {
                PactDir = Path.Join("..", "..", "..", "..", "..", "pacts"),
                Outputters = new[] { new XUnitOutput(output) },
                DefaultJsonSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }
            };

            pact = Pact.V3("Consumer", "ProductService", Config).WithHttpInteractions(port);
            ApiClient = new Consumer.Consumer(new System.Uri($"http://localhost:{port}"));
        }

        [Fact]
        public async void GetAllProducts()
        {
            // Arange
            pact.UponReceiving("A valid request for all products")
                    .Given("products exist")
                    .WithRequest(HttpMethod.Get, "/api/products")
                .WillRespond()
                    .WithStatus(HttpStatusCode.OK)
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithJsonBody(new TypeMatcher(products));

            await pact.VerifyAsync(async ctx =>
            {
                var response = await ApiClient.GetAllProducts();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            });
        }

        [Fact]
        public async void GetProduct()
        {
            // Arange
            pact.UponReceiving("A valid request for a product")
                    .Given("product with ID 10 exists")
                    .WithRequest(HttpMethod.Get, "/api/products/10")
                .WillRespond()
                    .WithStatus(HttpStatusCode.OK)
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithJsonBody(new TypeMatcher(products[1]));

            await pact.VerifyAsync(async ctx =>
            {
                var response = await ApiClient.GetProduct(10);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            });
        }

        [Fact]
        public async void CreateProduct()
        {
            var a = new { type = "Electronics", name = "Mobile", price = "$ 300" };
            var b = new { id = 3, type = "Electronics", name = "Mobile", abc = "$ 300" };
            // Arange
            pact.UponReceiving("A valid request to create a product")
                    .Given("product creation")
                    .WithHeader("test","test")
                    .WithRequest(HttpMethod.Post, "/api/products")
                    .WithJsonBody(a)
                .WillRespond()
                    .WithStatus(HttpStatusCode.OK)
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithJsonBody(new TypeMatcher(b));
            
            await pact.VerifyAsync(async ctx =>
            {
                var response = await ApiClient.CreateProduct(0);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            });
        }

    }
}