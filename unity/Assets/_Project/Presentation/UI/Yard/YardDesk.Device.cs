using UnityEngine;

namespace Steel.Presentation.UI
{
    internal sealed partial class YardDesk
    {
        private DeviceTrial trial;
        private void DrawDevice()
        {
            if (trial.Active)
            {
                detail.text = "Pengukuran berjalan. Tutup buku, lalu jalan sambil mengusap kamera.";
                Key(0,"Lanjut bermain",Close);
                return;
            }
            detail.text = trial.Report.Length > 0 ? trial.Report :
                "Ukur FPS dan jeda frame di HP ini selama 30 detik.\n" +
                "Berjalan di pasar sambil memutar kamera, lalu buka halaman ini lagi.\n" +
                "Hasil hanya di layar ini; tidak diunggah.";
            Key(0,"Mulai uji 30 detik",() => { Close(); trial.Begin(); });
            if (trial.Report.Length > 0) Key(1,"Salin hasil",() =>
            {
                GUIUtility.systemCopyBuffer = trial.Report; feedback.text = "Hasil disalin.";
            });
        }
    }
}
