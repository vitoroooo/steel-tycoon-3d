namespace Steel.Editor.Characters.Toon
{
    /// <summary>Everyone who can stand in Sukabaja. To add one: a Look in tools/blender/kartun/warga, then a line here.</summary>
    internal sealed class ToonCitizen
    {
        /// <summary>A woman in a hijab with her market bag.</summary>
        internal static readonly ToonCitizen Shopper = new ToonCitizen("Warga_ibu", 1.58f);
        /// <summary>The neighbour in a peci and sarong-green trousers.</summary>
        internal static readonly ToonCitizen Neighbour = new ToonCitizen("Warga_tetangga", 1.76f);
        /// <summary>A young man in a red football shirt.</summary>
        internal static readonly ToonCitizen RedShirt = new ToonCitizen("Warga_pemuda", 1.64f);
        /// <summary>A smelter or demolition hand: hard hat, vest and boots.</summary>
        internal static readonly ToonCitizen Worker = new ToonCitizen("Warga_pekerja", 1.78f);

        internal readonly string Model;
        internal readonly float HeightMetres;

        private ToonCitizen(string file, float heightMetres)
        {
            Model = "Characters/" + file;
            HeightMetres = heightMetres;
        }
    }
}
