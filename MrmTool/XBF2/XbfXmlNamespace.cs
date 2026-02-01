using System;
using System.Collections.Generic;
using System.Text;

namespace MrmTool.XBF2
{
    public class XbfXmlNamespace
    {
        internal XbfXmlNamespace(string prefix, string @namespace)
        {
            Prefix = prefix;
            Namespace = @namespace;
        }

        internal XbfXmlNamespace(string @namespace)
        {
            Prefix = null;
            Namespace = @namespace;
        }

        public string? Prefix { get; set; }

        public string Namespace { get; set; }
    }
}
