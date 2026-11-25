using System;
using UnityEngine;

namespace Steel.Services
{
    public sealed class ResourceConfigSource : IConfigSource
    {
        public string Read(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException(nameof(name));
            TextAsset asset = Resources.Load<TextAsset>("Config/" + name);
            if (asset == null) throw new InvalidOperationException("Missing config: " + name);
            return asset.text;
        }
    }
}
