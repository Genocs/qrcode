using Convey;
using Convey.Docs.Swagger;
using Convey.Logging;
using Convey.WebApi;
using Convey.WebApi.CQRS;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace Genocs.QRCodeLibrary.WebApi
{
    public class Program
    {
        public static Task Main(string[] args)
            => CreateHostBuilder(args).Build().RunAsync();

        public static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args)
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.ConfigureServices(services => services
                            .AddConvey()
//                            .AddErrorHandler<ExceptionToResponseMapper>()
                            .AddServices()
//                            .AddHttpClient()
                            .AddWebApi()
                            .AddSwaggerDocs()
                            .Build())
                            .Configure(app => app
                                .UseConvey()
//                                .UseErrorHandler()
                                .UseRouting()
                                .UseEndpoints(r => r.MapControllers())
                                .UseDispatcherEndpoints(endpoints => endpoints
                                    .Get("", ctx => ctx.Response.WriteAsync("QRCode Service. Go to ./docs to get informations."))
                                    .Get("ping", ctx => ctx.Response.WriteAsync("pong"))
                                )
                                .UseSwaggerDocs())
                            .UseLogging();
                    });

    }
}

