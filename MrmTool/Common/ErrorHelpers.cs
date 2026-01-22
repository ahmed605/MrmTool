using System.Diagnostics;
using System.Runtime.CompilerServices;

using TerraFX.Interop.Windows;
using static TerraFX.Interop.Windows.Windows;

namespace MrmTool.Common
{
    internal static unsafe class ErrorHelpers
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough, DebuggerHidden, StackTraceHidden]
        internal static void ThrowIfWin32Error(uint value, [CallerArgumentExpression(nameof(value))] string? valueExpression = null)
        {
            if (value is not 0)
            {
                ThrowExternalException(valueExpression ?? "Method", HRESULT_FROM_WIN32(value));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough, DebuggerHidden, StackTraceHidden]
        internal static void ThrowLastErrorIfNull(void* value, [CallerArgumentExpression(nameof(value))] string? valueExpression = null)
        {
            if (value is null)
            {
                ThrowExternalException(valueExpression ?? "Method", HRESULT_FROM_WIN32(GetLastError()));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough, DebuggerHidden, StackTraceHidden]
        internal static void ThrowLastErrorIfNull(nint value, [CallerArgumentExpression(nameof(value))] string? valueExpression = null)
        {
            if (value is 0)
            {
                ThrowExternalException(valueExpression ?? "Method", HRESULT_FROM_WIN32(GetLastError()));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough, DebuggerHidden, StackTraceHidden]
        internal static void ThrowLastErrorIfNull(nuint value, [CallerArgumentExpression(nameof(value))] string? valueExpression = null)
        {
            if (value is 0)
            {
                ThrowExternalException(valueExpression ?? "Method", HRESULT_FROM_WIN32(GetLastError()));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough, DebuggerHidden, StackTraceHidden]
        internal static void ThrowLastErrorIfNull(ulong value, [CallerArgumentExpression(nameof(value))] string? valueExpression = null)
        {
            if (value is 0)
            {
                ThrowExternalException(valueExpression ?? "Method", HRESULT_FROM_WIN32(GetLastError()));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough, DebuggerHidden, StackTraceHidden]
        internal static void ThrowLastErrorIfNull(uint value, [CallerArgumentExpression(nameof(value))] string? valueExpression = null)
        {
            if (value is 0)
            {
                ThrowExternalException(valueExpression ?? "Method", HRESULT_FROM_WIN32(GetLastError()));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough, DebuggerHidden, StackTraceHidden]
        internal static void ThrowLastErrorIfNull(ushort value, [CallerArgumentExpression(nameof(value))] string? valueExpression = null)
        {
            if (value is 0)
            {
                ThrowExternalException(valueExpression ?? "Method", HRESULT_FROM_WIN32(GetLastError()));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough, DebuggerHidden, StackTraceHidden]
        internal static void ThrowLastErrorIfDefault<T>(T value, [CallerArgumentExpression(nameof(value))] string? valueExpression = null) where T : IEquatable<T>
        {
            if (value.Equals(default))
            {
                ThrowExternalException(valueExpression ?? "Method", HRESULT_FROM_WIN32(GetLastError()));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough, DebuggerHidden, StackTraceHidden]
        internal static void ThrowLastErrorIfFalse(bool value, [CallerArgumentExpression(nameof(value))] string? valueExpression = null)
        {
            if (value is false)
            {
                ThrowExternalException(valueExpression ?? "Method", HRESULT_FROM_WIN32(GetLastError()));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough, DebuggerHidden, StackTraceHidden]
        internal static void ThrowLastErrorIfFalse(BOOL value, [CallerArgumentExpression(nameof(value))] string? valueExpression = null)
        {
            if (value.Value is 0)
            {
                ThrowExternalException(valueExpression ?? "Method", HRESULT_FROM_WIN32(GetLastError()));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough, DebuggerHidden, StackTraceHidden]
        internal static void LOG_LAST_ERROR_IF(bool value, [CallerArgumentExpression(nameof(value))] string? valueExpression = null)
        {
            if (value is true)
            {
                Debug.WriteLine($"LOG_LAST_ERROR_IF: 0x{HRESULT_FROM_WIN32(GetLastError())} [{valueExpression ?? "Method"}]");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough, DebuggerHidden, StackTraceHidden]
        internal static void LOG_LAST_ERROR_IF(BOOL value, [CallerArgumentExpression(nameof(value))] string? valueExpression = null)
        {
            if (value.Value is not 0)
            {
                Debug.WriteLine($"LOG_LAST_ERROR_IF: 0x{HRESULT_FROM_WIN32(GetLastError())} [{valueExpression ?? "Method"}]");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough, DebuggerHidden, StackTraceHidden]
        public static HRESULT LOG_IF_FAILED(HRESULT value, [CallerArgumentExpression(nameof(value))] string? valueExpression = null)
        {
            if (value.FAILED)
            {
                Debug.WriteLine($"LOG_IF_FAILED: 0x{value} [{valueExpression ?? "Method"}]");
            }

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough, DebuggerHidden, StackTraceHidden]
        public static bool SUCCEEDED_LOG(HRESULT value, [CallerArgumentExpression(nameof(value))] string? valueExpression = null)
        {
            if (value.FAILED)
            {
                Debug.WriteLine($"SUCCEEDED_LOG: 0x{value} [{valueExpression ?? "Method"}]");
                return false;
            }

            return true;
        }
    }
}
