using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Shared;

namespace App
{
    public class APIInfo
    {
        public class Response
        {
            public double Version { get; set; }
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/api/info/", async context =>
                {
                    await context.Response.WriteAsync(getResp(context));
                });
            });
        }

        private static string getResp(HttpContext context)
        {
            //(byte[] ret, Error err) = Shared.File.Read(AppPath.GetAppPath() + "/data/ui/index.html");

            Response resp = new Response();
            resp.Version = 1.9;
        
            string par1 = context.Request.Query["par1"];

            byte[] b = { };

            if (par1 == "12")
            {
                b = JsonSerializer.SerializeToUtf8Bytes(resp);
            }
            else
            {

                b = JsonSerializer.SerializeToUtf8Bytes(resp);
            }

            return Encoding.UTF8.GetString(b, 0, b.Length);
        }
    }
}