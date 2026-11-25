using UnityEngine;
using Steel.Presentation.Characters.Toon;

namespace Steel.Presentation.Characters.Npc
{
    /// <summary>Keeps a bystander alive on the spot: breathing plus one looping stance, out of step with the neighbours.</summary>
    public sealed class Bystander : MonoBehaviour
    {
        public ToonJoints Joints;
        public Stance Stance;
        [Range(0, 1), Tooltip("Where in the loop this person begins.")] public float StartAt;
        public float Tempo = 1;
        [Tooltip("Top of the bench under a sitting person, in metres above their feet.")] public float SeatHeight;
        private const float LoopSeconds = 9;

        private void LateUpdate() => Pose((Time.time + StartAt * LoopSeconds) * Tempo);

        /// <summary>The stance at one moment of its loop; also used to shoot review sheets without playing.</summary>
        public void Pose(float time)
        {
            Joints.Rest();
            ToonIdle.Breathe(Joints, time, 1);
            ToonStances.Apply(Joints, Stance, time);
            if (Stance == Stance.Sit) Joints.Sit(transform.position.y + SeatHeight);
            else Joints.Plant(transform.position.y);
        }
    }
}
