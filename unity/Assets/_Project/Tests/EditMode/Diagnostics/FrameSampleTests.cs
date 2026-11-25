using NUnit.Framework;
using Steel.Services;

namespace Steel.Tests
{
    public sealed class FrameSampleTests
    {
        [Test] public void MeasuresTailAndStopsWhenWindowEnds()
        {
            var s = new FrameSample();
            s.Add(double.NaN); s.Add(0); s.Add(-1); s.Add(double.PositiveInfinity);
            for (int i=0;i<90;i++) s.Add(.020);
            for (int i=0;i<9;i++) s.Add(.075);
            s.Add(28); Assert.IsTrue(s.Finished);
            s.Add(5);
            Assert.AreEqual(100,s.Frames); Assert.AreEqual(75,s.P95Ms());
            Assert.AreEqual(10,s.SlowFrames); Assert.AreEqual(28000,s.WorstMs);
            Assert.AreEqual(100/30.475,s.AverageFps,.00001);
        }
    }
}
