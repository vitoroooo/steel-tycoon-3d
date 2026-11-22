using UnityEngine;

namespace Steel.Presentation.Characters.Toon
{
    /// <summary>What a bystander is doing while standing in one place.</summary>
    public enum Stance { Stand, Talk, Sit, Squat, Work }

    /// <summary>Looping poses for cartoon bystanders, added on top of the rest pose. Angles are degrees.</summary>
    internal static class ToonStances
    {
        internal static void Apply(ToonJoints joints, Stance stance, float time)
        {
            for (int side = 0; side < 2; side++) Limbs(joints, stance, side, time);
            switch (stance)
            {
                case Stance.Stand: Torso(joints, 0, Mathf.Sin(time * .5f) * 2.5f, Mathf.Sin(time * .6f) * 22); break;
                case Stance.Talk: Torso(joints, Mathf.Sin(time * 2.6f) * 3, Mathf.Sin(time * .8f) * 3, Mathf.Sin(time * .4f) * 8); break;
                case Stance.Sit: Torso(joints, -4, 0, Mathf.Sin(time * .45f) * 18); break;
                case Stance.Squat: Torso(joints, 14, 0, Mathf.Sin(time * .5f) * 14); break;
                case Stance.Work: Torso(joints, 20 + Mathf.Sin(time * 2.2f) * 5, 0, 0); break;
            }
        }

        private static void Limbs(ToonJoints joints, Stance stance, int side, float time)
        {
            float turn = time + side * Mathf.PI;   // the two sides never move together
            float busy = Mathf.SmoothStep(0, 1, .5f + Mathf.Sin(turn * .8f));   // this hand's turn to gesture
            float swing = Mathf.Sin(time * 2.2f + side * .4f);   // both hands on one tool
            switch (stance)
            {
                case Stance.Stand: Arm(joints, side, -4, -12 + Mathf.Sin(turn * .7f) * 4, 0); break;
                case Stance.Talk:
                    Arm(joints, side, Mathf.Lerp(-6, -38, busy),
                        Mathf.Lerp(-14, -92, busy) + Mathf.Sin(turn * 4) * 9 * busy, Mathf.Lerp(0, -12, busy));
                    break;
                case Stance.Sit: Leg(joints, side, -85, 85 + Mathf.Sin(turn * .9f) * 7); Arm(joints, side, -25, -30, -18); break;
                case Stance.Squat: Leg(joints, side, -105, 125); Arm(joints, side, -45, -25 + Mathf.Sin(turn * .7f) * 5, -15); break;
                case Stance.Work: Arm(joints, side, -50 + swing * 22, -45 - swing * 20, -10); break;
            }
        }

        /// <summary>Shoulder forward (negative) and away from the body, then the elbow. The modelled arms
        /// already hang a little outward, so resting a hand on a knee needs a negative <paramref name="outward"/>.</summary>
        private static void Arm(ToonJoints joints, int side, float upperArm, float forearm, float outward)
        {
            ToonJoints.Bend(joints.UpperArms[side], upperArm, 0, side == 0 ? -outward : outward);
            ToonJoints.Bend(joints.Forearms[side], forearm, 0, 0);
        }

        /// <summary>Thigh forward and calf back, with the sole kept level.</summary>
        private static void Leg(ToonJoints joints, int side, float thigh, float calf)
        {
            ToonJoints.Bend(joints.Thighs[side], thigh, 0, 0);
            ToonJoints.Bend(joints.Calves[side], calf, 0, 0);
            ToonJoints.Bend(joints.Feet[side], -(thigh + calf), 0, 0);
        }

        /// <summary>Leans the spine and lets the head stay level, look around and counter the sway.</summary>
        private static void Torso(ToonJoints joints, float lean, float sway, float look)
        {
            ToonJoints.Bend(joints.Spine, lean, 0, sway);
            ToonJoints.Bend(joints.Head, -lean * .6f, look, -sway);
        }
    }
}
