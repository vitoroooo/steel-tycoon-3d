using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Steel.Core.Trade.Street;

namespace Steel.Editor.Plan.Places.Doors
{
    /// <summary>Notes where each seller was built and writes Sellers.json, so the game deals exactly where the city shows a seller.</summary>
    internal sealed class DoorLedger
    {
        private const string Path = "Assets/_Project/Resources/Config/Sellers.json";
        private readonly List<(Vector3 At, SellerKind Kind, int Place)> doors = new List<(Vector3, SellerKind, int)>();

        internal int Count => doors.Count;

        /// <summary>Returns the id the game will know this door by.</summary>
        internal string Add(Vector3 at, SellerKind kind, int place)
        {
            doors.Add((at, kind, place));
            return "s" + (doors.Count - 1);
        }

        internal void Write()
        {
            var json = new StringBuilder("{\n  \"count\": " + doors.Count);
            for (int i = 0; i < doors.Count; i++)
                json.Append(",\n  \"s" + i + "x\": " + Number(doors[i].At.x) + ", \"s" + i + "z\": " + Number(doors[i].At.z) +
                    ", \"s" + i + "kind\": " + (int)doors[i].Kind + ", \"s" + i + "place\": " + doors[i].Place);
            File.WriteAllText(Path, json.Append("\n}\n").ToString());
            AssetDatabase.ImportAsset(Path);
        }

        private static string Number(float value) => value.ToString("0.##", CultureInfo.InvariantCulture);
    }
}
