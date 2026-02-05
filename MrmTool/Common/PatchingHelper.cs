using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TerraFX.Interop.Windows;

using static MrmTool.Common.ErrorHelpers;
using static TerraFX.Interop.Windows.Windows;

#if IS_64_BIT
using IMAGE_THUNK_DATA = TerraFX.Interop.Windows.IMAGE_THUNK_DATA64;
using IMAGE_NT_HEADERS = TerraFX.Interop.Windows.IMAGE_NT_HEADERS64;
#else
using IMAGE_THUNK_DATA = TerraFX.Interop.Windows.IMAGE_THUNK_DATA32;
using IMAGE_NT_HEADERS = TerraFX.Interop.Windows.IMAGE_NT_HEADERS32;
#endif

namespace MrmTool.Common
{
    internal static unsafe class PatchingHelper
    {
        internal static void* GetModuleEntryPoint(HMODULE Module)
        {
            if (Module.Value is null)
                return null;

            var dosHeader = (IMAGE_DOS_HEADER*)Module;
            var ntHeaders = (IMAGE_NT_HEADERS*)((byte*)Module + dosHeader->e_lfanew);

            return (byte*)Module + ntHeaders->OptionalHeader.AddressOfEntryPoint;
        }

#if true // Specialized version for MrmTool usecase

        internal static readonly (nuint Thunk, nuint OriginalFunction)[] PatchedFunctions = new (nuint, nuint)[3];

        // Originally written by @DaZombieKiller for the XWine1 project
        private static HRESULT XWineFindImport(HMODULE Module,
                                               ReadOnlySpan<byte> Import,
                                               IMAGE_THUNK_DATA* pImportAddressTable,
                                               IMAGE_THUNK_DATA* pImportNameTable,
                                               IMAGE_THUNK_DATA** pThunk)
        {
            for (nuint j = 0; pImportNameTable[j].u1.AddressOfData > 0; j++)
            {
                if ((pImportNameTable[j].u1.AddressOfData & IMAGE.IMAGE_ORDINAL_FLAG) != 0)
                    continue;

                var name = MemoryMarshal.CreateReadOnlySpanFromNullTerminated((byte*)Unsafe.AsPointer(in ((IMAGE_IMPORT_BY_NAME*)((byte*)Module + pImportNameTable[j].u1.AddressOfData))->Name.e0));

                if (!name.SequenceEqual(Import))
                    continue;

                *pThunk = &pImportAddressTable[j];
                return S.S_OK;
            }

            *pThunk = null;
            return E.E_FAIL;
        }

        // Originally written by @DaZombieKiller for the XWine1 project
        internal static HRESULT XWineGetImport(HMODULE Module,
                                               HMODULE ImportModule,
                                               ReadOnlySpan<byte> Import,
                                               IMAGE_THUNK_DATA** pThunk)
        {
            if (ImportModule.Value is null)
                return E.E_INVALIDARG;

            if (pThunk == null)
                return E.E_POINTER;

            if (Module.Value is null)
                Module = GetModuleHandleW(null);

            var dosHeader = (IMAGE_DOS_HEADER*)Module;
            var ntHeaders = (IMAGE_NT_HEADERS*)((byte*)Module + dosHeader->e_lfanew);
            var directory = &ntHeaders->OptionalHeader.DataDirectory[IMAGE.IMAGE_DIRECTORY_ENTRY_IMPORT];

            if (directory->VirtualAddress <= 0 || directory->Size <= 0)
                return E.E_FAIL;

            var peImports = (IMAGE_IMPORT_DESCRIPTOR*)((byte*)Module + directory->VirtualAddress);

            for (nuint i = 0; peImports[i].Name > 0; i++)
            {
                if (GetModuleHandleA((sbyte*)((byte*)Module + peImports[i].Name)) != ImportModule)
                    continue;

                var iatThunks = (IMAGE_THUNK_DATA*)((byte*)Module + peImports[i].FirstThunk);
                var intThunks = (IMAGE_THUNK_DATA*)((byte*)Module + peImports[i].OriginalFirstThunk);

                if (SUCCEEDED(XWineFindImport(Module, Import, iatThunks, intThunks, pThunk)))
                    return S.S_OK;
            }

            var delayDir = &ntHeaders->OptionalHeader.DataDirectory[IMAGE.IMAGE_DIRECTORY_ENTRY_DELAY_IMPORT];
            if (delayDir->VirtualAddress > 0 && directory->Size > 0)
            {
                var delayImports = (IMAGE_DELAYLOAD_DESCRIPTOR*)((byte*)Module + delayDir->VirtualAddress);

                for (nuint i = 0; delayImports[i].DllNameRVA > 0; i++)
                {
                    if (GetModuleHandleA((sbyte*)((byte*)Module + delayImports[i].DllNameRVA)) != ImportModule)
                        continue;

                    var iatThunks = (IMAGE_THUNK_DATA*)((byte*)Module + delayImports[i].ImportAddressTableRVA);
                    var intThunks = (IMAGE_THUNK_DATA*)((byte*)Module + delayImports[i].ImportNameTableRVA);

                    if (SUCCEEDED(XWineFindImport(Module, Import, iatThunks, intThunks, pThunk)))
                        return S.S_OK;
                }
            }

            *pThunk = null;
            return E.E_FAIL;
        }

