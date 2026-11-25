using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Steel.Editor.Models
{
    /// <summary>Models built by tools/blender: already in metres, facing +Z, with named slots.</summary>
    internal static class AuthoredModel
    {
        private const string Root = "Assets/_Project/Models/";

        /// <summary>Instantiates the model and swaps every slot for the material that paint returns.</summary>
        internal static GameObject Create(string path, Func<string, Material> paint)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(Root + path + ".fbx");
            if (prefab == null) throw new InvalidOperationException("Model not built: " + path);
            var model = UnityEngine.Object.Instantiate(prefab);
            model.name = prefab.name;
            // The FBX root carries the exporter's half turn; without it the nose points down +Z.
            model.transform.rotation = Quaternion.identity;
            foreach (var renderer in model.GetComponentsInChildren<MeshRenderer>())
                renderer.sharedMaterials = renderer.sharedMaterials.Select(m => paint(m.name)).ToArray();
            return model;
        }
    }
}
