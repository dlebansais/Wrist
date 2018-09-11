namespace SmallArgon2d
{
    /// <summary>
    /// The implementation of Argon2d for use in the crypto library
    /// </summary>
    internal class Argon2dCore : Argon2Core
    {
        private class PseudoRands : IArgon2PseudoRands
        {
            public PseudoRands(Argon2Lane[] lanes)
            {
                Lanes = lanes;
            }

            public ulong PseudoRand(int segment, int prevLane, int prevOffset)
            {
                return Lanes[prevLane][prevOffset][0];
            }

            private Argon2Lane[] Lanes;
        }

        public Argon2dCore(int hashSize)
            : base(hashSize)
        {
        }

        protected override int Type { get { return 0; } }

        internal override IArgon2PseudoRands GenerateState(Argon2Lane[] lanes, int segmentLength, int pass, int lane, int slice)
        {
            return new PseudoRands(lanes);
        }
    }
}