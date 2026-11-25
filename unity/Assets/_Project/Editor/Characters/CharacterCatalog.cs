namespace Steel.Editor.Characters
{
    /// <summary>Every avatar in the game. To add one: run tools/assets/prepare_rocketbox.py, then add a line here.</summary>
    internal static class CharacterCatalog
    {
        /// <summary>The player: a young pengepul in a worn T-shirt and jeans.</summary>
        internal static readonly CharacterSpec Player = new CharacterSpec("Male_Adult_09", "m017", 1.7f);
        /// <summary>A woman in a headscarf, out shopping.</summary>
        internal static readonly CharacterSpec Shopper = new CharacterSpec("Female_Adult_06", "f202", 1.58f);
        internal static readonly CharacterSpec RedShirt = new CharacterSpec("Male_Adult_06", "m011", 1.68f);
        internal static readonly CharacterSpec Neighbour = new CharacterSpec("Male_Adult_14", "m012", 1.72f);
        /// <summary>A smelter hand in coveralls and a hard hat.</summary>
        internal static readonly CharacterSpec Worker = new CharacterSpec("Construction_Male_03", "m102", 1.74f);
    }
}