        // Originally written by @DaZombieKiller for the XWine1 project
        internal static HRESULT XWinePatchImport(HMODULE Module,
                                                 HMODULE ImportModule,
                                                 ReadOnlySpan<byte> Import,
                                                 void* Function,
                                                 int Index)
        {
            HRESULT hr;

            uint protect;
            IMAGE_THUNK_DATA* pThunk;
            if (!SUCCEEDED_LOG(hr = XWineGetImport(Module, ImportModule, Import, &pThunk)))
            {
                return hr;
            }

            if (!VirtualProtect(&pThunk->u1.Function, (nuint)sizeof(nuint), PAGE.PAGE_READWRITE, &protect))
            {
                return HRESULT_FROM_WIN32(GetLastError());
            }

            nuint originalFunction = (nuint)pThunk->u1.Function;
            pThunk->u1.Function = (nuint)Function;

            PatchedFunctions[Index] = ((nuint)(void*)&pThunk->u1.Function, originalFunction);

            if (!VirtualProtect(&pThunk->u1.Function, (nuint)sizeof(nuint), protect, &protect))
            {
                return HRESULT_FROM_WIN32(GetLastError());
            }

            return S.S_OK;
        }

#else // Generic version

        internal static readonly Dictionary<nuint, nuint> PatchedFunctions = [];

        // Originally written by @DaZombieKiller for the XWine1 project
        private static HRESULT XWineFindImport(HMODULE Module,
                                               byte* Import,
                                               IMAGE_THUNK_DATA* pImportAddressTable,
                                               IMAGE_THUNK_DATA* pImportNameTable,
                                               IMAGE_THUNK_DATA** pThunk)
        {
            for (nuint j = 0; pImportNameTable[j].u1.AddressOfData > 0; j++)
            {
                if ((pImportNameTable[j].u1.AddressOfData & IMAGE.IMAGE_ORDINAL_FLAG) != 0)
                {
                    if (!IS_INTRESOURCE((nuint)Import))
                        continue;

                    if (((pImportNameTable[j].u1.Ordinal & ~IMAGE.IMAGE_ORDINAL_FLAG) == (nuint)Import))
                    {
                        *pThunk = &pImportAddressTable[j];
                        return S.S_OK;
                    }

                    continue;
                }

                var name = MemoryMarshal.CreateReadOnlySpanFromNullTerminated((byte*)Unsafe.AsPointer(in ((IMAGE_IMPORT_BY_NAME*)((byte*)Module + pImportNameTable[j].u1.AddressOfData))->Name.e0));
                var importName = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(Import);

                if (!name.SequenceEqual(importName))
                    continue;

                *pThunk = &pImportAddressTable[j];
                return S.S_OK;
            }

            *pThunk = null;
            return E.E_FAIL;
        }

