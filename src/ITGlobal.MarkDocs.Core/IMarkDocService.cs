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
        ///     Initializes documentation data (foreground)
        /// </summary>
        [PublicAPI]
        void Initialize();

        /// <summary>
        ///     Gets a list of all available documentatios
        /// </summary>
        [PublicAPI, NotNull]
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
        [PublicAPI, CanBeNull]
        IDocumentation GetDocumentation([NotNull] string documentationId);

        /// <summary>
        ///     Checks out latest revision of specified documentation branch.
        ///     Any local changes are discarded.
        /// </summary>
        /// <param name="documentationId">
        ///     Documentation ID
        /// </param>
        [PublicAPI]
        void RefreshDocumentation([NotNull] string documentationId);

        /// <summary>
        ///     Fetches all documentation branches. 
        ///     Existing branches are updated, new branches are checked out as 'private' documentation.
        ///     Any local changes are discarded.
        /// </summary>
        [PublicAPI]
        void RefreshAllDocumentations();

        /// <summary>
        ///     Gets an instance of an extension
        /// </summary>
        /// <typeparam name="TExtension">
        ///     Extension type
        /// </typeparam>
        /// <returns>
        ///     An instance of an extension or null if no such extension is registered.
        /// </returns>
        [PublicAPI, CanBeNull]
        TExtension GetExtension<TExtension>()
            where TExtension : class, IExtension;
    }
}