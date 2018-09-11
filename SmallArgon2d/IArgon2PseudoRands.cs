namespace SmallArgon2d
{
    internal interface IArgon2PseudoRands
    {
        ulong PseudoRand(int segment, int prevLane, int prevOffset);
    }
}