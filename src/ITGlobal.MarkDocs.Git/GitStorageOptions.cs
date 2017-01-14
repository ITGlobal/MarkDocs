using System;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Git
{
    /// <summary>
    ///     Options for git storage
    /// </summary>
    [PublicAPI]
    public sealed class GitStorageOptions
    {
        private string _fetchUrl;
        private string _userName;
        private string _password;

        /// <summary>
        ///     Defines fetch URL
        /// </summary>
        [PublicAPI, NotNull]
        public string FetchUrl
        {
            get { return _fetchUrl; }
            set
            {
                _fetchUrl = value;
                UpdateUrl();
            }
        }

        /// <summary>
        ///     Defines content directory root
        /// </summary>
        [PublicAPI, NotNull]
        public string Directory { get; set; }

        /// <summary>
        ///     A username for fetch URL
        /// </summary>
        [PublicAPI, CanBeNull]
        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                UpdateUrl();
            }
        }

        /// <summary>
        ///     A password for fetch UTL
        /// </summary>
        [PublicAPI, CanBeNull]
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                UpdateUrl();
            }
        }

        /// <summary>
        ///     Indicates whether git storage should use branches as a documentation version source
        /// </summary>
        [PublicAPI]
        public RefInclusionRules Branches { get; } = new RefInclusionRules { Use = true };

        /// <summary>
        ///     Indicates whether git storage should use tags as a documentation version source
        /// </summary>
        [PublicAPI]
        public RefInclusionRules Tags { get; } = new RefInclusionRules();


        /// <summary>
        ///     Actual URL with credentials
        /// </summary>
        internal string Url { get; private set; }
        
        /// <summary>
        ///     Enable periodical remote polling
        /// </summary>
        public bool EnablePolling { get; set; }

        /// <summary>
        ///     Remote polling timer interval
        /// </summary>
        public TimeSpan PollingInterval { get; set; } = TimeSpan.FromMinutes(5);
        
        private void UpdateUrl()
        {
            if (FetchUrl == null)
            {
                Url = null;
                return;
            }

            var builder = new UriBuilder(FetchUrl);

            if (!string.IsNullOrEmpty(UserName))
            {
                builder.UserName = Uri.EscapeDataString(UserName);
            }

            if (!string.IsNullOrEmpty(Password))
            {
                builder.Password = Uri.EscapeDataString(Password);
            }

            Url = builder.Uri.ToString();
        }
    }
}
