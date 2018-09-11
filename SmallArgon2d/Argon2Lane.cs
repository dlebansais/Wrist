using System;

namespace SmallArgon2d
{
    internal class Argon2Lane
    {
        public Argon2Lane(int blockCount)
        {
            Memory = new ulong[128 * blockCount];
        }

        public Argon2Memory this[int index]
        {
            get
            {
                if (index < 0 || index > BlockCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                return new Argon2Memory(Memory, 128 * index);
            }
        }

        public int BlockCount { get { return Memory.Length / 128; } }

        private ulong[] Memory;
    }
}