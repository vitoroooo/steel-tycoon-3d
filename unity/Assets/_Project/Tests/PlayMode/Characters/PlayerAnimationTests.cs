using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Steel.Presentation.Characters;
using Steel.Presentation.Characters.Toon;
using Steel.Presentation.Player;

namespace Steel.PlayTests.Characters
{
    public sealed class PlayerAnimationTests
    {
        private const float WalkSpeed = 1.4f;

        [UnityTest]
        public IEnumerator WalkingStridesWhileHipsStayOverThePlayer()
        {
            yield return SceneManager.LoadSceneAsync("City");
            yield return null;
            var mover = Object.FindObjectOfType<PlayerMover>();
            mover.enabled = false;
            mover.GetComponent<CharacterController>().enabled = false;
            Transform visual = mover.Visual;
            var gait = visual.GetComponent<ToonGait>();
            Assert.IsNotNull(gait, "The cartoon body lost its procedural gait");
            Transform hips = gait.Joints.Hips, left = gait.Joints.Feet[0], right = gait.Joints.Feet[1];
            yield return null;
            float ankle = Mathf.Min(left.position.y, right.position.y) - visual.position.y;
            float widestStride = 0, worstDrift = 0, worstLift = 0;
            for (float end = Time.time + 1.5f; Time.time < end;)
            {
                // Poses are read before this frame's move: they were set in the last LateUpdate.
                yield return null;
                Vector3 drift = hips.position - visual.position;
                drift.y = 0;
                worstDrift = Mathf.Max(worstDrift, drift.magnitude);
                widestStride = Mathf.Max(widestStride, Mathf.Abs(Vector3.Dot(left.position - right.position, visual.forward)));
                float lowest = Mathf.Min(left.position.y, right.position.y) - visual.position.y;
                worstLift = Mathf.Max(worstLift, Mathf.Abs(lowest - ankle));
                mover.transform.position += Vector3.right * WalkSpeed * Time.deltaTime;
            }
            Assert.Greater(visual.GetComponent<LocomotionAnimator>().GroundSpeed, 1, "Ground speed never reached the gait");
            Assert.Less(worstDrift, .02f, "Hips drifted away from the player");
            Assert.Greater(widestStride, .25f, "Legs never strode");
            Assert.Less(worstLift, .03f, "The lower foot left the ground or sank into it");
        }

        private static Transform Find(Transform root, string name)
        {
            foreach (Transform child in root.GetComponentsInChildren<Transform>())
                if (child.name == name) return child;
            return null;
        }
    }
}
