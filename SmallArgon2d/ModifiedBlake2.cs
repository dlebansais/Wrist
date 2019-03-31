namespace SmallArgon2d
{
    internal static class ModifiedBlake2
    {
        private static ulong Rotate(ulong x, int y)
        {
            return (((x) >> (y)) ^ ((x) << (64 - (y))));
        }

        private static void ModifiedG(ulong[] v, int a, int b, int c, int d)
        {
            ulong t;
            uint va0, vb0, vc0, vd0;
            ulong va1, vb1, vc1, vd1;

            va0 = (uint)v[a];
            va1 = (ulong)va0;

            vb0 = (uint)v[b];
            vb1 = (ulong)vb0;

            t = va1 * vb1;

            v[a] = v[a] + v[b] + 2 * t;

            v[d] = Rotate(v[d] ^ v[a], 32);

            vc0 = (uint)v[c];
            vc1 = (ulong)vc0;

            vd0 = (uint)v[d];
            vd1 = (ulong)vd0;

            t = vc1 * vd1;

            //t = ((ulong)((uint)v[c])) * ((ulong)((uint)v[d]));
            v[c] = v[c] + v[d] + 2 * t;

            v[b] = Rotate(v[b] ^ v[c], 24);

            va0 = (uint)v[a];
            va1 = (ulong)va0;

            vb0 = (uint)v[b];
            vb1 = (ulong)vb0;

            t = va1 * vb1;

            //t = ((ulong)((uint)v[a])) * ((ulong)((uint)v[b]));
            v[a] = v[a] + v[b] + 2 * t;


            v[d] = Rotate(v[d] ^ v[a], 16);

            vc0 = (uint)v[c];
            vc1 = (ulong)vc0;

            vd0 = (uint)v[d];
            vd1 = (ulong)vd0;

            t = vc1 * vd1;

            //t = ((ulong)((uint)v[c])) * ((ulong)((uint)v[d]));
            v[c] = v[c] + v[d] + 2 * t;

            v[b] = Rotate(v[b] ^ v[c], 63);
        }

        public static void DoRoundColumns(ulong[] v, int i)
        {
            i *= 16;
            ModifiedG(v, i, i + 4, i + 8, i + 12);
            ModifiedG(v, i + 1, i + 5, i + 9, i + 13);
            ModifiedG(v, i + 2, i + 6, i + 10, i + 14);
            ModifiedG(v, i + 3, i + 7, i + 11, i + 15);
            ModifiedG(v, i, i + 5, i + 10, i + 15);
            ModifiedG(v, i + 1, i + 6, i + 11, i + 12);
            ModifiedG(v, i + 2, i + 7, i + 8, i + 13);
            ModifiedG(v, i + 3, i + 4, i + 9, i + 14);
        }

        public static void DoRoundRows(ulong[] v, int i)
        {
            i *= 2;
            ModifiedG(v, i, i + 32, i + 64, i + 96);
            ModifiedG(v, i + 1, i + 33, i + 65, i + 97);
            ModifiedG(v, i + 16, i + 48, i + 80, i + 112);
            ModifiedG(v, i + 17, i + 49, i + 81, i + 113);
            ModifiedG(v, i, i + 33, i + 80, i + 113);
            ModifiedG(v, i + 1, i + 48, i + 81, i + 96);
            ModifiedG(v, i + 16, i + 49, i + 64, i + 97);
            ModifiedG(v, i + 17, i + 32, i + 65, i + 112);
        }

        public static void Blake2Prime(Argon2Memory memory, LittleEndianActiveStream dataStream, int size = -1)
        {
            LittleEndianActiveStream hashStream = new LittleEndianActiveStream();

            if (size < 0 || size > (memory.Length * 8))
            {
                size = memory.Length * 8;
            }

            hashStream.Expose(size);
            hashStream.Expose(dataStream);


            if (size <= 64)
            {
                HMACBlake2B blake2 = new HMACBlake2B(8 * size);
                blake2.Initialize();

                memory.Blit(blake2.ComputeHash(hashStream), 0, 0, size);
            }
            else
            {
                HMACBlake2B blake2 = new HMACBlake2B(512);
                blake2.Initialize();

                int offset = 0;
                byte[] chunk = blake2.ComputeHash(hashStream);

                memory.Blit(chunk, offset, 0, 32); // copy half of the chunk
                offset += 4;
                size -= 32;

                while (size > 64)
                {
                    blake2.Initialize();
                    chunk = blake2.ComputeHash(chunk);
                    memory.Blit(chunk, offset, 0, 32); // half again

                    offset += 4;
                    size -= 32;
                }

                blake2 = new HMACBlake2B(size * 8);
                blake2.Initialize();
                memory.Blit(blake2.ComputeHash(chunk), offset, 0, size); // copy the rest
            }
        }
    }
}