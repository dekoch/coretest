using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Shared;

namespace App
{

    public class API
    {
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            APIInfo.Configure(app, env);
            APIDB.Configure(app, env);
            APIPLC.Configure(app, env);
        }
    }
}