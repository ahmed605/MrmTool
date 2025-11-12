using TerraFX.Interop.Windows;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Windows.ApplicationModel.Resources.Core;

using static TerraFX.Interop.Windows.Windows;

namespace MrmTool
{
    internal static unsafe partial class NativeUtils
    {
        private enum WINDOWCOMPOSITIONATTRIB
        {
            WCA_UNDEFINED = 0,
            WCA_NCRENDERING_ENABLED = 1,
            WCA_NCRENDERING_POLICY = 2,
            WCA_TRANSITIONS_FORCEDISABLED = 3,
            WCA_ALLOW_NCPAINT = 4,
            WCA_CAPTION_BUTTON_BOUNDS = 5,
            WCA_NONCLIENT_RTL_LAYOUT = 6,
            WCA_FORCE_ICONIC_REPRESENTATION = 7,
            WCA_EXTENDED_FRAME_BOUNDS = 8,
            WCA_HAS_ICONIC_BITMAP = 9,
            WCA_THEME_ATTRIBUTES = 10,
            WCA_NCRENDERING_EXILED = 11,
            WCA_NCADORNMENTINFO = 12,
            WCA_EXCLUDED_FROM_LIVEPREVIEW = 13,
            WCA_VIDEO_OVERLAY_ACTIVE = 14,
            WCA_FORCE_ACTIVEWINDOW_APPEARANCE = 15,
            WCA_DISALLOW_PEEK = 16,
            WCA_CLOAK = 17,
            WCA_CLOAKED = 18,
            WCA_ACCENT_POLICY = 19,
            WCA_FREEZE_REPRESENTATION = 20,
            WCA_EVER_UNCLOAKED = 21,
            WCA_VISUAL_OWNER = 22,
            WCA_HOLOGRAPHIC = 23,
            WCA_EXCLUDED_FROM_DDA = 24,
            WCA_PASSIVEUPDATEMODE = 25,
            WCA_USEDARKMODECOLORS = 26,
            WCA_CORNER_STYLE = 27,
            WCA_PART_COLOR = 28,
            WCA_DISABLE_MOVESIZE_FEEDBACK = 29,
            WCA_SYSTEMBACKDROP_TYPE = 30,
            WCA_SET_TAGGED_WINDOW_RECT = 31,
            WCA_CLEAR_TAGGED_WINDOW_RECT = 32,
            WCA_LAST = 33,
        }

        private enum PreferredAppMode
        {
            Default,
            AllowDark,
            ForceDark,
            ForceLight,
            Max
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct WINDOWCOMPOSITIONATTRIBDATA
        {
            public WINDOWCOMPOSITIONATTRIB Attrib;
            public void* pvData;
            public uint cbData;
        }

        [PreserveSig]
        [DllImport("uxtheme.dll", EntryPoint = "#137")]
        private static extern BOOL IsDarkModeAllowedForWindow(HWND hwnd);

        [PreserveSig]
        [DllImport("uxtheme.dll", EntryPoint = "#132")]
        private static extern BOOL ShouldAppsUseDarkMode();

        [PreserveSig]
        [DllImport("user32.dll")]
        private static extern BOOL SetWindowCompositionAttribute(HWND hwnd, WINDOWCOMPOSITIONATTRIBDATA* data);

        [PreserveSig]
        [DllImport("uxtheme.dll", EntryPoint = "#135")]
        private static extern void SetPreferredAppMode(PreferredAppMode appMode);

        [PreserveSig]
        [DllImport("uxtheme.dll", EntryPoint = "#104")]
        private static extern void RefreshImmersiveColorPolicyState();

        [PreserveSig]
        [DllImport("uxtheme.dll", EntryPoint = "#133")]
        private static extern void AllowDarkModeForWindow(HWND hwnd, bool allow);

        public enum CoreWindowType : int
        {
            IMMERSIVE_BODY = 0,
            IMMERSIVE_DOCK,
            IMMERSIVE_HOSTED,
            IMMERSIVE_TEST,
            IMMERSIVE_BODY_ACTIVE,
            IMMERSIVE_DOCK_ACTIVE,
            NOT_IMMERSIVE
        }

