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

            MemoryStream mstream = new MemoryStream(data);
            mstream.Seek(srcOffset, SeekOrigin.Begin);
            BinaryReader reader = new BinaryReader(mstream);

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
                for (int n = 0; n < remainder; ++n)
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
            int off = Offset;
            for (int i = 0; i < 128; i++)
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
#if ARGUMENT_CHECK
                if (index < 0 || index > 128)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
#endif
                return Data[Offset + index];
            }
            set
            {
#if ARGUMENT_CHECK
                if (index < 0 || index > 128)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
#endif

                Data[Offset + index] = value;
            }
        }

        public void GetBuffer(byte[] result)
        {
            for (int i = 0; i < result.Length / 8; i++)
            {
                byte[] b = BitConverter.GetBytes(Data[Offset + i]);
                int Length = ((i + 1) * 8) <= result.Length ? 8 : (result.Length - (i * 8));
                Array.Copy(b, 0, result, i * 8, Length);
            }
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