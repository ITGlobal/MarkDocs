using System;
using System.Reflection;

namespace ITGlobal.MarkDocs.Format
{
    [AttributeUsage(AttributeTargets.Field)]
    internal sealed class ResourceAttribute : Attribute
    {
        public ResourceAttribute(string id)
        {
            Id = id;
        }

        public string Id { get; }

        public static string GetId<T>(T enumField)
        {
            var type = typeof(T);
            var memInfo = type.GetMember(enumField.ToString());
            var attribute = memInfo[0].GetCustomAttribute<ResourceAttribute>();
            return attribute.Id;
        }
    }
}