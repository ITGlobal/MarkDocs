using System;
using System.Threading;

namespace ITGlobal.MarkDocs.Format
{
    internal static class MarkdownRenderingContext
    {
        public static bool IsPresent => CurrentState.Value != null;

        public static IRenderContext RenderContext => Get(_ => _.RenderContext);
        public static IUmlRenderer UmlRenderer => Get(_ => _.UmlRenderer);
        public static IMathRenderer MathRenderer => Get(_ => _.MathRenderer);
        public static ITocRenderer TocRenderer => Get(_ => _.TocRenderer);
        public static IChildrenListRenderer ChildrenListRenderer => Get(_ => _.ChildrenListRenderer);
        public static IResourceUrlResolver ResourceUrlResolver => Get(_ => _.ResourceUrlResolver);

        private sealed class State
        {
            public IRenderContext RenderContext { get; set; }
            public IUmlRenderer UmlRenderer { get; set; }
            public IMathRenderer MathRenderer { get; set; }
            public ITocRenderer TocRenderer { get; set; }
            public IChildrenListRenderer ChildrenListRenderer { get; set; }
            public IResourceUrlResolver ResourceUrlResolver { get; set; }
        }

        public struct ScopeToken : IDisposable
        {
            public void Dispose() => ClearCurrentRenderingContext();
        }


        public static ScopeToken SetCurrentRenderingContext(
            IRenderContext renderContext,
            MarkdownOptions options,
            IResourceUrlResolver resourceUrlResolver
        )
        {
            CurrentState.Value = new State
            {
                RenderContext = renderContext,
                UmlRenderer = options.UmlRenderer,
                MathRenderer = options.MathRenderer,
                TocRenderer = options.TocRenderer,
                ChildrenListRenderer = options.ChildrenListRenderer,
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