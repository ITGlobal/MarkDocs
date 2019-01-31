using System;
using System.Collections.Generic;
using ITGlobal.MarkDocs.Extensions;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     MarkDocs documentation service. Provides access to the documentation data.
    /// </summary>
    [PublicAPI]
    public interface IMarkDocService : IDisposable
    {
        /// <summary>
        ///     Gets a list of all available documentations
        /// </summary>
        [NotNull]
        IReadOnlyList<IDocumentation> Documentations { get; }

        /// <summary>
        ///     Gets a documentation by its ID
        /// </summary>
        /// <param name="documentationId">
        ///     Documentation ID
        /// </param>
        /// <returns>
        ///     A documentation if exists, null otherwise
        /// </returns>
        [CanBeNull]
        IDocumentation GetDocumentation([NotNull] string documentationId);
        
        /// <summary>
        ///     Fetches all documentation branches. 
        ///     Existing branches are updated, new branches are checked out.
        ///     Any local changes are discarded.
        /// </summary>
        void Synchronize();

        /// <summary>
        ///     Gets an instance of an extension
        /// </summary>
        /// <typeparam name="TExtension">
        ///     Extension type
        /// </typeparam>
        /// <returns>
        ///     An instance of an extension or null if no such extension is registered.
        /// </returns>
        [NotNull]
        TExtension GetExtension<TExtension>()
            where TExtension : class, IExtension;
    }
}