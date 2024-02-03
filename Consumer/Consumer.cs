using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Consumer
{
    public class Consumer
    {
        private readonly Uri BaseUri;

        public Consumer(Uri baseUri)
        {
            this.BaseUri = baseUri;
        }

        public async Task<HttpResponseMessage> GetAllProducts()
        {
            using (var client = new HttpClient { BaseAddress = BaseUri })
            {
                try
                {
                    var response = await client.GetAsync($"/api/products");
                    return response;
                }
                catch (Exception ex)
                {
                    throw new Exception("There was a problem connecting to Provider API.", ex);
                }
            }
        }

        public async Task<HttpResponseMessage> GetProduct(int id)
        {
            using (var client = new HttpClient { BaseAddress = BaseUri })
            {
                try
                {
                    var response = await client.GetAsync($"/api/products/{id}");
                    return response;
                }
                catch (Exception ex)
                {
                    throw new Exception("There was a problem connecting to Provider API.", ex);
                }
            }
        }

        public async Task<HttpResponseMessage> CreateProduct(int a)
        {
            using (var client = new HttpClient { BaseAddress = BaseUri })
            {
                try
                {
                  

                    //client.DefaultRequestHeaders.Add("test", "test");
                    
                    var response = await client.PostAsJsonAsync($"/api/products", new { type = "Electronics", name = "Mobile", price = "$ 300" });
                    if (a == 0)
                        throw new Exception("");
                    return response;
                }
                catch (Exception ex)
                {
                    throw new Exception("There was a problem connecting to Provider API.", ex);
                }
            }
        }
    }
}
