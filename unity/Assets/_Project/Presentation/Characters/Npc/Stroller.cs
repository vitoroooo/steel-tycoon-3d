using UnityEngine;

namespace Steel.Presentation.Characters.Npc
{
    /// <summary>Walks a bystander round a loop of waypoints: eases off, pauses, turns smoothly and waits for anything in the way.</summary>
    public sealed class Stroller : MonoBehaviour
    {
        public Vector3[] Route;
        public float Pace = 1.3f, PauseSeconds = 3;
        private const float Acceleration = 2.2f, TurnTime = .28f, Arrive = .25f, Courtesy = 1.1f, BodyRadius = .3f;
        private const float StepReach = .6f, Settle = .08f;

        private int next;
        private readonly RaycastHit[] groundHits = new RaycastHit[16];
        private float speed, wait, turnRate, fallRate;

        private void Start() => next = Route.Length > 1 ? 1 : 0;

        private void Update()
        {
            Vector3 to = Route[next] - transform.position; to.y = 0;
            float distance = to.magnitude, dt = Time.deltaTime;
            if (wait > 0) wait -= dt;
            else if (distance < Arrive)
            {
                // Every other waypoint is a place to stop and look around.
                if (next % 2 == 0) wait = PauseSeconds * Random.Range(.6f, 1.4f);
                next = (next + 1) % Route.Length;
            }
            bool blocked = Blocked();
            // Brake early enough to stop on the mark: v² = 2·a·d.
            float wanted = wait > 0 || blocked ? 0 : Mathf.Min(Pace, Mathf.Sqrt(2 * Acceleration * distance));
            speed = Mathf.MoveTowards(speed, wanted, Acceleration * (blocked ? 3 : 1) * dt);
            if (distance > .01f && wait <= 0)
            {
                float heading = Mathf.Atan2(to.x, to.z) * Mathf.Rad2Deg;
                float yaw = Mathf.SmoothDampAngle(transform.eulerAngles.y, heading, ref turnRate, TurnTime);
                transform.rotation = Quaternion.Euler(0, yaw, 0);
            }
            Vector3 position = transform.position + transform.forward * speed * dt;
            position.y = Mathf.SmoothDamp(position.y, Ground(position), ref fallRate, Settle);
            transform.position = position;
        }

        private bool Blocked()
        {
            Vector3 chest = transform.position + Vector3.up;
            return Physics.SphereCast(chest, BodyRadius, transform.forward, out RaycastHit hit, Courtesy,
                ~0, QueryTriggerInteraction.Ignore) && !hit.transform.IsChildOf(transform);
        }

        /// <summary>Height of the pavement under the feet, so kerbs are stepped up and down.</summary>
        private float Ground(Vector3 at)
        {
            Vector3 knee = new Vector3(at.x, at.y + StepReach, at.z);
            float top = float.MinValue;
            int count = Physics.RaycastNonAlloc(knee, Vector3.down, groundHits, StepReach * 2, ~0, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < count; i++)
                if (!groundHits[i].transform.IsChildOf(transform)) top = Mathf.Max(top, groundHits[i].point.y);
            if (top > float.MinValue) return top;
            return at.y;
        }
    }
}
