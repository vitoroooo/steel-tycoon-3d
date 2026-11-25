namespace Steel.Editor.Characters
{
    /// <summary>One imported avatar: where its files live and how tall it stands in the world.</summary>
    internal readonly struct CharacterSpec
    {
        internal readonly string Folder;
        internal readonly string TexturePrefix;
        internal readonly float HeightMetres;

        internal CharacterSpec(string folder, string texturePrefix, float heightMetres)
        {
            Folder = folder;
            TexturePrefix = texturePrefix;
            HeightMetres = heightMetres;
        }

        internal string ModelPath => RocketboxImportRules.Root + Folder + "/" + Folder + ".fbx";
        internal string TexturePath(string part, string map, string extension) =>
            RocketboxImportRules.Root + Folder + "/Textures/" + TexturePrefix + "_" + part + "_" + map + extension;
    }
}
