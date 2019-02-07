using System;
using System.Threading;

namespace ITGlobal.MarkDocs.Format.Impl
{
    internal static class MarkdownPageReadContext
    {
        public static bool IsPresent => CurrentState.Value != null;

        public static IPageReadContext Current => Get(_ => _);

        public struct ScopeToken : IDisposable
        {
            public void Dispose() => ClearContext();
        }

        public static ScopeToken Use(IPageReadContext context)
        {
            CurrentState.Value = context;
            return new ScopeToken();
        }

        private static void ClearContext() => CurrentState.Value = null;

        private static readonly ThreadLocal<IPageReadContext> CurrentState = new ThreadLocal<IPageReadContext>();

        private static T Get<T>(Func<IPageReadContext, T> f)
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