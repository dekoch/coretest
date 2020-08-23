using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Shared;

namespace App
{

    public class UI
    {
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(AppPath.GetStaticPath()),
                RequestPath = "/static"
            });

            (byte[] ret, Error err) = Shared.File.Read(AppPath.GetAppPath() + "/data/ui/index.html");


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync(Encoding.UTF8.GetString(ret, 0, ret.Length));
                });
            });
        }
    }
}