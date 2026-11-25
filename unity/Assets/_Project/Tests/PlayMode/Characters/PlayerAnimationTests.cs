using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Steel.Presentation.Characters;
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
            Transform hips = visual.Find("Bip01");
            Transform left = Find(visual, "Bip01 L Foot"), right = Find(visual, "Bip01 R Foot");
            Assert.IsNotNull(visual.GetComponent<HipsAnchor>());
            var animator = visual.GetComponent<Animator>();
            Assert.IsNotNull(animator.runtimeAnimatorController, "Avatar lost its animator controller");
            float widestStride = 0, worstDrift = 0;
            for (float end = Time.time + 1.5f; Time.time < end;)
            {
                // Poses are read before this frame's move: they were pinned in the last LateUpdate.
                yield return null;
                Vector3 drift = hips.position - visual.position;
                drift.y = 0;
                worstDrift = Mathf.Max(worstDrift, drift.magnitude);
                widestStride = Mathf.Max(widestStride, Vector3.Distance(left.position, right.position));
                mover.transform.position += Vector3.right * WalkSpeed * Time.deltaTime;
            }
            Assert.Greater(animator.GetFloat("Speed"), 1, "Measured ground speed never reached the animator");
            Assert.Less(worstDrift, .02f, "Hips drifted away: the clip travel is not cancelled");
            Assert.Greater(widestStride, .45f, "Legs never strode: the walk clip is not playing");
        }

        private static Transform Find(Transform root, string name)
        {
            foreach (Transform child in root.GetComponentsInChildren<Transform>())
                if (child.name == name) return child;
            return null;
        }
    }
}
