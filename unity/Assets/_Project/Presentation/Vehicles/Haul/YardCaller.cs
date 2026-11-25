using System.Globalization;
using UnityEngine;
using Steel.Presentation.Characters;
using Steel.Services;

namespace Steel.Presentation.Vehicles.Haul
{
    /// <summary>A collector who pushes his cart to the yard's scale whenever the yard buys a lot, waits for the reading and leaves.</summary>
    public sealed class YardCaller : MonoBehaviour
    {
        private enum Stage { Away, Coming, Weighing, Leaving }
        private const float Pace = 1.3f, WeighSeconds = 4, Poll = .5f, WheelRadius = .33f;
        public Transform Collector, WheelLeft, WheelRight, GripLeft, GripRight;
        public ArmReach Arms;
        /// <summary>The scale's readout: grade, weight and what was paid for the last lot.</summary>
        public TextMesh Readout;
        /// <summary>Where the collector comes in from the street and where the scale is, on this lot.</summary>
        public Vector3 Gate, Scale;
        private GameHost host;
        private Stage stage;
        private long seen;
        private float wait, roll;

        private void Start()
        {
            host = FindObjectOfType<GameHost>();
            seen = host.Game.LastVisit.Serial;   // lots bought before this scene opened have already left
            Collector.gameObject.SetActive(false);
        }

        private void Update()
        {
            wait -= Time.deltaTime;
            if (stage == Stage.Away && wait <= 0)
            {
                wait = Poll;
                if (host.Game.LastVisit.Serial != seen) Arrive();
            }
            else if (stage == Stage.Coming && Walk(Scale)) { stage = Stage.Weighing; wait = WeighSeconds; Read(); }
            else if (stage == Stage.Weighing && wait <= 0) stage = Stage.Leaving;
            else if (stage == Stage.Leaving && Walk(Gate)) { stage = Stage.Away; Collector.gameObject.SetActive(false); }
        }

        private void Arrive()
        {
            seen = host.Game.LastVisit.Serial; stage = Stage.Coming;
            Collector.localPosition = Gate; Collector.gameObject.SetActive(true);
            Arms.Hold(GripLeft, GripRight);
        }

        private void Read()
        {
            YardVisit visit = host.Game.LastVisit; var id = CultureInfo.GetCultureInfo("id-ID");
            Readout.text = visit.Grade + "  " + visit.Kg.ToString("N0", id) + " kg\nRp" + visit.Paid.ToString("N0", id);
        }

        /// <summary>One frame of pushing the cart towards a spot on the lot; true once there.</summary>
        private bool Walk(Vector3 to)
        {
            Vector3 from = Collector.localPosition, next = Vector3.MoveTowards(from, to, Pace * Time.deltaTime);
            if ((to - from).sqrMagnitude > .0001f)
                Collector.localRotation = Quaternion.Slerp(Collector.localRotation, Quaternion.LookRotation(to - from), 8 * Time.deltaTime);
            roll = (roll + (next - from).magnitude / WheelRadius * Mathf.Rad2Deg) % 360;
            WheelLeft.localRotation = WheelRight.localRotation = Quaternion.Euler(roll, 0, 0);
            Collector.localPosition = next;
            return next == to;
        }
    }
}
