namespace Steel.Core.State
{
    /// <summary>Rolls that follow a saved sequence number, so reloading a career cannot roll again for a better result.</summary>
    public static class Dice
    {
        /// <summary>The next number of the sequence, from 0 up to but not including 1.</summary>
        public static double Next(ref int sequence)
        {
            sequence = sequence == int.MaxValue ? 1 : sequence + 1;
            return Unit(unchecked((uint)sequence));
        }

        public static bool Roll(ref int sequence, double chance) => chance > 0 && Next(ref sequence) < chance;

        /// <summary>The same number for the same key every time: for things the whole city shares, such as a game day.</summary>
        public static double Unit(uint key)
        {
            uint hash = unchecked(key * 747796405u + 2891336453u);
            hash = unchecked(((hash >> (int)((hash >> 28) + 4)) ^ hash) * 277803737u);
            return ((hash >> 22) ^ hash) / 4294967296d;
        }
    }
}
