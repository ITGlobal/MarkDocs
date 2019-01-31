using System;
using ITGlobal.MarkDocs.Cache.Model;
using ITGlobal.MarkDocs.Source;

namespace ITGlobal.MarkDocs.Impl
{
    internal sealed class SourceInfo : ISourceInfo
    {
        private readonly SourceInfoModel _model;

        public SourceInfo(SourceInfoModel model)
        {
            _model = model;
        }

        public string SourceUrl => _model.SourceUrl;
        public string Version => _model.Version;
        public DateTime? LastChangeTime => _model.LastChangeTime;
        public string LastChangeId => _model.LastChangeId;
        public string LastChangeDescription => _model.LastChangeDescription;
        public string LastChangeAuthor => _model.LastChangeAuthor;
    }
}