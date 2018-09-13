using System;
using System.Collections.Generic;
using System.Linq;

namespace SmallArgon2d
{
    internal abstract class Argon2Core
    {
        public Argon2Core(int tagLine)
        {
            TagLine = tagLine;
        }

        public int MemorySize { get; set; }

        public int Iterations { get; set; }

        public byte[] AssociatedData { get; set; }

        public byte[] Salt { get; set; }

        public byte[] Secret { get; set; }

        protected abstract int Type { get; }

        // Private stuff starts here
        internal void InitializeLanesTask(int l, int s, int i, Argon2Lane[] lanes, int start)
        {
            Argon2Lane lane = lanes[l];
            int segmentLength = lane.BlockCount / 4;
            int curOffset = s * segmentLength + start;

            int prevLane = l;
            int prevOffset = curOffset - 1;
            if (curOffset == 0)
            {
                prevOffset = lane.BlockCount - 1;
            }

            IArgon2PseudoRands state = GenerateState(lanes, segmentLength, i, l, s);
            for (int c = start; c < segmentLength; ++c, curOffset++)
            {
                ulong pseudoRand = state.PseudoRand(c, prevLane, prevOffset);
                int refLane = (int)((uint)(pseudoRand >> 32) % lanes.Length);

                if (i == 0 && s == 0)
                {
                    refLane = l;
                }

                int refIndex = IndexAlpha(l == refLane, (uint)pseudoRand, lane.BlockCount, segmentLength, i, s, c);
                Argon2Memory refBlock = lanes[refLane][refIndex];
                Argon2Memory curBlock = lane[curOffset];

                Compress(curBlock, refBlock, lanes[prevLane][prevOffset]);
                prevOffset = curOffset;
            }
        }

        internal byte[] Hash(byte[] password)
        {
            Argon2Lane[] lanes = InitializeLanes(password);

            int start = 2;
            for (int i = 0; i < Iterations; ++i)
            {
                for (int s = 0; s < 4; s++)
                {
                    IEnumerable<Action> segment = Enumerable.Range(0, lanes.Length).Select(l => new Action(() =>
                    {
                        InitializeLanesTask(l, s, i, lanes, start);
                    }));

                    foreach (Action t in segment)
                        t();

                    start = 0;
                }
            }

            return Finalize(lanes);
        }

        private static void XorLanes(Argon2Lane[] lanes)
        {
            Argon2Memory data = lanes[0][lanes[0].BlockCount - 1];

            foreach (Argon2Lane lane in lanes.Skip(1))
            {
                Argon2Memory block = lane[lane.BlockCount-1];

                for (int b = 0; b < 128; ++b)
                {
                    if (!BitConverter.IsLittleEndian)
                    {
                        block[b] = (block[b] >> 56) ^
                            ((block[b] >> 40) & 0xff00UL) ^
                            ((block[b] >> 24) & 0xff0000UL) ^
                            ((block[b] >> 8) & 0xff000000UL) ^
                            ((block[b] << 8) & 0xff00000000UL) ^
                            ((block[b] << 24) & 0xff0000000000UL) ^
                            ((block[b] << 40) & 0xff000000000000UL) ^
                            ((block[b] << 56) & 0xff00000000000000UL);
                    }

                    data[b] ^= block[b];
                }
            }
        }

        private byte[] Finalize(Argon2Lane[] lanes)
        {
            XorLanes(lanes);

            LittleEndianActiveStream ds = new LittleEndianActiveStream();
            ds.Expose(lanes[0][lanes[0].BlockCount - 1]);

            ModifiedBlake2.Blake2Prime(lanes[0][1], ds, TagLine);
            byte[] result = new byte[TagLine];
            Argon2Memory memory = lanes[0][1];
            memory.GetBuffer(result);

            return result;
        }

        internal static void Compress(Argon2Memory dest, Argon2Memory refb, Argon2Memory prev)
        {
            ulong[] tmpblock = new ulong[dest.Length];
            for (int n = 0; n < 128; ++n)
            {
                tmpblock[n] = refb[n] ^ prev[n];
                dest[n] ^= tmpblock[n];
            }

            for (int i = 0; i < 8; ++i)
                ModifiedBlake2.DoRoundColumns(tmpblock, i);
            for (int i = 0; i < 8; ++i)
                ModifiedBlake2.DoRoundRows(tmpblock, i);

            for (int n = 0; n < 128; ++n)
                dest[n] ^= tmpblock[n];
        }

