using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     Options for markdown format
    /// </summary>
    [PublicAPI]
    public sealed class MarkdownOptions
    {
        /// <summary>
        ///     Resource URL resolver
        /// </summary>
        [PublicAPI]
        public IResourceUrlResolver ResourceUrlResolver { get; set; } = new DefaultResourceUrlResolver();

        /// <summary>
        ///     A syntax colorizer
        /// </summary>
        [PublicAPI]
        public ISyntaxColorizer SyntaxColorizer { get; set; } = new ClientHighlightJsSyntaxColorizer();

        /// <summary>
        ///     A PlantUML renderer
        /// </summary>
        [PublicAPI]
        public IUmlRenderer UmlRenderer { get; set; } = new PlantUmlWebServiceRenderer();

        /// <summary>
        ///     A MathML/Tex/LaTex renderer
        /// </summary>
        [PublicAPI]
        public IMathRenderer MathRenderer { get; set; } = new CodecogsMathRenderer();

        /// <summary>
        ///     A TOC renderer
        /// </summary>
        [PublicAPI]
        public ITocRenderer TocRenderer { get; set; }

        /// <summary>
        ///     ptions for rendering markdown into HTML
        /// </summary>
        [PublicAPI]
        public MarkdownRenderingOptions Rendering { get; } = new MarkdownRenderingOptions();

        /// <summary>
        ///     Enable abbreviations extension
        /// </summary>
        [PublicAPI]
        public bool UseAbbreviations { get; set; } = true;

        /// <summary>
        ///     Enable automatic identifiers extension
        /// </summary>
        [PublicAPI]
        public bool UseAutoIdentifiers { get; set; } = true;

        /// <summary>
        ///     Enable citations extension
        /// </summary>
        [PublicAPI]
        public bool UseCitations { get; set; } = true;

        /// <summary>
        ///     Enable custom container extension
        /// </summary>
        [PublicAPI]
        public bool UseCustomContainers { get; set; } = true;

        /// <summary>
        ///     Enable definition lists extension
        /// </summary>
        [PublicAPI]
        public bool UseDefinitionLists { get; set; } = true;

        /// <summary>
        ///     Enable extra emphasis extension
        /// </summary>
        [PublicAPI]
        public bool UseEmphasisExtras { get; set; } = true;

        /// <summary>
        ///     Enable grid tables extension
        /// </summary>
        [PublicAPI]
        public bool UseGridTables { get; set; } = true;

        /// <summary>
        ///     Enable HTML attributes extension
        /// </summary>
        [PublicAPI]
        public bool UseGenericAttributes { get; set; } = true;

        /// <summary>
        ///     Enable figures extension
        /// </summary>
        [PublicAPI]
        public bool UseFigures { get; set; } = true;

        /// <summary>
        ///     Enable footer extension
        /// </summary>
        [PublicAPI]
        public bool UseFooters { get; set; } = true;

        /// <summary>
        ///     Enable footnotes extension
        /// </summary>
        [PublicAPI]
        public bool UseFootnotes { get; set; } = true;

        /// <summary>
        ///     Enable media links extension
        /// </summary>
        [PublicAPI]
        public bool UseMediaLinks { get; set; } = true;

        /// <summary>
        ///     Enable pipe table extension
        /// </summary>
        [PublicAPI]
        public bool UsePipeTables { get; set; } = true;

        /// <summary>
        ///     Enable extra list extension
        /// </summary>
        [PublicAPI]
        public bool UseListExtras { get; set; } = true;

        /// <summary>
        ///     Enable task list extension
        /// </summary>
        [PublicAPI]
        public bool UseTaskLists { get; set; } = true;

        /// <summary>
        ///     Enable Bootstap styles
        /// </summary>
        [PublicAPI]
        public bool UseBootstrap { get; set; } = true;

        /// <summary>
        ///     Enable emoji and smiley support
        /// </summary>
        [PublicAPI]
        public bool UseEmojiAndSmiley { get; set; } = true;

        /// <summary>
        ///     Enable smarty-pants extension
        /// </summary>
        [PublicAPI]
        public bool UseSmartyPants { get; set; } = true;

        /// <summary>
        ///     Enable font-awesome icons extension
        /// </summary>
        [PublicAPI]
        public bool UseIcons { get; set; } = true;
    }
}