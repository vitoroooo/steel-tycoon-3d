using UnityEngine;

namespace Steel.Presentation.Characters.Animals
{
    public sealed class YardChicken : MonoBehaviour
    {
        public ChickenPose Pose;
        public Transform Player;
        public Vector3[] Stops;
        private float pause, speed;
        private int next;

        private void Start() { pause = 1 + Mathf.Repeat(transform.position.z, 2); next = 1; }

        private void Update()
        {
            if (!Pose.gameObject.activeInHierarchy) return;
            Vector3 away = transform.position - Player.position;
            away.y = 0;
            bool startled = away.sqrMagnitude < 2.25f;
            if (startled)
            {
                float best = -1;
                for (int i = 0; i < Stops.Length; i++)
                {
                    float distance = (Stops[i] - Player.position).sqrMagnitude;
                    if (distance > best) { best = distance; next = i; }
                }
                pause = 0;
            }
            Vector3 delta = Stops[next] - transform.position; delta.y = 0;
            if (pause <= 0 && delta.sqrMagnitude < .03f)
            {
                next = (next + 1) % Stops.Length;
                pause = 1.6f + Mathf.Repeat(Stops[next].x, 1.5f);
            }
            bool feeding = pause > 0;
            pause -= Time.deltaTime;
            float target = feeding ? 0 : startled ? 1.15f : .42f;
            speed = Mathf.MoveTowards(speed, target, Time.deltaTime * 2);
            if (!feeding && delta.sqrMagnitude > .001f)
                transform.rotation = Quaternion.RotateTowards(transform.rotation,
                    Quaternion.LookRotation(delta), 240 * Time.deltaTime);
            Vector3 direction = transform.forward, origin = transform.position + Vector3.up * .15f;
            if (speed > 0 && Physics.SphereCast(origin, .065f, direction, out _, .28f,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                next = (next + 1) % Stops.Length; speed = 0; pause = .45f;
            }
            Vector3 position = transform.position + direction * Mathf.Min(delta.magnitude, speed * Time.deltaTime);
            if (Physics.Raycast(position + Vector3.up * .35f, Vector3.down, out var hit, .7f,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                transform.position = new Vector3(position.x, hit.point.y, position.z);
            Pose.Apply(speed, feeding);
        }
    }
}
