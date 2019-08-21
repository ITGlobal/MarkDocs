using System;
using System.Threading.Tasks;
using ITGlobal.CommandLine;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace ITGlobal.MarkDocs.Tools.Serve.Middlewares
{
    public sealed class InitializeMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _services;

        private readonly object _stateLock = new object();
        private Task _task;

        public InitializeMiddleware(RequestDelegate next, IServiceProvider services)
        {
            _next = next;
            _services = services;
        }

        [UsedImplicitly]
        public async Task Invoke(HttpContext context)
        {
            Task task;
            lock (_stateLock)
            {
                if (_task == null)
                {
                    _task = Task.Factory.StartNew(InitializeImpl);
                }

                task = _task;
            }

            try
            {
                await task;
            }
            catch
            {
                context.Response.StatusCode = 502;
                await context.Response.WriteAsync("Service Unavailable");
                return;
            }

            await _next(context);
        }

        private void InitializeImpl()
        {
            try
            {
                if (!Startup.Config.Verbose)
                {
                    Console.Error.WriteLine("Initializing...");
                }

                var service = _services.GetRequiredService<IMarkDocService>();
                if (service.Documentations.Count == 0)
                {
                    service.Synchronize();
                }
                Log.Information("Documentation is ready, {N} branch(es) are available", service.Documentations.Count);

                if (!Startup.Config.Verbose)
                {
                    Console.Error.WriteLine($"Documentation site is ready! Open {Startup.Config.PublicUrl.Cyan()} to view it.");
                    Console.Error.WriteLine($"Press {"<Ctrl+C>".Cyan()} to exit");
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Unable to initialize documentation");
                Environment.ExitCode = -1;
                var lifetime = _services.GetRequiredService<IApplicationLifetime>();
                lifetime.StopApplication();

                throw;
            }
        }
    }
}
