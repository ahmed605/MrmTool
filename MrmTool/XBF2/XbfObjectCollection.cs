#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XbfAnalyzer.Xbf
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CsWinRT1028:Class is not marked partial", Justification = "<Pending>")]
    public class XbfObjectCollection : List<XbfObject>
    {
        public override string ToString()
        {
            return ToString(0);
        }

        public string ToString(int indentLevel)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var obj in this)
                sb.AppendLine(obj.ToString(indentLevel));
            return sb.ToString();
        }
    }
}
