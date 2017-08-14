using System;
using System.Threading;

namespace ITGlobal.MarkDocs.Format
{
    internal static class MarkdownParseContext
    {
        public static bool IsPresent => CurrentState.Value != null;

        public static IParseContext ParseContext => Get(_ => _.ParseContext);

        private sealed class State
        {
            public IParseContext ParseContext { get; set; }
        }

        public struct ScopeToken : IDisposable
        {
            public void Dispose() => ClearCurrentRenderingContext();
        }

        public static ScopeToken SetCurrentParseContext(IParseContext parseContext)
        {
            CurrentState.Value = new State
            {
                ParseContext = parseContext
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
                throw new InvalidOperationException("No active MarkdownParseContext");
            }

            return f(state);
        }
    }
}