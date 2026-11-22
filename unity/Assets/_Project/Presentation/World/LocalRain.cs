using Steel.Services;
using UnityEngine;
using UnityEngine.Rendering;

namespace Steel.Presentation.World
{
    [RequireComponent(typeof(Light))]
    public sealed class LocalRain : MonoBehaviour
    {
        private GameHost host;
        private Transform eye;
        private Light sun;
        private ParticleSystem drops;
        private Material streak;
        private float dryLight, next;
        private void Start()
        {
            host = FindObjectOfType<GameHost>(); sun = GetComponent<Light>(); dryLight = sun.intensity;
        }
        private void Update()
        {
            if (Time.unscaledTime >= next)
            {
                next = Time.unscaledTime + .35f;
                if (eye == null) eye = Camera.main?.transform;
                bool wet = host != null && host.Game.RainHere;
                if (wet && drops == null) Build();
                if (drops != null && wet != drops.isEmitting)
                { if (wet) drops.Play(); else drops.Stop(true,ParticleSystemStopBehavior.StopEmitting); }
                sun.intensity = dryLight * (wet ? .63f : 1f);
            }
            if (eye != null && drops != null && drops.isEmitting)
                drops.transform.position = eye.position + Vector3.up * 6;
        }
        private void Build()
        {
            var shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
            if (shader == null) return;
            drops = new GameObject("Hujan lokal").AddComponent<ParticleSystem>();
            drops.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
            drops.transform.rotation = Quaternion.Euler(90,0,0);
            var main = drops.main; main.startLifetime = .9f; main.startSpeed = 13; main.startSize = .022f;
            main.simulationSpace = ParticleSystemSimulationSpace.World; main.maxParticles = 160;
            main.startColor = new Color(.58f,.68f,.76f,1); var em = drops.emission; em.rateOverTime = 125;
            var shape = drops.shape; shape.shapeType = ParticleSystemShapeType.Rectangle; shape.scale = new Vector3(16,16,1);
            var r = drops.GetComponent<ParticleSystemRenderer>(); r.renderMode = ParticleSystemRenderMode.Stretch;
            r.velocityScale = .018f; r.lengthScale = 4; r.shadowCastingMode = ShadowCastingMode.Off;
            r.receiveShadows = false; streak = new Material(shader); r.sharedMaterial = streak;
        }
        private void OnDestroy()
        {
            if (drops != null) Destroy(drops.gameObject); if (streak != null) Destroy(streak);
        }
    }
}
