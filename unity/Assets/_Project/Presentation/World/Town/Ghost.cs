using UnityEngine;
using Steel.Services.Remote;

namespace Steel.Presentation.World.Town
{
    /// <summary>Another player in the shared city. The server tells where he is every few seconds; in between he
    /// walks there at the pace that brings him in when the next report is due, so he never jumps.</summary>
    public sealed class Ghost : MonoBehaviour
    {
        public TextMesh Label;
        /// <summary>Further than this from where he was shown he is simply put there: he came from out of sight.</summary>
        private const float Leap = 60, TurnTime = .25f, Drop = 6;
        private Vector3 target;
        private float pace, turnRate;
        public string Player { get; private set; }

        public void Show(RemotePlayer seen, float secondsToNext)
        {
            target = new Vector3(seen.x, transform.position.y, seen.z);
            Vector3 to = target - transform.position; to.y = 0;
            if (!gameObject.activeSelf || seen.name != Player || to.magnitude > Leap)
            {
                transform.position = Grounded(target); to = Vector3.zero;
                gameObject.SetActive(true);
            }
            Player = seen.name;
            pace = to.magnitude / Mathf.Max(.5f, secondsToNext);
            Label.text = string.IsNullOrEmpty(seen.vehicle) ? seen.name : seen.name + "\n(berkendara)";
        }

        public void Hide() { Player = null; gameObject.SetActive(false); }

        private void Update()
        {
            Vector3 to = target - transform.position; to.y = 0;
            if (to.sqrMagnitude < .0004f) return;
            float yaw = Mathf.SmoothDampAngle(transform.eulerAngles.y, Mathf.Atan2(to.x, to.z) * Mathf.Rad2Deg, ref turnRate, TurnTime);
            transform.rotation = Quaternion.Euler(0, yaw, 0);
            transform.position = Grounded(Vector3.MoveTowards(transform.position, transform.position + to, pace * Time.deltaTime));
        }

        /// <summary>On the pavement under him; people are on a layer rays ignore, so he never stands on himself.</summary>
        private static Vector3 Grounded(Vector3 at)
        {
            Vector3 above = new Vector3(at.x, at.y + Drop / 2, at.z);
            return Physics.Raycast(above, Vector3.down, out RaycastHit hit, Drop, Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore) ? new Vector3(at.x, hit.point.y, at.z) : at;
        }
    }
}
