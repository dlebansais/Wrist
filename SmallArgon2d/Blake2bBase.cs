using System;

namespace SmallArgon2d
{
    internal abstract class Blake2bBase
    {
        public Blake2bBase(int hashBytes)
        {
            HashSize = (uint)hashBytes;
        }

        public int ByteSize
        {
            get
            {
                return (int)HashSize;
            }
        }

        public void Initialize(byte[] key)
        {
            if ((key?.Length ?? 0) > B.Length)
                throw new ArgumentException($"Blake2 key size is too large. Max size is {B.Length} bytes", nameof(key));

            Array.Copy(Blake2Constants.IV, H, 8);
            H[0] ^= 0x01010000UL ^ (((ulong)(key?.Length ?? 0)) << 8) ^ HashSize;

            // Start with the key
            if (key?.Length > 0)
            {
                Array.Copy(key, B, key.Length);
                Update(B, 0, B.Length);
            }
        }

        public void Update(byte[] data, int offset, int size)
        {
            while (size > 0)
            {
                if (C == 128)
                {
                    T[0] += (ulong)C;
                    if (T[0] < (ulong)C)
                        ++T[1];

                    // We filled our buffer
                    this.Compress(false);
                    C = 0;
                }
                
                int nextChunk = Math.Min(size, 128 - C);

                // Copy the next batch of data
                Array.Copy(data, offset, B, C, nextChunk);
                C += nextChunk;
                offset += nextChunk;

                size -= nextChunk;
            }
        }

        public byte[] Final()
        {
            T[0] += (ulong)C;
            if (T[0] < (ulong)C)
                ++T[1];

            while (C < 128)
                B[C++] = 0;
            C = 0;

            this.Compress(true);
            uint hashByteSize = HashSize;
            byte[] result = new byte[hashByteSize];
            for (int i = 0; i < hashByteSize; ++i)
            {
                result[i] = (byte)((H[i >> 3] >> (8 * (i & 7))) & 0xff);
            }

            return result;
        }

        public abstract void Compress(bool isFinal);

        protected ulong[] Hash { get { return H; } }

        protected ulong TotalSegmentsLow { get { return T[0]; } }

        protected ulong TotalSegmentsHigh { get { return T[1]; } }

        protected byte[] DataBuffer { get { return B; } }

        private ulong[] H = new ulong[8];
        private ulong[] T = new ulong[2];
        private byte[] B = new byte[128];
        private int C;
        private uint HashSize;
    }
}
