using Microsoft.Win32;
using MrmTool.Common;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using TerraFX.Interop.Windows;
using Windows.ApplicationModel.Resources.Core;
using Windows.Management.Deployment;

using static MrmTool.Common.ErrorHelpers;
using static TerraFX.Interop.Windows.Windows;

namespace MrmTool
{
    internal static unsafe partial class NativeUtils
    {
        private readonly static RegistryKey personalizeKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize")!;

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

        //[PreserveSig]
        //[DllImport("uxtheme.dll", EntryPoint = "#132")]
        //private static extern BOOL ShouldAppsUseDarkMode();

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

        [PreserveSig]
        [DllImport("iertutil.dll", EntryPoint = "#797")]
        private static extern HRESULT IEConfiguration_SetBrowserAppProfile(char* profile, uint type, uint unk);

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

        [Flags]
        internal enum ASTA_TEST_MODE_FLAGS
        {
            NONE = 0x0,
            RO_INIT_SINGLETHREADED_CREATES_ASTAS = 0x1,
            GIT_LIFETIME_EXTENSION_ENABLED = 0x2,
            ROINITIALIZEASTA_ALLOWED = 0x4,
        }

        [DllImport("combase.dll", EntryPoint = "#100")]
        internal static extern void CoSetASTATestMode(ASTA_TEST_MODE_FLAGS flags);

