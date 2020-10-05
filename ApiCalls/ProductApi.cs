using Invoice.DB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Invoice.ApiCalls
{
    public class ProductApi
    {
        static readonly HttpClient httpClient = new HttpClient();
        static readonly string apiServer = "http://localhost:54431/";
        
        public static IQueryable<product> GetProductList(string category)
        {
            var requestUri = apiServer + "InvoiceApi/api/ProductsSync";
            if (!String.IsNullOrWhiteSpace(category))
            {
                requestUri += $"?category={category}";
            }

            var response = httpClient.GetStringAsync(requestUri).Result;
            if (response == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<List<product>>(response).AsQueryable();

        }

        public static void GetProducts()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:54431/");

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            // List data response.
           // HttpResponseMessage response = client.GetAsync(urlParameters).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
            HttpResponseMessage response = client.GetAsync("api/ProductsSync").Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body.
                var aa = response.Content.ReadAsStringAsync();
                //var dataObjects = response.Content.ReadAsAsync<IEnumerable<DataObject>>().Result;  //Make sure to add a reference to System.Net.Http.Formatting.dll
                //foreach (var d in dataObjects)
                //{
                //    Console.WriteLine("{0}", d.Name);
                //}
            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            //Make any other calls using HttpClient here.

            //Dispose once all HttpClient calls are complete. This is not necessary if the containing object will be disposed of; for example in this case the HttpClient instance will be disposed automatically when the application terminates so the following call is superfluous.
            client.Dispose();
        }
    }
}
