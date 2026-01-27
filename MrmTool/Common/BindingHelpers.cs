using MrmLib;
using System.Runtime.CompilerServices;

namespace MrmTool.Common
{
    internal static class BindingHelpers
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string FormatQualifier(string attributeName, double fallbackScore, QualifierOperator op, int priority, string value)
        {
            return $"{attributeName} {op.Symbol} {value}{(priority is { } p && p != 0 ? $", Priority = {p}" : string.Empty)}{(fallbackScore is { } s && s != 0 ? $", Fallback Score = {s}" : string.Empty)}";
        }
    }
}
