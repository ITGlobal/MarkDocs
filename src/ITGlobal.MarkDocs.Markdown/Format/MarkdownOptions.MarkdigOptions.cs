using JetBrains.Annotations;
using Markdig.Extensions.JiraLinks;

namespace ITGlobal.MarkDocs.Format
{
    partial class MarkdownOptions
    {
        private bool _useAbbreviations = true;

        /// <summary>
        ///     Enable abbreviations extension
        /// </summary>
        [NotNull]
        public MarkdownOptions UseAbbreviations(bool use = true)
        {
            _useAbbreviations = use;
            return this;
        }


        private bool _useAutoIdentifiers = true;

        /// <summary>
        ///     Enable automatic identifiers extension
        /// </summary>
        [NotNull]
        public MarkdownOptions UseAutoIdentifiers(bool use = true)
        {
            _useAutoIdentifiers = use;
            return this;
        }


        private bool _useCitations = true;

        /// <summary>
        ///     Enable citations extension
        /// </summary>
        [NotNull]
        public MarkdownOptions UseCitations(bool use = true)
        {
            _useCitations = use;
            return this;
        }


        private bool _useCustomContainers = true;

        /// <summary>
        ///     Enable custom container extension
        /// </summary>
        [NotNull]
        public MarkdownOptions UseCustomContainers(bool use = true)
        {
            _useCustomContainers = use;
            return this;
        }


        private bool _useDefinitionLists = true;

        /// <summary>
        ///     Enable definition lists extension
        /// </summary>
        [NotNull]
        public MarkdownOptions UseDefinitionLists(bool use = true)
        {
            _useDefinitionLists = use;
            return this;
        }


        private bool _useEmphasisExtras = true;

        /// <summary>
        ///     Enable extra emphasis extension
        /// </summary>
        [NotNull]
        public MarkdownOptions UseEmphasisExtras(bool use = true)
        {
            _useEmphasisExtras = use;
            return this;
        }


        private bool _useGridTables = true;

        /// <summary>
        ///     Enable grid tables extension
        /// </summary>
        [NotNull]
        public MarkdownOptions UseGridTables(bool use = true)
        {
            _useGridTables = use;
            return this;
        }


        private bool _useHtmlAttributes = true;

        /// <summary>
        ///     Enable HTML attributes extension
        /// </summary>
        [NotNull]
        public MarkdownOptions UseHtmlAttributes(bool use = true)
        {
            _useHtmlAttributes = use;
            return this;
        }


        private bool _useFigures = true;

        /// <summary>
        ///     Enable figures extension
        /// </summary>
        [NotNull]
        public MarkdownOptions UseFigures(bool use = true)
        {
            _useFigures = use;
            return this;
        }


        private bool _useFooters = true;

        /// <summary>
        ///     Enable footer extension
        /// </summary>
        [NotNull]
        public MarkdownOptions UseFooters(bool use = true)
        {
            _useFooters = use;
            return this;
        }


        private bool _useFootnotes = true;

        /// <summary>
        ///     Enable footnotes extension
        /// </summary>
        [NotNull]
        public MarkdownOptions UseFootnotes(bool use = true)
        {
            _useFootnotes = use;
            return this;
        }


        private bool _useMediaLinks = true;

        /// <summary>
        ///     Enable media links extension
        /// </summary>
        [NotNull]
        public MarkdownOptions UseMediaLinks(bool use = true)
        {
            _useMediaLinks = use;
            return this;
        }


        private bool _usePipeTables = true;

        /// <summary>
        ///     Enable pipe table extension
        /// </summary>
        [NotNull]
        public MarkdownOptions UsePipeTables(bool use = true)
        {
            _usePipeTables = use;
            return this;
        }


        private bool _useListExtras = true;

        /// <summary>
        ///     Enable extra list extension
        /// </summary>
        [NotNull]
        public MarkdownOptions UseListExtras(bool use = true)
        {
            _useListExtras = use;
            return this;
        }


        private bool _useTaskLists = true;

        /// <summary>
        ///     Enable task list extension
        /// </summary>
        [NotNull]
        public MarkdownOptions UseTaskLists(bool use = true)
        {
            _useTaskLists = use;
            return this;
        }


        private bool _useBootstrap = true;

        /// <summary>
        ///     Enable Bootstap styles
        /// </summary>
        [NotNull]
        public MarkdownOptions UseBootstrap(bool use = true)
        {
            _useBootstrap = use;
            return this;
        }


        private bool _useEmojiAndSmiley = true;

        /// <summary>
        ///     Enable emoji and smiley support
        /// </summary>
        [NotNull]
        public MarkdownOptions UseEmojiAndSmiley(bool use = true)
        {
            _useEmojiAndSmiley = use;
            return this;
        }


        private bool _useSmartyPants = true;

        /// <summary>
        ///     Enable smarty-pants extension
        /// </summary>
        [NotNull]
        public MarkdownOptions UseSmartyPants(bool use = true)
        {
            _useSmartyPants = use;
            return this;
        }


        private bool _useIcons = true;

        /// <summary>
        ///     Enable font-awesome icons extension
        /// </summary>
        [NotNull]
        public MarkdownOptions UseIcons(bool use = true)
        {
            _useIcons = use;
            return this;
        }


        private bool _dontRenderFirstHeading = true;

        /// <summary>
        ///     Disable rendering of the first heading in the document
        /// </summary>
        [NotNull]
        public MarkdownOptions DontRenderFirstHeading(bool use = true)
        {
            _dontRenderFirstHeading = use;
            return this;
        }


        private JiraLinkOptions _jiraLinkOptions;

        /// <summary>
        ///     Enable JIRA links
        /// </summary>
        [NotNull]
        public MarkdownOptions UseJiraLinks(JiraLinkOptions options)
        {
            _jiraLinkOptions = options;
            return this;
        }


        private bool _useAdmonition = true;

        /// <summary>
        ///     Enable admonition extension
        /// </summary>
        [NotNull]
        public MarkdownOptions UseAdmonition(bool use = true)
        {
            _useAdmonition = use;
            return this;
        }


        private bool _useAlerts = true;

        /// <summary>
        ///     Enable alert extension
        /// </summary>
        [NotNull]
        public MarkdownOptions UseAlerts(bool use = true)
        {
            _useAlerts = use;
            return this;
        }


        private bool _useHtmlIncludes = true;

        /// <summary>
        ///     Enable HTML include extension
        /// </summary>
        [NotNull]
        public MarkdownOptions UseHtmlIncludes(bool use = true)
        {
            _useHtmlIncludes = use;
            return this;
        }
    }
}
