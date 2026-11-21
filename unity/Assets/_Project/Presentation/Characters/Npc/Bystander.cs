using UnityEngine;

namespace Steel.Presentation.Characters.Npc
{
    /// <summary>Starts a bystander's looping pose part-way through and at its own tempo, so neighbours never move in step.</summary>
    [RequireComponent(typeof(Animator))]
    public sealed class Bystander : MonoBehaviour
    {
        [Range(0, 1)] public float StartAt;
        public float Tempo = 1;

        private void Start()
        {
            var animator = GetComponent<Animator>();
            animator.speed = Tempo;
            animator.Play(0, 0, StartAt);
        }
    }
}
