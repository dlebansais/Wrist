using System;
using System.Collections.Generic;
using System.IO;

namespace SmallArgon2d
{
    // This could be refactored to support either endianness but I only need little endian for this
    internal class LittleEndianActiveStream : Stream
    {
        public LittleEndianActiveStream(byte[] buffer = null)
        {
            _bufferSetupActions = new List<Action>();
            _buffer = buffer;
            _bufferAvailable = _buffer?.Length ?? 0;
        }

        public void Expose(short data)
        {
            _bufferSetupActions.Add(() => BufferShort((ushort)data));
        }

        public void Expose(ushort data)
        {
            _bufferSetupActions.Add(() => BufferShort(data));
        }

        public void Expose(int data)
        {
            _bufferSetupActions.Add(() => BufferInt((uint)data));
        }

        public void Expose(uint data)
        {
            _bufferSetupActions.Add(() => BufferInt(data));
        }

        public void Expose(byte data)
        {
            _bufferSetupActions.Add(() => BufferByte(data));
        }

        public void Expose(byte[] data)
        {
            if (data != null)
            {
                _bufferSetupActions.Add(() => BufferArray(data, 0, data.Length));
            }
        }

        public void Expose(Stream subStream)
        {
            if (subStream != null)
            {
                _bufferSetupActions.Add(() => BufferSubStream(subStream));
            }
        }

        public void Expose(Argon2Memory memory)
        {
            if (memory != null)
            {
                byte[] buff = new byte[0x80 * 8];
                memory.GetBuffer(buff);
                Expose(buff);
            }
        }

        public void ClearBuffer()
        {
            for (int i = 0; i < _buffer.Length; ++i)
            {
                _buffer[i] = 0;
            }

            _bufferAvailable = 0;
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int totalRead = 0;
            while (totalRead < count)
            {
                int available = _bufferAvailable - _bufferOffset;
                if (available == 0)
                {
                    if (_bufferSetupActions.Count == 0)
                    {
                        // There's nothing left to queue up - we read what we could
                        return totalRead;
                    }

                    _bufferSetupActions[0]();
                    _bufferSetupActions.RemoveAt(0);

                    // We are safe to assume that offset becomes 0 after that call
                    available = _bufferAvailable;
                }

                // If we only need to read part of available - reduce that
                available = Math.Min(available, count - totalRead);
                Array.Copy(_buffer, _bufferOffset, buffer, offset, available);

                _bufferOffset += available;
                offset += available;
                totalRead += available;
            }

            return totalRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException("LittleEndianActiveStream is non-seekable");
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException("LittleEndianActiveStream is an actual Stream that doesn't support length");
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _bufferSetupActions.Add(() => BufferArray(buffer, offset, count));
        }

        private void BufferSubStream(Stream stream)
        {
            ReserveBuffer(1024);
            int result = stream.Read(_buffer, 0, 1024);
            if (result == 1024)
            {
                _bufferSetupActions.Insert(0, () => BufferSubStream(stream));
            }
            else
            {
                stream.Dispose();
            }

            _bufferAvailable = result;
        }

        private void BufferByte(byte value)
        {
            ReserveBuffer(1);
            _buffer[0] = value;
        }

        private void BufferArray(byte[] value, int offset, int length)
        {
            ReserveBuffer(value.Length);
            Array.Copy(value, offset, _buffer, 0, length);
        }

        private void BufferShort(ushort value)
        {
//            ReserveBuffer(sizeof(ushort));
            ReserveBuffer(2);
            _buffer[0] = (byte)value;
            _buffer[1] = (byte)(value >> 8);
        }

        private void BufferInt(uint value)
        {
//            ReserveBuffer(sizeof(uint));
            ReserveBuffer(4);
            _buffer[0] = (byte)value;
            _buffer[1] = (byte)(value >> 8);
            _buffer[2] = (byte)(value >> 16);
            _buffer[3] = (byte)(value >> 24);
        }

        private void ReserveBuffer(int size)
        {
            if (_buffer == null)
            {
                _buffer = new byte[size];
            }
            else if (_buffer.Length < size)
            {
                Array.Resize(ref _buffer, size);
            }

            _bufferOffset = 0;
            _bufferAvailable = size;
        }

        private List<Action> _bufferSetupActions;

        private byte[] _buffer;
        private int _bufferOffset;
        private int _bufferAvailable;

        public override bool CanRead
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool CanSeek
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool CanWrite
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override long Length
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override long Position
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
    }
}