        [GuidRVAGen.Guid("79b9d5f2-879e-4b89-b798-79e47598030c")]
        public static partial Guid* IID_ICoreWindow { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ShouldAppsUseDarkMode()
        {
            return personalizeKey.GetValue("AppsUseLightTheme") is 0;
        }

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

        internal static ResourceManager InitializeResourceManager(string priFileName = "resources.pri")
        {
            ResourceManager manager;

            try
            {
                // This is in a try-catch because Current throws in unpackaged apps on old Windows builds
                ArgumentNullException.ThrowIfNull(manager = ResourceManager.Current);
            }
            catch
            {
                void* pManager = default;

                try
                {
                    var managerStatics = ResourceManager.As<IResourceManagerStaticInternal>();
                    ThrowIfFailed((HRESULT)managerStatics.GetCurrentResourceManagerForSystemProfile(&pManager));
                }
                catch
                {
                    var managerStatics = ResourceManager.As<IResourceManagerStaticInternalOld>();
                    ThrowIfFailed((HRESULT)managerStatics.GetCurrentResourceManagerForSystemProfile(&pManager));
                }

                manager = ResourceManager.FromAbi((nint)pManager);
                Marshal.Release((nint)pManager);

                var systemEx = (ISystemResourceManagerExtensions2)(object)manager;
                systemEx.LoadPriFileForSystemUse(AppContext.BaseDirectory + $"\\{priFileName}");
            }

            return manager;
        }

        // References: https://gist.github.com/diversenok/930600b5aec5e8d15664662b9176a691, https://ntdoc.m417z.com/peb

        [StructLayout(LayoutKind.Sequential)]
        struct SWITCH_CONTEXT_ATTRIBUTE
        {
            public ulong ContextUpdateCounter;
            public BOOL AllowContextUpdate;
            public BOOL EnableTrace;
            public ulong EtwHandle;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SWITCH_CONTEXT_DATA
        {
            public ulong OsMaxVersionTested;
            public uint TargetPlatform;
            public ulong ContextMinimum;
            public Guid Platform;
            public Guid MinPlatform;
            public uint ContextSource;
            public uint ElementCount;
            public _Elements Elements;

            [InlineArray(48)]
            public struct _Elements
            {
                public Guid e0;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        struct SWITCH_CONTEXT
        {
            public SWITCH_CONTEXT_ATTRIBUTE Attribute;
            public SWITCH_CONTEXT_DATA Data;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct SDBQUERYRESULT
        {
            public fixed uint Exes[16];
            public fixed uint ExeFlags[16];
            public fixed uint Layers[8];
            public uint LayerFlags;
            public uint AppHelp;
            public uint ExeCount;
            public uint LayerCount;
            public Guid ID;
            public uint ExtraFlags;
            public uint CustomSDBMap;
            public _DB DB;

            [InlineArray(16)]
            public struct _DB
            {
                public Guid e0;
            }
        }

        /*
        [StructLayout(LayoutKind.Sequential)]
        struct APPCOMPAT_EXE_DATA_EIGHT
        {
            public fixed ushort ShimEngine[MAX.MAX_PATH];
            public uint Size;
            public uint Magic;
            public ushort ExeType;
            public SDBQUERYRESULT SdbQueryResult;
            public fixed byte DbgLogChannels[1024];
            public SWITCH_CONTEXT SwitchContext; // ulong[128]
        }
        */

        [StructLayout(LayoutKind.Sequential)]
        struct APPCOMPAT_EXE_DATA_TH1
        {
            public fixed ushort ShimEngine[MAX.MAX_PATH];
            public uint Size;
            public uint Magic;
            public ushort ExeType;
            public SDBQUERYRESULT SdbQueryResult;
            public fixed byte DbgLogChannels[1024];
            public SWITCH_CONTEXT SwitchContext; // ulong[128]
        }

        [StructLayout(LayoutKind.Sequential)]
        struct APPCOMPAT_EXE_DATA_RS2
        {
            public uint Size;
            public uint Magic;
            public BOOL LoadShimEngine;
            public ushort ExeType;
            public SDBQUERYRESULT SdbQueryResult;
            public fixed byte DbgLogChannels[1024];
            public SWITCH_CONTEXT SwitchContext;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct APPCOMPAT_EXE_DATA
        {
            public fixed ulong Reserved[65];
            public uint Size;
            public uint Magic;
            public BOOL LoadShimEngine;
            public ushort ExeType;
            public SDBQUERYRESULT SdbQueryResult;
            public fixed byte DbgLogChannels[1024];
            public SWITCH_CONTEXT SwitchContext;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static nint OffsetOfAppCompatShimData()
        {
            var padding = sizeof(void*) - sizeof(uint);

            PEB peb;
            return (nint)((byte*)&peb.SessionId - (byte*)&peb) + sizeof(uint) + padding + (2 * sizeof(ulong));
            //                                                 + SessionId    + Padding + (AppCompatFlags + AppCompatFlagsUser)
        }

        private static PEB* _peb = null;

        internal static PEB* NtCurrentPeb()
        {
            if (_peb is not null)
                return _peb;

            PROCESS_BASIC_INFORMATION info = new();

            uint len = 0;
            ThrowIfFailed(HRESULT_FROM_NT(NtQueryInformationProcess(
                GetCurrentProcess(),
                PROCESSINFOCLASS.ProcessBasicInformation,
                &info,
                (uint)sizeof(PROCESS_BASIC_INFORMATION),
                &len
            )));

            _peb = info.PebBaseAddress;
            return _peb;
        }

        private static ReadOnlySpan<byte> Windows10_PlatformID => [0x12, 0x7a, 0x0f, 0x8e, 0xb3, 0xbf, 0xe8, 0x4f, 0xb9, 0xa5, 0x48, 0xfd, 0x50, 0xa1, 0x5a, 0x9a];

        private static nint _switchContextOffset = 0;

        private static SWITCH_CONTEXT* GetSwitchContext(APPCOMPAT_EXE_DATA* pShim)
        {
            if (_switchContextOffset is 0)
            {
                if (Windows10_PlatformID.SequenceEqual(new((byte*)&pShim->SwitchContext.Data.Platform, sizeof(Guid))))
                {
                    _switchContextOffset = (nint)((byte*)&pShim->SwitchContext - (byte*)pShim);
                }
                else if (Windows10_PlatformID.SequenceEqual(new((byte*)&((APPCOMPAT_EXE_DATA_RS2*)pShim)->SwitchContext.Data.Platform, sizeof(Guid))))
                {
                    _switchContextOffset = (nint)((byte*)&((APPCOMPAT_EXE_DATA_RS2*)pShim)->SwitchContext - (byte*)pShim);
                }
                else if (Windows10_PlatformID.SequenceEqual(new((byte*)&((APPCOMPAT_EXE_DATA_TH1*)pShim)->SwitchContext.Data.Platform, sizeof(Guid))))
                {
                    _switchContextOffset = (nint)((byte*)&((APPCOMPAT_EXE_DATA_TH1*)pShim)->SwitchContext - (byte*)pShim);
                }
                /*else if (Windows10_PlatformID.SequenceEqual(new((byte*)&((APPCOMPAT_EXE_DATA_EIGHT*)pShim)->SwitchContext.Data.Platform, sizeof(Guid))))
                {
                    _switchContextOffset = (nint)((byte*)&((APPCOMPAT_EXE_DATA_EIGHT*)pShim)->SwitchContext - (byte*)pShim);
                }*/
                else
                {
                    var current = (byte*)pShim;
                    var end = (byte*)&pShim[1] - sizeof(Guid);
                    var offset = (nint)((byte*)&pShim->SwitchContext.Data.Platform - (byte*)&pShim->SwitchContext);

                    while (current <= end)
                    {
                        if (Windows10_PlatformID.SequenceEqual(new(current, sizeof(Guid))))
                        {
                            _switchContextOffset = (nint)(current - (byte*)pShim) - offset;
                            break;
                        }

                        current++;
                    }
                }
            }

            if (_switchContextOffset is not 0)
                return (SWITCH_CONTEXT*)((byte*)pShim + _switchContextOffset);

            return null;
        }

        internal static SWITCH_CONTEXT_DATA* GetSwitchContextData()
        {
            var appCompat = *(APPCOMPAT_EXE_DATA**)((nint)NtCurrentPeb() + OffsetOfAppCompatShimData());
            if (appCompat is null)
                return null;

            var switchContext = GetSwitchContext(appCompat);
            return switchContext is not null ? &switchContext->Data : null;
        }

        internal static void ShowFileInExplorer(string path)
        {
            fixed (char* pPath = path)
            {
                ITEMIDLIST* pList = default;
                if (SUCCEEDED_LOG(SHParseDisplayName(pPath, null, &pList, 0, null)))
                {
                    LOG_IF_FAILED(SHOpenFolderAndSelectItems(pList, 0, null, 0));
                    ILFree(pList);
                }
            }
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
        private static int AppPolicyGetWindowingModelHook(HANDLE processToken, AppPolicyWindowingModel* policy)
        {
            *policy = AppPolicyWindowingModel.AppPolicyWindowingModel_None;
            return TerraFX.Interop.Windows.ERROR.ERROR_SUCCESS;
        }

        private static readonly string? settingsPFN =
            new PackageManager().FindPackagesForUser(null, "windows.immersivecontrolpanel_cw5n1h2txyewy").FirstOrDefault()?.Id.FullName;

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
        private static int GetCurrentPackageInfoHook(uint flags, uint* bufferLength, byte* buffer, uint* count)
        {
            if (settingsPFN is not null)
            {
                fixed (char* pSettingPFN = settingsPFN)
                {
                    PACKAGE_INFO_REFERENCE pir;
                    if (OpenPackageInfoByFullName(pSettingPFN, 0, &pir) is TerraFX.Interop.Windows.ERROR.ERROR_SUCCESS)
                    {
                        var result = GetPackageInfo(pir, flags, bufferLength, buffer, count);
                        _ = ClosePackageInfo(pir);
                        return result;
                    }
                }
            }

            return GetCurrentPackageInfo(flags, bufferLength, buffer, count);
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
        private static int IsImmersiveProcessHook(void* unk)
        {
            return 1;
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
        private static void* GetProcAddressHook(HMODULE module, sbyte* procName)
        {
            if (procName is not null && !IS_INTRESOURCE((nuint)procName))
            {
                var name = MemoryMarshal.CreateReadOnlySpanFromNullTerminated((byte*)procName);
                if (name.SequenceEqual("IsImmersiveProcess"u8))
                {
                    return (delegate* unmanaged[Stdcall]<void*, int>)&IsImmersiveProcessHook;
                }
            }

            return GetProcAddress(module, procName);
        }

        internal static bool PatchWebViewAppModelChecks()
        {
            var appmodel = LoadLibraryA((sbyte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("kernel.appcore.dll"u8)));
            var xaml = LoadLibraryA((sbyte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("Windows.UI.Xaml.dll"u8)));
            var iertutil = LoadLibraryA((sbyte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("iertutil.dll"u8)));
            var kb = GetModuleHandleA((sbyte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("kernelbase.dll"u8)));

            if (appmodel.Value is not null &&
                xaml.Value is not null &&
                iertutil.Value is not null)
            {
                var fptr = (delegate* unmanaged[Stdcall]<HANDLE, AppPolicyWindowingModel*, int>)&AppPolicyGetWindowingModelHook;
                if (SUCCEEDED_LOG(PatchingHelper.XWinePatchImport(xaml, appmodel, "AppPolicyGetWindowingModel"u8, fptr, 0)))
                {
                    var fptr2 = (delegate* unmanaged[Stdcall]<uint, uint*, byte*, uint*, int>)&GetCurrentPackageInfoHook;
                    if (SUCCEEDED_LOG(PatchingHelper.XWinePatchImport(xaml, appmodel, "GetCurrentPackageInfo"u8, fptr2, 1)))
                    {
                        var fptr3 = (delegate* unmanaged[Stdcall]<HMODULE, sbyte*, void*>)&GetProcAddressHook;
                        if (SUCCEEDED_LOG(PatchingHelper.XWinePatchImport(iertutil, kb, "GetProcAddress"u8, fptr3, 2)) &&
                            SUCCEEDED_LOG(IEConfiguration_SetBrowserAppProfile((char*)Unsafe.AsPointer(in MemoryMarshal.GetReference("MicrosoftEdge".AsSpan())), 2, 0)))
                        {
                            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static void OnProcessExit(object? sender, EventArgs e)
        {
            foreach (var (Thunk, OriginalFunction) in PatchingHelper.PatchedFunctions)
            {
                var thunk = (nuint*)Thunk;

                uint protect;
                if (VirtualProtect(thunk, (nuint)sizeof(nuint), PAGE.PAGE_READWRITE, &protect))
                {
                    *thunk = OriginalFunction;
                    VirtualProtect(thunk, (nuint)sizeof(nuint), protect, &protect);
                }
            }

            var edge = GetModuleHandleA((sbyte*)Unsafe.AsPointer(in MemoryMarshal.GetReference("edgehtml.dll"u8)));
            if (edge.Value is not null)
            {
                var dllmain = (delegate* unmanaged[Stdcall]<HINSTANCE, uint, void*, BOOL>)PatchingHelper.GetModuleEntryPoint(edge);
                if (dllmain is not null)
                {
                    LOG_LAST_ERROR_IF(!dllmain(edge, DLL_PROCESS_DETACH, null));
                }
            }
        }
    }

#if ENABLE_HSTRING_MARSHALLER
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
#endif

    [Guid("4a8eac58-b652-459d-8de1-239471e8b22b")]
    [GeneratedComInterface(Options = ComInterfaceOptions.ComObjectWrapper)]
    internal unsafe partial interface IResourceManagerStaticInternal
    {
        void _stub0();
        void _stub1();
        void _stub2();
        void _stub3();

        [PreserveSig]
        int GetCurrentResourceManagerForSystemProfile(void** ppResult);
    }

    [Guid("7d9da47a-8bc7-49d3-97aa-f7db06049172")]
    [GeneratedComInterface(Options = ComInterfaceOptions.ComObjectWrapper)]
    internal unsafe partial interface IResourceManagerStaticInternalOld
    {
        void _stub0();
        void _stub1();
        void _stub2();
        void _stub3();

        [PreserveSig]
        int GetCurrentResourceManagerForSystemProfile(void** ppResult);
    }

    [Guid("8c25e859-1042-4da0-9232-bf2aa8ff3726")]
    [GeneratedComInterface(StringMarshalling = StringMarshalling.Utf16, Options = ComInterfaceOptions.ComObjectWrapper)]
    internal unsafe partial interface ISystemResourceManagerExtensions2
    {
        void _stub0();
        void _stub1();
        void _stub2();

        void LoadPriFileForSystemUse(string path);
    }

    [Guid("4dc10e42-52e7-46da-8ae8-92a4e8afe20c")]
    [GeneratedComInterface(Options = ComInterfaceOptions.ComObjectWrapper)]
    internal unsafe partial interface IPickerPrivateInitialization
    {
        void _stub0();
        void _stub1();
        void _stub2();

        void SetInitialLocationWithShellItem(void* pShellItem);

        void SetNamespaceRoot(void* pShellItem);

        void SetInProcOverride(int bInProc);

        void SetTargetFolderLibrary(void* pShellItem);

#if ENABLE_HSTRING_MARSHALLER
        void PrepopulateCallingAppData([MarshalUsing(typeof(HStringMarshaller))] string appId, [MarshalUsing(typeof(HStringMarshaller))] string packageFullName);
#endif
    }

    [Guid("6090202d-2843-4ba5-9b0d-fc88eecd9ce5")]
    [GeneratedComInterface(Options = ComInterfaceOptions.ComObjectWrapper)]
    internal partial interface ICoreApplicationPrivate2
    {
        void _stub3();
        void _stub4();
        void _stub5();
        void _stub6();
        void _stub7();
        nint CreateNonImmersiveView();
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WindowCreationParameters
    {
        public int Left, Top, Width, Height;
        public byte TransparentBackground, IsCoreNavigationClient;
    }

    [Guid("c45f3f8c-61e6-4f9a-be88-fe4fe6e64f5f")]
    [GeneratedComInterface(Options = ComInterfaceOptions.ComObjectWrapper)]
    internal unsafe partial interface IFrameworkApplicationStaticsPrivate
    {
        void _stub0();
        void _stub1();
        void _stub2();

        void StartInCoreWindowHostingMode(WindowCreationParameters windowParams, void* callback);
        void EnableFailFastOnStowedException();
    }
}