        // Originally written by @DaZombieKiller for the XWine1 project
        internal static HRESULT XWineGetImport(HMODULE Module,
                                               HMODULE ImportModule,
                                               byte* Import,
                                               IMAGE_THUNK_DATA** pThunk)
        {
            if (ImportModule.Value is null)
                return E.E_INVALIDARG;

            if (pThunk == null)
                return E.E_POINTER;

            if (Module.Value is null)
                Module = GetModuleHandleW(null);

            var dosHeader = (IMAGE_DOS_HEADER*)Module;
            var ntHeaders = (IMAGE_NT_HEADERS*)((byte*)Module + dosHeader->e_lfanew);
            var directory = &ntHeaders->OptionalHeader.DataDirectory[IMAGE.IMAGE_DIRECTORY_ENTRY_IMPORT];

            if (directory->VirtualAddress <= 0 || directory->Size <= 0)
                return E.E_FAIL;

            var peImports = (IMAGE_IMPORT_DESCRIPTOR*)((byte*)Module + directory->VirtualAddress);

            for (nuint i = 0; peImports[i].Name > 0; i++)
            {
                if (GetModuleHandleA((sbyte*)((byte*)Module + peImports[i].Name)) != ImportModule)
                    continue;

                var iatThunks = (IMAGE_THUNK_DATA*)((byte*)Module + peImports[i].FirstThunk);
                var intThunks = (IMAGE_THUNK_DATA*)((byte*)Module + peImports[i].OriginalFirstThunk);

                if (SUCCEEDED(XWineFindImport(Module, Import, iatThunks, intThunks, pThunk)))
                    return S.S_OK;
            }

            var delayDir = &ntHeaders->OptionalHeader.DataDirectory[IMAGE.IMAGE_DIRECTORY_ENTRY_DELAY_IMPORT];
            if (delayDir->VirtualAddress > 0 && directory->Size > 0)
            {
                var delayImports = (IMAGE_DELAYLOAD_DESCRIPTOR*)((byte*)Module + delayDir->VirtualAddress);

                for (nuint i = 0; delayImports[i].DllNameRVA > 0; i++)
                {
                    if (GetModuleHandleA((sbyte*)((byte*)Module + delayImports[i].DllNameRVA)) != ImportModule)
                        continue;

                    var iatThunks = (IMAGE_THUNK_DATA*)((byte*)Module + delayImports[i].ImportAddressTableRVA);
                    var intThunks = (IMAGE_THUNK_DATA*)((byte*)Module + delayImports[i].ImportNameTableRVA);

                    if (SUCCEEDED(XWineFindImport(Module, Import, iatThunks, intThunks, pThunk)))
                        return S.S_OK;
                }
            }

            *pThunk = null;
            return E.E_FAIL;
        }

        // Originally written by @DaZombieKiller for the XWine1 project
        internal static HRESULT XWinePatchImport(HMODULE Module,
                                                 HMODULE ImportModule,
                                                 byte* Import,
                                                 void* Function)
        {
            HRESULT hr;

            uint protect;
            IMAGE_THUNK_DATA* pThunk;
            if (!SUCCEEDED_LOG(hr = XWineGetImport(Module, ImportModule, Import, &pThunk)))
            {
                return hr;
            }

            if (!VirtualProtect(&pThunk->u1.Function, (nuint)sizeof(nuint), PAGE.PAGE_READWRITE, &protect))
            {
                return HRESULT_FROM_WIN32(GetLastError());
            }

            nuint originalFunction = (nuint)pThunk->u1.Function;
            pThunk->u1.Function = (nuint)Function;

            PatchedFunctions.TryAdd((nuint)(void*)&pThunk->u1.Function, originalFunction);


            if (!VirtualProtect(&pThunk->u1.Function, (nuint)sizeof(nuint), protect, &protect))
            {
                return HRESULT_FROM_WIN32(GetLastError());
            }

            return S.S_OK;
        }

#endif
    }
}
