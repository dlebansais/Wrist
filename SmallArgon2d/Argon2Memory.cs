using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace SmallArgon2d
{
    internal class Argon2Memory : IEnumerable<ulong>
    {
        public Argon2Memory(ulong[] data, int offset)
        {
            Data = data;
            Offset = offset;
        }

        public int Length { get { return 128; } }

        public void Blit(byte[] data, int destOffset = 0, int srcOffset = 0, int byteLength = -1)
        {
            int remainder = 0;
            int length;
            if (byteLength < 0)
            {
                length = Length;
            }
            else
            {
                length = byteLength / 8;
                remainder = byteLength - (length * 8);
            }

            int readSize = Math.Min((data.Length / 8), length);

            var mstream = new MemoryStream(data);
            mstream.Seek(srcOffset, SeekOrigin.Begin);
            var reader = new BinaryReader(mstream);

            readSize += destOffset;
            int i = destOffset;
            for (; i < readSize; ++i)
            {
                this[i] = reader.ReadUInt64();
            }

            if (remainder > 0)
            {
                ulong extra = 0;

                // get the remainder as a few bytes
                for (var n = 0; n < remainder; ++n)
                    extra = extra | ((ulong)reader.ReadByte() << (8 * n));

                this[i++] = extra;
            }

            for (; i < length; ++i)
            {
                this[i] = 0;
            }
        }

        public void Set(ulong value)
        {
            var off = Offset;
            for (var i = 0; i < 128; i++)
            {
                Data[off++] = value;
            }
        }

        public IEnumerator<ulong> GetEnumerator()
        {
            return new Enumerator(Data, Offset);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(Data, Offset);
        }

        public ulong this[int index]
        {
            get
            {
                if (index < 0 || index > 128)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                return Data[Offset + index];
            }
            set
            {
                if (index < 0 || index > 128)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                Data[Offset + index] = value;
            }
        }

        internal unsafe class Stream : UnmanagedMemoryStream
        {
            public Stream(Argon2Memory memory)
            {
                Data = GCHandle.Alloc(memory.Data, GCHandleType.Pinned);
                base.Initialize((byte*)Data.AddrOfPinnedObject() + (memory.Offset * 8), 1024, 1024, FileAccess.Read);
            }

            protected override void Dispose(bool isDispose)
            {
                base.Dispose(isDispose);
                Data.Free();
            }

            private GCHandle Data;
        }

        private class Enumerator : IEnumerator<ulong>
        {
            private int StartOffset;
            private int CurrentOffset;
            private ulong[] Data;

            public Enumerator(ulong[] data, int start)
            {
                StartOffset = start;
                Data = data;

                Reset();
            }

            public ulong Current
            {
                get
                {
                    if (CurrentOffset >= (StartOffset + 128))
                        return 0UL;

                    return Data[CurrentOffset];
                }
            }

            object IEnumerator.Current { get { return (object)this.Current; } }

            public bool MoveNext()
            {
                if (++CurrentOffset >= (StartOffset + 128))
                {
                    return false;
                }

                return true;
            }

            public void Reset()
            {
                CurrentOffset = StartOffset - 1;
            }

            public void Dispose()
            {
            }
        }

        private ulong[] Data;
        private int Offset;
    }
}