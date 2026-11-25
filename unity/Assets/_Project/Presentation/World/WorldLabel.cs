using UnityEngine;

namespace Steel.Presentation.World
{
    public sealed class WorldLabel : MonoBehaviour
    {
        private void LateUpdate()
        {
            if (Camera.main)
            {
                transform.rotation = Camera.main.transform.rotation;
                GetComponent<Renderer>().enabled = Vector3.Distance(transform.position, Camera.main.transform.position) < 20;
            }
        }
    }
}
