using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Shared;

namespace App
{
    public class APIDB
    {

        public class Reqest
        {
            public double Version { get; set; }
        }

        public class Response
        {
            public double Version { get; set; }
            public string DB { get; set; }
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/api/db/", async context =>
                {
                    await context.Response.WriteAsync(getResp(context));
                });
            });
        }

        private static string getResp(HttpContext context)
        {
            //(byte[] ret, Error err) = Shared.File.Read(AppPath.GetAppPath() + "/data/ui/index.html");

            Response resp = new Response();
            resp.Version = 1.0;

            SQLite db = new SQLite();


            string par1 = context.Request.Query["par1"];
            string par2 = context.Request.Query["par2"];

            if (par1 == "1")
            {
                db.CreateTable("", AppPath.GetAppPath() + "/data/db.sqlite3");
            }
            else if (par1 == "2")
            {
                db.WriteTable(par2, AppPath.GetAppPath() + "/data/db.sqlite3");
            }
            else if (par1 == "3")
            {
             resp.DB = db.ReadTable("", AppPath.GetAppPath() + "/data/db.sqlite3");   
            }

            byte[] b = JsonSerializer.SerializeToUtf8Bytes(resp);
            return Encoding.UTF8.GetString(b, 0, b.Length);
        }
    }
}