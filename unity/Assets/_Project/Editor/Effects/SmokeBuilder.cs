using UnityEditor;
using UnityEngine;

namespace Steel.Editor.Effects
{
    /// <summary>Chimney smoke: slow puffs that rise, swell, lean with the wind and thin out.</summary>
    internal static class SmokeBuilder
    {
        internal static readonly Vector3 Wind = new Vector3(1.6f, 0, .7f);   // metres per second, matches the cloud drift

        internal static void Create(string name, Vector3 mouth, float width, Color tint, float lifetime = 15)
        {
            var go = new GameObject(name); go.transform.SetParent(CityGeometry.Root, false);
            go.transform.SetPositionAndRotation(mouth, Quaternion.Euler(-90, 0, 0));   // the cone points up
            var smoke = go.AddComponent<ParticleSystem>();
            var main = smoke.main;
            main.startLifetime = new ParticleSystem.MinMaxCurve(lifetime * .75f, lifetime);
            main.startSpeed = new ParticleSystem.MinMaxCurve(1.8f, 2.6f);
            main.startSize = new ParticleSystem.MinMaxCurve(width * 1.1f, width * 1.7f);
            main.startRotation = new ParticleSystem.MinMaxCurve(0, Mathf.PI * 2);
            main.startColor = tint; main.maxParticles = 200; main.prewarm = true;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            var emission = smoke.emission; emission.rateOverTime = 10;
            var shape = smoke.shape; shape.shapeType = ParticleSystemShapeType.Cone; shape.angle = 6; shape.radius = width * .4f;
            var drift = smoke.velocityOverLifetime; drift.enabled = true; drift.space = ParticleSystemSimulationSpace.World;
            drift.x = new ParticleSystem.MinMaxCurve(Wind.x * .7f, Wind.x * 1.3f);
            drift.y = new ParticleSystem.MinMaxCurve(-.2f, .3f);
            drift.z = new ParticleSystem.MinMaxCurve(Wind.z * .7f, Wind.z * 1.3f);
            var grow = smoke.sizeOverLifetime; grow.enabled = true;
            grow.size = new ParticleSystem.MinMaxCurve(1, AnimationCurve.EaseInOut(0, .7f, 1, 3.6f));
            var spin = smoke.rotationOverLifetime; spin.enabled = true;
            spin.z = new ParticleSystem.MinMaxCurve(-.25f, .25f);
            var fade = smoke.colorOverLifetime; fade.enabled = true;
            var gradient = new Gradient();
            gradient.SetKeys(new[] { new GradientColorKey(Color.white, 0), new GradientColorKey(Color.white, 1) },
                new[] { new GradientAlphaKey(0, 0), new GradientAlphaKey(.9f, .08f), new GradientAlphaKey(.45f, .6f), new GradientAlphaKey(0, 1) });
            fade.color = gradient;
            var renderer = go.GetComponent<ParticleSystemRenderer>();
            renderer.sharedMaterial = FxMaterials.Sprite("Asap", "smoke_puff", false);
            renderer.sortingFudge = 2;
        }
    }
}
