using System;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace ITGlobal.MarkDocs.Format
{
    internal static class MarkdownRenderingContext
    {
        public static bool IsMarkdownRenderingContextPresent => CurrentState.Value != null;

        public static ILogger Logger => Get(_ => _.Logger);
        public static IRenderContext RenderContext => Get(_ => _.RenderContext);
        public static IUmlRenderer UmlRenderer => Get(_ => _.UmlRenderer);
        public static IMathRenderer MathRenderer => Get(_ => _.MathRenderer);
        public static IResourceUrlResolver ResourceUrlResolver => Get(_ => _.ResourceUrlResolver);

        private sealed class State
        {
            public ILogger Logger { get; set; }
            public IRenderContext RenderContext { get; set; }
            public IUmlRenderer UmlRenderer { get; set; }
            public IMathRenderer MathRenderer { get; set; }
            public IResourceUrlResolver ResourceUrlResolver { get; set; }
        }

        public struct ScopeToken : IDisposable
        {
            public void Dispose() => ClearCurrentRenderingContext();
        }


        public static ScopeToken SetCurrentRenderingContext(
            ILogger logger,
            IRenderContext renderContext,
            IUmlRenderer umlRenderer,
            IMathRenderer mathRenderer,
            IResourceUrlResolver resourceUrlResolver
        )
        {
            CurrentState.Value = new State
            {
                Logger = logger,
                RenderContext = renderContext,
                UmlRenderer = umlRenderer,
                MathRenderer = mathRenderer,
                ResourceUrlResolver = resourceUrlResolver
            };

            return new ScopeToken();
        }

        private static void ClearCurrentRenderingContext() => CurrentState.Value = null;

        private static readonly ThreadLocal<State> CurrentState = new ThreadLocal<State>();

        private static T Get<T>(Func<State, T> f)
        {
            var state = CurrentState.Value;
            if (state == null)
            {
                throw new InvalidOperationException("No active MarkdownRenderingContext");
            }

            return f(state);
        }
    }
}