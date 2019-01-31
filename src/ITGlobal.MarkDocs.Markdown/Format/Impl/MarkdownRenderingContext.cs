using System;
using System.Threading;

namespace ITGlobal.MarkDocs.Format.Impl
{
    internal static class MarkdownRenderingContext
    {
        public static bool IsPresent => CurrentState.Value != null;

        public static IPageRenderContext RenderContext => Get(_ => _.RenderContext);
        public static IResourceUrlResolver ResourceUrlResolver => Get(_ => _.ResourceUrlResolver);

        private sealed class State
        {
            public IPageRenderContext RenderContext { get; set; }
            public IResourceUrlResolver ResourceUrlResolver { get; set; }
        }

        public struct ScopeToken : IDisposable
        {
            public void Dispose() => ClearCurrentRenderingContext();
        }
        
        public static ScopeToken SetCurrentRenderingContext(
            IPageRenderContext renderContext,
            IResourceUrlResolver resourceUrlResolver
        )
        {
            CurrentState.Value = new State
            {
                RenderContext = renderContext,
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