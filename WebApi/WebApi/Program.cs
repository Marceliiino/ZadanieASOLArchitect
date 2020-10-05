using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using Microsoft.Owin.Hosting;
using Owin;
using System.Web.Http.Owin;

namespace WebApi
{
    public class Program
    {
        static void Main()
        {
            var baseAddress = "http://localhost:54431/InvoiceApi/";

            using (var app = WebApp.Start<Startup>(url: baseAddress))
            {
                Console.WriteLine();
                Console.WriteLine($"WebAPI Service ({baseAddress}) started.");
                Console.WriteLine("Press ENTER to stop the server and close app...");
                Console.ReadLine();
            }
        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);


            config.EnsureInitialized();
            appBuilder.UseWebApi(config);

        }
    }
}