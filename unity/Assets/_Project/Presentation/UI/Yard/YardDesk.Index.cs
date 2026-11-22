using UnityEngine;

namespace Steel.Presentation.UI
{
    internal sealed partial class YardDesk
    {
        private RectTransform index;
        private void BuildIndex()
        {
            UiFactory.Button(panel,"DAFTAR ISI",new Vector2(150,54),new Vector2(289,170),
                () => index.gameObject.SetActive(true));
            index = UiFactory.Panel("Daftar halaman",panel,Vector2.one*.5f,new Vector2(812,430),Vector2.zero);
            UiFactory.Text(index,"Buku usaha   /   Daftar isi",24,new Vector2(738,50),new Vector2(0,171));
            for (int i = 0; i < Pages.Length; i++)
            {
                int destination = i;
                UiFactory.Button(index,Pages[i],new Vector2(244,54),
                    new Vector2((i%3-1)*259,98-i/3*66),() =>
                    { page = destination; index.gameObject.SetActive(false); Turn(0); });
            }
            UiFactory.Button(index,"KEMBALI",new Vector2(180,54),new Vector2(-274,-165),
                () => index.gameObject.SetActive(false));
            index.gameObject.SetActive(false);
        }
    }
}