        internal abstract IArgon2PseudoRands GenerateState(Argon2Lane[] lanes, int segmentLength, int pass, int lane, int slice);

        internal Argon2Lane[] InitializeLanes(byte[] password)
        {
            byte[] blockHash = Initialize(password);

            Argon2Lane[] lanes = new Argon2Lane[1];

            // Adjust memory size if needed so that each segment has an even size
            int segmentLength = MemorySize / (lanes.Length * 4);
            MemorySize = segmentLength * 4 * lanes.Length;
            int blocksPerLane = MemorySize / lanes.Length;

            if (blocksPerLane < 4)
            {
                throw new InvalidOperationException($"Memory should be enough to provide at least 4 blocks");
            }

            Action[] init = new Action[lanes.Length * 2];
            for (int i = 0; i < lanes.Length; ++i)
            {
                lanes[i] = new Argon2Lane(blocksPerLane);

                int taskIndex = i * 2;
                int iClosure = i;
                init[taskIndex] = () => {
                    LittleEndianActiveStream stream = new LittleEndianActiveStream();
                    stream.Expose(blockHash);
                    stream.Expose(0);
                    stream.Expose(iClosure);

                    ModifiedBlake2.Blake2Prime(lanes[iClosure][0], stream);
                };

                init[taskIndex + 1] = () => {
                    LittleEndianActiveStream stream = new LittleEndianActiveStream();
                    stream.Expose(blockHash);
                    stream.Expose(1);
                    stream.Expose(iClosure);

                    ModifiedBlake2.Blake2Prime(lanes[iClosure][1], stream);
                };
            }

            foreach (Action t in init)
                t();

            Array.Clear(blockHash, 0, blockHash.Length);
            return lanes;
        }

        internal byte[] Initialize(byte[] password)
        {
            // Initialize the lanes
            HMACBlake2B blake2 = new HMACBlake2B(512);
            LittleEndianActiveStream dataStream = new LittleEndianActiveStream();

            dataStream.Expose(1);
            dataStream.Expose(TagLine);
            dataStream.Expose(MemorySize);
            dataStream.Expose(Iterations);
            dataStream.Expose((uint)0x13);
            dataStream.Expose((uint)Type);
            dataStream.Expose(password.Length);
            dataStream.Expose(password);
            dataStream.Expose(Salt?.Length ?? 0);
            dataStream.Expose(Salt);
            dataStream.Expose(Secret?.Length ?? 0);
            dataStream.Expose(Secret);
            dataStream.Expose(AssociatedData?.Length ?? 0);
            dataStream.Expose(AssociatedData);

            blake2.Initialize();
            byte[] blockhash = blake2.ComputeHash(dataStream);

            dataStream.ClearBuffer();
            return blockhash;
        }

        private static int IndexAlpha(bool sameLane, uint pseudoRand, int laneLength, int segmentLength, int pass, int slice, int index)
        {
            uint refAreaSize;
            if (pass == 0)
            {
                if (slice == 0)
                    refAreaSize = (uint)index - 1;
                else if (sameLane)
                    refAreaSize = (uint)(slice * segmentLength) + (uint)index - 1;
                else
                    refAreaSize = (uint)(slice * segmentLength) - ((index == 0) ? 1U : 0);
            }
            else if (sameLane)
                refAreaSize = (uint)laneLength - (uint)segmentLength + (uint)index - 1;
            else
                refAreaSize = (uint)laneLength - (uint)segmentLength - ((index == 0) ? 1U : 0);

            ulong relativePos = pseudoRand;
            relativePos = relativePos * relativePos >> 32;
            relativePos = refAreaSize - 1 - (refAreaSize * relativePos >> 32);

            uint startPos = 0;
            if (pass != 0)
                startPos = (slice == 3) ? 0 : ((uint)slice + 1U) * (uint)segmentLength;

            return (int)(((ulong)startPos + relativePos) % (ulong)laneLength);
        }

        private int TagLine;
    }
}