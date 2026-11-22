using UnityEngine;

namespace Steel.Presentation.Characters.Toon
{
    /// <summary>The small motions of a body at rest: breathing, a wandering glance, loose arms.</summary>
    internal static class ToonIdle
    {
        private const float BreathRate = 1.7f, GlanceRate = .45f;

        /// <param name="weight">1 when standing still, 0 at full walking pace.</param>
        internal static void Breathe(ToonJoints joints, float time, float weight)
        {
            float breath = Mathf.Sin(time * BreathRate);
            ToonJoints.Bend(joints.Spine, breath * 1.2f, 0, 0);
            ToonJoints.Bend(joints.Head, -breath * .8f, Mathf.Sin(time * GlanceRate) * 6 * weight,
                Mathf.Sin(time * GlanceRate * 1.3f) * 1.5f * weight);
            for (int side = 0; side < 2; side++)
            {
                float outward = side == 0 ? -1 : 1;
                ToonJoints.Bend(joints.UpperArms[side], 0, 0, outward * (2 + breath * 1.5f));
                ToonJoints.Bend(joints.Forearms[side], -6 * weight, 0, 0);
            }
        }
    }
}
