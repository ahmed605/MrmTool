using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using TerraFX.Interop.Windows;
using Windows.Foundation;
using Windows.Storage.Streams;
using WinRT;

using static TerraFX.Interop.Windows.Windows;
using IBufferByteAccess = TerraFX.Interop.WinRT.IBufferByteAccess;

namespace MrmTool.Common
{
    internal unsafe partial class RandomAccessStreamOverBuffer : IRandomAccessStream
    {
        private readonly IBuffer _buffer;
        private ComPtr<IBufferByteAccess> _bufferByteAccess;
        private readonly bool _readOnly;
        private ulong _position;

        internal IBuffer Buffer => _buffer;

        internal IBufferByteAccess* BufferByteAccess => _bufferByteAccess.Get();

        internal RandomAccessStreamOverBuffer(IBuffer buffer, bool readOnly = true)
        {
            _buffer = buffer;
            _readOnly = readOnly;

            ThrowIfFailed(((IUnknown*)((IWinRTObject)buffer).NativeObject.ThisPtr)->QueryInterface(
                (Guid*)Unsafe.AsPointer(in IID.IID_IBufferByteAccess),
                (void**)_bufferByteAccess.GetAddressOf()));
        }

        public bool CanRead => true;

        public bool CanWrite => !_readOnly;

        public ulong Position => _position;

        public ulong Size 
        { 
            get => _buffer.Length;
            set
            {
                if (_readOnly)
                {
                    ThrowUnauthorizedAccessException("Stream is read-only.");
                }

                ArgumentOutOfRangeException.ThrowIfGreaterThan(value, _buffer.Capacity, nameof(Size));
                _buffer.Length = (uint)value;
            }
        }

        public IRandomAccessStream CloneStream()
        {
            return new RandomAccessStreamOverBuffer(_buffer, _readOnly);
        }

        public void Dispose()
        {
            _bufferByteAccess.Dispose();
        }

        public IAsyncOperation<bool> FlushAsync()
        {
            // Nothing to do here
            return AsyncInfo.FromResult(true);
        }

        public IInputStream GetInputStreamAt(ulong position)
        {
            throw new NotImplementedException();
        }

        public IOutputStream GetOutputStreamAt(ulong position)
        {
            throw new NotImplementedException();
        }

        public IAsyncOperationWithProgress<IBuffer, uint> ReadAsync(IBuffer buffer, uint count, InputStreamOptions options)
        {
            return AsyncInfo.Run((CancellationToken token, IProgress<uint> progress) =>
            {
                return Task.Run(() =>
                {
                    ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(_position, _buffer.Length, nameof(Position));

                    byte* data = buffer.GetData();

                    byte* srcData = default;
                    _bufferByteAccess.Get()->Buffer(&srcData);

                    uint dataToCopy = (uint)Math.Min(_buffer.Length - _position, count);
                    NativeMemory.Copy(srcData + _position, data, dataToCopy);
                    buffer.Length = dataToCopy;

                    _position += dataToCopy;
                    return buffer;
                });
            });
        }

        public void Seek(ulong position)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(position, _buffer.Length, nameof(position));
            _position = position;
        }

        public IAsyncOperationWithProgress<uint, uint> WriteAsync(IBuffer buffer)
        {
            return AsyncInfo.Run((CancellationToken token, IProgress<uint> progress) =>
            {
                return Task.Run(() =>
                {
                    if (_readOnly)
                    {
                        ThrowUnauthorizedAccessException("Stream is read-only.");
                    }

                    ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(_position, _buffer.Capacity, nameof(Position));

                    byte* data = buffer.GetData();

                    byte* dstData = default;
                    _bufferByteAccess.Get()->Buffer(&dstData);

                    uint dataToCopy = (uint)Math.Min(_buffer.Capacity - _position, buffer.Length);
                    NativeMemory.Copy(data, dstData + _position, dataToCopy);
                    _position += dataToCopy;
                    _buffer.Length = Math.Max((uint)_position, _buffer.Length);

                    return dataToCopy;
                });
            });
        }

        [DoesNotReturn]
        private static void ThrowUnauthorizedAccessException(string message)
        {
            throw new UnauthorizedAccessException(message);
        }
    }
}