        [DllImport("Windows.UI.dll", EntryPoint = "#1500", ExactSpelling = true)]
        public static extern int PrivateCreateCoreWindow(
            CoreWindowType coreWindowType,
            char* windowTitle,
            int x,
            int y,
            int width,
            int height,
            uint dwAttributes,
            HWND hOwnerWindow,
            Guid* riid,
            nint* pCoreWindow);

        [GuidRVAGen.Guid("79b9d5f2-879e-4b89-b798-79e47598030c")]
        public static partial Guid* IID_ICoreWindow { get; }

        internal static void EnsureTitleBarTheme(HWND hwnd)
        {
            bool isDarkMode = IsDarkModeAllowedForWindow(hwnd) && ShouldAppsUseDarkMode();

            WINDOWCOMPOSITIONATTRIBDATA data = new()
            {
                Attrib = WINDOWCOMPOSITIONATTRIB.WCA_USEDARKMODECOLORS,
                pvData = &isDarkMode,
                cbData = (uint)sizeof(BOOL)
            };

            SetWindowCompositionAttribute(hwnd, &data);
        }

        internal static void EnableDarkModeSupport(HWND hwnd)
        {
            SetPreferredAppMode(PreferredAppMode.AllowDark);
            RefreshImmersiveColorPolicyState();

            AllowDarkModeForWindow(hwnd, true);
        }

        internal static void InitializeResourceManager(string priFileName = "resources.pri")
        {
            try
            {
                // This is in a Try-Catch because because Current throws in unpackaged apps
                ArgumentNullException.ThrowIfNull(ResourceManager.Current);
            }
            catch
            {
                void* pManager = default;
                var managerStatics = ResourceManager.As<IResourceManagerStaticInternal>();
                ThrowIfFailed((HRESULT)managerStatics.GetCurrentResourceManagerForSystemProfile(&pManager));
                var manager = ResourceManager.FromAbi((nint)pManager);
                Marshal.Release((nint)pManager);

                var systemEx = (ISystemResourceManagerExtensions2)(object)manager;
                systemEx.LoadPriFileForSystemUse(AppContext.BaseDirectory + $"\\{priFileName}");
            }
        }
    }

    [CustomMarshaller(typeof(string), MarshalMode.Default, typeof(HStringMarshaller))]
    internal static unsafe class HStringMarshaller
    {
        public static nint ConvertToUnmanaged(string? managed)
            => WinRT.MarshalString.FromManaged(managed);

        public static string? ConvertToManaged(nint unmanaged)
            => WinRT.MarshalString.FromAbi(unmanaged);

        public static void Free(nint unmanaged)
            => WinRT.MarshalString.DisposeAbi(unmanaged);
    }

    [GeneratedComInterface]
    [Guid("4a8eac58-b652-459d-8de1-239471e8b22b")]
    internal unsafe partial interface IResourceManagerStaticInternal
    {
        void _stub0();
        void _stub1();
        void _stub2();
        void _stub3();

        [PreserveSig]
        int GetCurrentResourceManagerForSystemProfile(void** ppResult);
    }

    [GeneratedComInterface(StringMarshalling = StringMarshalling.Utf16)]
    [Guid("8c25e859-1042-4da0-9232-bf2aa8ff3726")]
    internal unsafe partial interface ISystemResourceManagerExtensions2
    {
        void _stub0();
        void _stub1();
        void _stub2();

        void LoadPriFileForSystemUse(string path);
    }

    [GeneratedComInterface, Guid("4dc10e42-52e7-46da-8ae8-92a4e8afe20c")]
    internal unsafe partial interface IPickerPrivateInitialization
    {
        void _stub0();
        void _stub1();
        void _stub2();

        void SetInitialLocationWithShellItem(void* pShellItem);

        void SetNamespaceRoot(void* pShellItem);

        void SetInProcOverride(int bInProc);

        void SetTargetFolderLibrary(void* pShellItem);

        void PrepopulateCallingAppData([MarshalUsing(typeof(HStringMarshaller))] string appId, [MarshalUsing(typeof(HStringMarshaller))] string packageFullName);
    }

    [GeneratedComInterface, Guid("6090202d-2843-4ba5-9b0d-fc88eecd9ce5")]
    internal partial interface ICoreApplicationPrivate2
    {
        void _stub3();
        void _stub4();
        void _stub5();
        void _stub6();
        void _stub7();
        nint CreateNonImmersiveView();
    }
}
