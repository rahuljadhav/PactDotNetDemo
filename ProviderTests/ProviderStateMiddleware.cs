﻿using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PactNet;
using Provider.Model;
using Provider.Repositories;
using System.Net;
using System.Text;

namespace ProviderTests
{
    public class ProviderStateMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IProductRepository _repository;
        private readonly IDictionary<string, Action> _providerStates;

        public ProviderStateMiddleware(RequestDelegate next, IProductRepository repository)
        {
            _next = next;
            _repository = repository;
            _providerStates = new Dictionary<string, Action>
            {
                { "products exist", ProductsExist},
                { "product with ID 10 exists", Product10Exists }
            };
        }

        private void ProductsExist()
        {
            List<Product> products = new List<Product>()
            {
                new Product(9, "GEM Visa", "CREDIT_CARD", "v2"),
                new Product(10, "28 Degrees", "CREDIT_CARD", "v1")
            };

            _repository.SetState(products);
        }

        private void Product10Exists()
        {
            List<Product> products = new List<Product>()
            {
                new Product(10, "28 Degrees", "CREDIT_CARD", "v1")
            };

            _repository.SetState(products);
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/provider-states"))
            {
                await this.HandleProviderStatesRequest(context);
                await context.Response.WriteAsync(String.Empty);
            }
            else
            {
                await this._next(context);
            }
        }

        private async Task HandleProviderStatesRequest(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.OK;

            if (context.Request.Method.ToUpper() == HttpMethod.Post.ToString().ToUpper() &&
                context.Request.Body != null)
            {
                string jsonRequestBody = String.Empty;
                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
                {
                    jsonRequestBody = await reader.ReadToEndAsync();
                }

                var providerState = JsonConvert.DeserializeObject<ProviderState>(jsonRequestBody);

                //A null or empty provider state key must be handled
                if (providerState != null && !String.IsNullOrEmpty(providerState.State))
                {
                    _providerStates[providerState.State].Invoke();
                }
            }
        }
    }
}