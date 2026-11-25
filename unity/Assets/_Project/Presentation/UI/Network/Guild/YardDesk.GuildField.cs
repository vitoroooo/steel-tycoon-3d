using System;
using UnityEngine;
using UnityEngine.UI;

namespace Steel.Presentation.UI
{
    internal sealed partial class YardDesk
    {
        /// <summary>Asks for one line of text (an association name, a chat line) and hands it to <paramref name="send"/>.</summary>
        private void GuildField(string prompt, int limit, Action<string> send)
        {
            var modal = UiFactory.Panel("Tulis",panel,Vector2.one * .5f,panel.sizeDelta,Vector2.zero);
            UiFactory.Text(modal,prompt,24,new Vector2(660,50),new Vector2(0,135));
            UiFactory.Text(modal,"Maksimal " + limit + " karakter. Kata kasar disensor dan pesan dapat dilaporkan.",19,
                new Vector2(660,48),new Vector2(0,67));
            var field = UiFactory.Panel("Isi",modal,Vector2.one * .5f,new Vector2(660,64),Vector2.zero);
            field.GetComponent<Image>().color = new Color(.15f,.19f,.22f);
            var input = field.gameObject.AddComponent<InputField>();
            input.textComponent = UiFactory.Text(field,"",21,new Vector2(620,58),Vector2.zero);
            input.characterLimit = limit;
            UiFactory.Button(modal,"Batal",new Vector2(210,62),new Vector2(-125,-117),
                () => UnityEngine.Object.Destroy(modal.gameObject));
            UiFactory.Button(modal,"Kirim",new Vector2(210,62),new Vector2(125,-117),() =>
            {
                string text = input.text.Trim();
                UnityEngine.Object.Destroy(modal.gameObject);
                if (text.Length > 0) send(text);
                Draw();
            });
            input.ActivateInputField();
        }
    }
}
