using UnityEngine;

namespace Steel.Presentation.UI
{
    internal sealed partial class YardDesk
    {
        private RectTransform index;
        private RectTransform[] sections;
        private int indexPage;
        private void BuildIndex()
        {
            UiFactory.Button(panel,"DAFTAR ISI",new Vector2(150,54),new Vector2(289,170),
                () => index.gameObject.SetActive(true));
            index = UiFactory.Panel("Daftar halaman",panel,Vector2.one*.5f,new Vector2(812,430),Vector2.zero);
            UiFactory.Text(index,"Buku usaha   /   Daftar isi",24,new Vector2(738,50),new Vector2(0,171));
            sections = new RectTransform[Pages.Length];
            for (int i = 0; i < Pages.Length; i++)
            {
                int destination = i;
                sections[i] = UiFactory.Button(index,Pages[i],new Vector2(244,54),
                    new Vector2((i%3-1)*259,98-(i%12)/3*66),() =>
                    { page = destination; index.gameObject.SetActive(false); Turn(0); }).GetComponent<RectTransform>();
            }
            UiFactory.Button(index,"KEMBALI",new Vector2(180,54),new Vector2(-274,-165),
                () => index.gameObject.SetActive(false));
            if (sections.Length > 12)
                UiFactory.Button(index,"HALAMAN LAIN",new Vector2(180,54),new Vector2(274,-165),
                    () => ShowIndex((indexPage+1)%((sections.Length+11)/12)));
            ShowIndex(0);
            index.gameObject.SetActive(false);
        }
        private void ShowIndex(int n)
        {
            indexPage = n;
            for (int i = 0; i < sections.Length; i++) sections[i].gameObject.SetActive(i/12 == n);
        }
    }
}
