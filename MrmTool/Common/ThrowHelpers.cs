using System.Diagnostics.CodeAnalysis;

namespace MrmTool.Common
{
    internal static class ThrowHelpers
    {
        [DoesNotReturn]
        internal static void ThrowArgumentException(string message)
        {
            throw new ArgumentException(message);
        }

        [DoesNotReturn]
        internal static void ThrowArgumentException(string message, string paramName)
        {
            throw new ArgumentException(message, paramName);
        }

        [DoesNotReturn]
        internal static T ThrowArgumentException<T>(string message)
        {
            throw new ArgumentException(message);
        }

        [DoesNotReturn]
        internal static T ThrowArgumentException<T>(string message, string paramName)
        {
            throw new ArgumentException(message, paramName);
        }
    }
}
