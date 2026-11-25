using UnityEngine;
using UnityEngine.UI;

namespace Steel.Presentation.UI
{
    internal sealed partial class YardDesk
    {
        private void NetworkAddress() => NetworkField(false);
        private void NetworkLogin() => NetworkField(true);

        private void NetworkField(bool login)
        {
            var modal = UiFactory.Panel("Masuk pasar", panel, Vector2.one * .5f, panel.sizeDelta, Vector2.zero);
            UiFactory.Text(modal,login ? "Kirim token ke " + network.Host : "Alamat pasar",24,
                new Vector2(660,50),new Vector2(0,135));
            UiFactory.Text(modal,login ? "Gunakan hanya token yang diterbitkan pengelola alamat ini." :
                "Masukkan alamat https:// dari pengelola yang Anda kenal.",19,new Vector2(660,48),
                new Vector2(0,67));
            var field = UiFactory.Panel("Token pedagang",modal,Vector2.one * .5f,new Vector2(660,64),Vector2.zero);
            field.GetComponent<Image>().color = new Color(.15f,.19f,.22f);
            var inputField = field.gameObject.AddComponent<InputField>();
            inputField.textComponent = UiFactory.Text(field,"",21,new Vector2(620,58),Vector2.zero);
            inputField.contentType = login ? InputField.ContentType.Password : InputField.ContentType.Standard;
            inputField.characterLimit = login ? 43 : 253;
            inputField.shouldHideMobileInput = login;
            UiFactory.Button(modal,"Batal",new Vector2(210,62),new Vector2(-125,-117),
                () => Object.Destroy(modal.gameObject));
            UiFactory.Button(modal,login ? "Hubungkan" : "Pilih alamat",new Vector2(210,62),new Vector2(125,-117),() =>
            {
                string access = inputField.text.Trim();
                inputField.text = "";
                Object.Destroy(modal.gameObject);
                NetworkOpen(NetworkTab.Account);
                if (login) NetworkRun(() => network.Connect(access));
                else
                {
                    try { network = new Steel.Services.NetworkMarket(access); }
                    catch (System.ArgumentException e) { networkMessage = e.Message; }
                }
                Draw();
            });
            inputField.ActivateInputField();
        }
    }
}
