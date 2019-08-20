using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace ITGlobal.MarkDocs.Site.Middlewares
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
                var service = _services.GetRequiredService<IMarkDocService>();
                if (service.Documentations.Count == 0)
                {
                    service.Synchronize();
                }
                Log.Information("Documentation is ready, {N} branch(es) are available", service.Documentations.Count);
            }
            catch (Exception e)
            {
                Log.Error(e, "Unable to initialize documentation");
                throw;
            }
        }
    }
}
