namespace Steel.Core.Trade.Street
{
    /// <summary>Repeatable chance: the same door on the same restock round always offers the same goods, also after a reload.</summary>
    internal static class SellerRoll
    {
        /// <summary>A number from 0 up to but not including 1; each salt gives another one.</summary>
        internal static double Of(int seller, long round, int salt)
        {
            unchecked
            {
                ulong hash = (ulong)(seller + 1) * 0x9E3779B97F4A7C15UL ^ (ulong)(round + 7) * 0xC2B2AE3D27D4EB4FUL ^
                    (ulong)(salt + 3) * 0x165667B19E3779F9UL;
                hash ^= hash >> 31; hash *= 0x7FB5D329728EA185UL; hash ^= hash >> 27;
                hash *= 0x81DADEF4BC2DD44DUL; hash ^= hash >> 33;
                return (hash >> 11) / (double)(1UL << 53);
            }
        }
    }
}
