using UnityEngine;

namespace Steel.Editor.Effects
{
    /// <summary>Showers of sparks off the furnace: bursts of bright streaks that arc and fall.</summary>
    internal static class SparkBuilder
    {
        internal static void Create(Vector3 source, float reach = 5)
        {
            var go = new GameObject("Percikan tungku"); go.transform.SetParent(CityGeometry.Root, false);
            go.transform.SetPositionAndRotation(source, Quaternion.Euler(-90, 0, 0));
            var sparks = go.AddComponent<ParticleSystem>();
            var main = sparks.main;
            main.startLifetime = new ParticleSystem.MinMaxCurve(.7f, 1.6f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(reach * .6f, reach * 1.4f);
            main.startSize = new ParticleSystem.MinMaxCurve(.05f, .11f);
            main.startColor = new ParticleSystem.MinMaxGradient(new Color(1, .85f, .45f), new Color(1, .5f, .12f));
            main.gravityModifier = 1; main.maxParticles = 300;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            var emission = sparks.emission; emission.rateOverTime = 14;
            emission.SetBursts(new[] { new ParticleSystem.Burst(0, 30, 60, 0, 2.7f), new ParticleSystem.Burst(1.1f, 15, 30, 0, 4.3f) });
            var shape = sparks.shape; shape.shapeType = ParticleSystemShapeType.Cone; shape.angle = 38; shape.radius = .5f;
            var fade = sparks.colorOverLifetime; fade.enabled = true;
            var gradient = new Gradient();
            gradient.SetKeys(new[] { new GradientColorKey(Color.white, 0), new GradientColorKey(new Color(1, .35f, .1f), 1) },
                new[] { new GradientAlphaKey(1, 0), new GradientAlphaKey(1, .6f), new GradientAlphaKey(0, 1) });
            fade.color = gradient;
            var bounce = sparks.collision; bounce.enabled = true; bounce.type = ParticleSystemCollisionType.World;
            bounce.bounce = .35f; bounce.dampen = .4f; bounce.lifetimeLoss = .3f; bounce.quality = ParticleSystemCollisionQuality.Low;
            var renderer = go.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = ParticleSystemRenderMode.Stretch;
            renderer.velocityScale = .06f; renderer.lengthScale = 1.5f;
            renderer.sharedMaterial = FxMaterials.Sprite("Percikan", "spark", true);
        }
    }
}
