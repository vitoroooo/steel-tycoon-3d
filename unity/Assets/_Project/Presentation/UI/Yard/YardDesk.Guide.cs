using UnityEngine;

namespace Steel.Presentation.UI
{
    /// <summary>Beginner's guide page. It is only shown when the player asks for it.</summary>
    internal sealed partial class YardDesk
    {
        private const int GuidePage = 17;
        private int topic;

        private void BuildGuide() => UiFactory.Button(panel, "PANDUAN", new Vector2(140, 54), new Vector2(134, 170),
            () => { page = GuidePage; Turn(0); });

        private void DrawGuide()
        {
            int count = GuideBook.Topics.Length; game.ReadGuide();
            detail.text = GuideBook.Heading(topic) + "\n" + GuideBook.Page(topic, game);
            Key(0, "TOPIK SEBELUMNYA", () => topic = (topic + count - 1) % count);
            Key(2, "TOPIK BERIKUTNYA", () => topic = (topic + 1) % count);
        }
    }
}
