using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Steel.Career.Accounts;
using Steel.Career.Careers;
using Steel.Career.Http;
using Steel.Services;

namespace Steel.Career
{
    /// <summary>The career server: dotnet run -- <market database> <game Resources folder> [port].
    /// With "generate <folder>" it writes the phone's RemoteGameService instead.</summary>
    public static class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length == 2 && args[0] == "generate") { Generate.RemoteServiceWriter.Write(args[1]); return 0; }
            if (args.Length < 2) { Console.Error.WriteLine("usage: Steel.Career <market.db> <Resources folder> [port]"); return 2; }
            WebApplication app = Build(args[0], args[1], args.Length > 2 ? int.Parse(args[2]) : 8766,
                () => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            app.StartAsync().GetAwaiter().GetResult();
            // port 0 lets the system choose; whoever started the server reads the address from this line
            Console.WriteLine("career " + string.Join(" ", app.Urls));
            app.WaitForShutdownAsync().GetAwaiter().GetResult();
            return 0;
        }

        public static WebApplication Build(string database, string resources, int port, Func<long> nowMs)
        {
            UnityEngine.Resources.Root = resources;
            var accounts = new AccountBook(database);
            var careers = new CareerBook(new ResourceConfigSource(), new CareerStore(accounts), nowMs);
            WebApplicationBuilder builder = WebApplication.CreateBuilder();
            builder.Logging.SetMinimumLevel(LogLevel.Warning);
            builder.WebHost.UseUrls("http://127.0.0.1:" + port);
            WebApplication app = builder.Build();
            CareerRoutes.Map(app, accounts, careers);
            return app;
        }
    }
}
