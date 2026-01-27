using MrmTool.Common;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Storage.Streams;
using WinRT;
using WinRT.Interop;

namespace MrmTool.Common
{
    internal unsafe partial class NativeBuffer : IBuffer, IDisposable, IBufferByteAccess_NativeBuffer
    {
        readonly uint _size;
        byte* _buffer;

        internal byte* Buffer => _buffer;

        internal NativeBuffer(uint size)
        {
            _size = size;
            _buffer = (byte*)NativeMemory.Alloc(size);
        }

        public uint Capacity => _size;

        public uint Length { get => _size; set { } }

        unsafe int IBufferByteAccess_NativeBuffer.GetBuffer(byte** ppBuffer)
        {
            *ppBuffer = _buffer;
            return 0;
        }

        public void Dispose()
        {
            if (_buffer is not null)
            {
                NativeMemory.Free(_buffer);
                _buffer = null;
            }

            GC.SuppressFinalize(this);
        }

        ~NativeBuffer()
        {
            if (_buffer is not null)
            {
                NativeMemory.Free(_buffer);
                _buffer = null;
            }
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Guid("905a0fef-bc53-11df-8c49-001e4fc686da")]
    [WindowsRuntimeType("Windows.Foundation.UniversalApiContract")]
    [WindowsRuntimeHelperType(typeof(ABI.MrmTool.Common.IBufferByteAccess_NativeBuffer_ABI))]
    internal unsafe interface IBufferByteAccess_NativeBuffer
    {
        int GetBuffer(byte** ppBuffer);
    }
}

namespace ABI.MrmTool.Common
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Guid("905a0fef-bc53-11df-8c49-001e4fc686da")]
    internal unsafe struct IBufferByteAccess_NativeBuffer_ABI
    {
        public void** lpVtbl;

        [Guid("905a0fef-bc53-11df-8c49-001e4fc686da")]
        public struct Vftbl
        {
            internal IUnknownVftbl IUnknownVftbl;

            public unsafe delegate* unmanaged[Stdcall]<IntPtr, byte**, int> Get_Buffer_0;

            public static readonly IntPtr AbiToProjectionVftablePtr;

            unsafe static Vftbl()
            {
                AbiToProjectionVftablePtr = ComWrappersSupport.AllocateVtableMemory(typeof(Vftbl), sizeof(IUnknownVftbl) + sizeof(IntPtr));
                *(Vftbl*)AbiToProjectionVftablePtr = new Vftbl
                {
                    IUnknownVftbl = IUnknownVftbl.AbiToProjectionVftbl,
                    Get_Buffer_0 = (delegate* unmanaged[Stdcall]<IntPtr, byte**, int>)&Do_Abi_get_Buffer_0
                };
            }

            [UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
            private unsafe static int Do_Abi_get_Buffer_0(IntPtr thisPtr, byte** buffer)
            {
                return ComWrappersSupport.FindObject<IBufferByteAccess_NativeBuffer>(thisPtr).GetBuffer(buffer);
            }
        }

        internal unsafe int GetBuffer(byte** ppBuffer)
        {
            return ((delegate* unmanaged[Stdcall]<void*, byte**, int>)lpVtbl[3])(Unsafe.AsPointer(ref this), ppBuffer);
        }

        internal static ObjectReference<IUnknownVftbl> FromAbi(IntPtr thisPtr)
        {
            return ObjectReference<IUnknownVftbl>.FromAbi(thisPtr, IID.IID_IBufferByteAccess);
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static unsafe class IBufferByteAccess_NativeBufferMethods
    {
        public static Guid IID => TerraFX.Interop.Windows.IID.IID_IBufferByteAccess;

        public static IntPtr AbiToProjectionVftablePtr => IBufferByteAccess_NativeBuffer_ABI.Vftbl.AbiToProjectionVftablePtr;
    }
}
