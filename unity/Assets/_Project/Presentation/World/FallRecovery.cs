using System;
using UnityEngine;
using Steel.Services;

namespace Steel.Presentation.World
{
    public sealed class FallRecovery : MonoBehaviour
    {
        public Vector3 SpawnOffset;
        public event Action Recovered;
        private GameHost host;
        private void Start() => host = FindObjectOfType<GameHost>();
        private void LateUpdate()
        {
            if (!host.Game.NeedsRespawn(transform.position.y)) return;
            var layout = host.Layout;
            Vector3 p = new Vector3(layout.Spawn.X, layout.SpawnY, layout.Spawn.Z) + SpawnOffset;
            var controller = GetComponent<CharacterController>();
            bool enabled = controller != null && controller.enabled;
            if (controller != null) controller.enabled = false;
            transform.position = p;
            var body = GetComponent<Rigidbody>();
            if (body != null)
            {
                body.position = p;
                if (!body.isKinematic) { body.velocity = Vector3.zero; body.angularVelocity = Vector3.zero; }
            }
            if (controller != null) controller.enabled = enabled;
            if (controller != null) host.Game.SetPosition(p.x, p.z);
            Recovered?.Invoke();
        }
    }
}
