using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace ITGlobal.MarkDocs.Site.Extensions
{
    public sealed class RazorViewRenderer
    {
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;

        private RazorViewRenderer(
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider)
        {
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        public static string Render<T>(IServiceProvider services, [AspMvcPartialView] string viewName, [AspMvcModelType] T model)
        {
            var html = RenderAsync(services, viewName, model).GetAwaiter().GetResult();
            return html;
        }

        public static async Task<string> RenderAsync<T>(IServiceProvider services, [AspMvcPartialView] string viewName, [AspMvcModelType] T model)
        {
            var renderer = new RazorViewRenderer(
                services.GetRequiredService<IRazorViewEngine>(),
                services.GetRequiredService<ITempDataProvider>(),
                services
            );
            var html = await renderer.RenderAsync(viewName, model);
            return html;
        }

        private async Task<string> RenderAsync<TModel>([AspMvcPartialView] string viewName, [AspMvcModelType] TModel model)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var httpContext = new DefaultHttpContext();
                httpContext.RequestServices = scope.ServiceProvider;
                var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

                var view = FindView(actionContext, viewName);

                using (var output = new StringWriter())
                {
                    var viewContext = new ViewContext(
                        actionContext,
                        view,
                        new ViewDataDictionary<TModel>(
                            metadataProvider: new EmptyModelMetadataProvider(),
                            modelState: new ModelStateDictionary())
                        {
                            Model = model
                        },
                        new TempDataDictionary(
                            actionContext.HttpContext,
                            _tempDataProvider),
                        output,
                        new HtmlHelperOptions()
                    );

                    await view.RenderAsync(viewContext);

                    return output.ToString();
                }
            }
        }

        private RazorView FindView(ActionContext actionContext, string viewName)
        {
            var getViewResult = _viewEngine.GetView(executingFilePath: null, viewPath: viewName, isMainPage: true);
            if (getViewResult.Success)
            {
                return (RazorView)getViewResult.View;
            }

            var findViewResult = _viewEngine.FindView(actionContext, viewName, isMainPage: true);
            if (findViewResult.Success)
            {
                return (RazorView)findViewResult.View;
            }

            var searchedLocations = getViewResult.SearchedLocations.Concat(findViewResult.SearchedLocations);
            var errorMessage = string.Join(
                Environment.NewLine,
                new[] { $"Unable to find view '{viewName}'. The following locations were searched:" }.Concat(searchedLocations)); ;

            throw new InvalidOperationException(errorMessage);
        }
    }
}