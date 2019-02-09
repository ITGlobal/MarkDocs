using ITGlobal.MarkDocs.Source;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     A listener for MarkDocs lifetime events
    /// </summary>
    [PublicAPI]
    public abstract class MarkDocsEventListener
    {
        /// <summary>
        ///     This method is called when MarkDocs starts to compile documentation <paramref name="id"/>
        /// </summary>
        public abstract CompilationEventListener CompilationStarted(string id);

        /// <summary>
        ///     This method is called when MarkDocs detects source changes
        /// </summary>
        public virtual void SourceChanged(ISourceTree sourceTree) { }

        /// <summary>
        ///     This method is called when MarkDocs detects source changes
        /// </summary>
        public virtual void SourceChanged() { }
    }
}