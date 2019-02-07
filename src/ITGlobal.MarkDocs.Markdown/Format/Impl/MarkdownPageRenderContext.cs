﻿using System;
using System.Threading;

namespace ITGlobal.MarkDocs.Format.Impl
{
    internal static class MarkdownPageRenderContext
    {
        public static bool IsPresent => CurrentState.Value != null;

        public static IPageRenderContext Current => Get(_ => _);

        public struct ScopeToken : IDisposable
        {
            public void Dispose() => ClearContext();
        }

        public static ScopeToken Use(IPageRenderContext context)
        {
            CurrentState.Value = context;
            return new ScopeToken();
        }

        private static void ClearContext() => CurrentState.Value = null;

        private static readonly ThreadLocal<IPageRenderContext> CurrentState = new ThreadLocal<IPageRenderContext>();

        private static T Get<T>(Func<IPageRenderContext, T> f)
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