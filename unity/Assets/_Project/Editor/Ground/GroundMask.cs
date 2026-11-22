using UnityEngine;

namespace Steel.Editor.Ground
{
    /// <summary>Marks the ground cells hidden under slabs and buildings, so nothing is planted through them.</summary>
    internal sealed class GroundMask
    {
        private const float Margin = .35f;      // keep-clear strip around every footprint
        private const float SlabHeight = .6f, SlabArea = 1, BuildingArea = 12;

        private readonly bool[,] covered;
        private readonly int cells;

        /// <param name="cells">Cells per side, matching the grid the caller plants on.</param>
        internal GroundMask(int cells)
        {
            this.cells = cells;
            covered = new bool[cells, cells];
            foreach (var renderer in Object.FindObjectsOfType<MeshRenderer>())
            {
                if (renderer.transform.parent == Plan.Strip.Parent)
                {
                    // A street mesh spans a whole sector; only the ground under its own quads is covered.
                    foreach (Bounds quad in Plan.StripMesh.Footprints(renderer.GetComponent<MeshFilter>().sharedMesh)) Mark(quad);
                    continue;
                }
                Bounds bounds = renderer.bounds;
                if (bounds.min.y > .5f) continue;                       // overhead: wires, canopies, signs
                float area = bounds.size.x * bounds.size.z;
                bool slab = bounds.size.y < SlabHeight && area > SlabArea;
                if (slab || area > BuildingArea) Mark(bounds);          // small props and plants may stand in grass
            }
        }

        internal bool Covers(int row, int column) => covered[row, column];

        /// <summary>True when anything is built within <paramref name="radius"/> metres of a world position.</summary>
        internal bool Covers(float x, float z, float radius)
        {
            int left = Cell(x - radius, GroundZones.Centre.x), right = Cell(x + radius, GroundZones.Centre.x);
            int south = Cell(z - radius, GroundZones.Centre.z), north = Cell(z + radius, GroundZones.Centre.z);
            for (int row = south; row <= north; row++)
                for (int column = left; column <= right; column++)
                    if (covered[row, column]) return true;
            return false;
        }

        private void Mark(Bounds bounds)
        {
            int left = Cell(bounds.min.x - Margin, GroundZones.Centre.x), right = Cell(bounds.max.x + Margin, GroundZones.Centre.x);
            int south = Cell(bounds.min.z - Margin, GroundZones.Centre.z), north = Cell(bounds.max.z + Margin, GroundZones.Centre.z);
            for (int row = south; row <= north; row++)
                for (int column = left; column <= right; column++)
                    covered[row, column] = true;
        }

        private int Cell(float world, float centre) =>
            Mathf.Clamp(Mathf.FloorToInt(((world - centre) / GroundZones.Size + .5f) * cells), 0, cells - 1);
    }
}
