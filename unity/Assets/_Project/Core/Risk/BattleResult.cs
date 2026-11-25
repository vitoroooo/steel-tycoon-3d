namespace Steel.Core.Risk
{
    /// <summary>How a fight at the gate ended: whether the gate stood, how long it took, and which posts' guards were knocked out.</summary>
    public readonly struct BattleResult
    {
        public readonly bool Held;
        public readonly int Rounds, Waves, GuardsDown;
        public readonly double GateLeft;
        public readonly bool[] Down;

        public BattleResult(bool held, int rounds, int waves, double gateLeft, bool[] down)
        {
            Held = held; Rounds = rounds; Waves = waves; GateLeft = gateLeft; Down = down;
            int count = 0;
            foreach (bool fell in down) if (fell) count++;
            GuardsDown = count;
        }
    }
}
