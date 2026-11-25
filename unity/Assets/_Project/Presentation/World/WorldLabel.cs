using UnityEngine;

namespace Steel.Presentation.World
{
    public sealed class WorldLabel : MonoBehaviour
    {
        private Camera eye;
        private Renderer mesh;
        private float nextRange;
        private void Awake() => mesh = GetComponent<Renderer>();
        private void LateUpdate()
        {
            if (!eye && !(eye = Camera.main)) return;
            if (Time.time >= nextRange)
            {
                mesh.enabled = (transform.position - eye.transform.position).sqrMagnitude < 400;
                nextRange = Time.time + .12f;
            }
            if (mesh.enabled) transform.rotation = eye.transform.rotation;
        }
    }
}
