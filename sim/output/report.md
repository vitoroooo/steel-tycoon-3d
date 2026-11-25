# Laporan simulasi ekonomi Fase 0

Seed 73; 1.000 pemain awal, 90 hari nyata, +20 pendatang/hari.
Target revisi Putaran 2; konfigurasi lengkap di config/. Bukan telemetri.

| Ukuran | PRD awal | Usulan |
|---|---:|---:|
| Tunggu bandar 15 ton (median hari) | 1.0000 | 1.0000 |
| Porsi AI maksimum H1–60 | 0.3519 | 0.0706 |
| Arbitrase NPC→AI (Rp/kg, pikap) | 400.0000 | -260.0000 |
| Laba harian median Kasta 1 (Rp) | 2,360,349.0740 | 691,082.0772 |
| Laba harian median Kasta 2 (Rp) | -16,424,831.8471 | 547,810.9601 |
| Laba harian median Kasta 3 (Rp) | N/A (belum tercapai) | 7,126,035.3015 |
| Tonggak triseda (hari; survivor awal di H90) | 1.0000 | 1.0000 |
| Tonggak pikap (hari; survivor awal di H90) | 13.0000 | 5.0000 |
| Tonggak kasta2 (hari; survivor awal di H90) | 28.0000 | 11.0000 |
| Tonggak cor_hanggar (hari; survivor awal di H90) | N/A (belum tercapai) | 12.0000 |
| Tonggak kasta3 (hari; survivor awal di H90) | N/A (belum tercapai) | 80.0000 |
| Tonggak b2g (hari; survivor awal di H90) | N/A (belum tercapai) | N/A (belum tercapai) |
| K2 santai (hari; survivor awal H90) | 64.0000 | 44.0000 |
| Porsi K2 H90 (rasio) | 0.2772 | 0.2120 |
| Porsi K3 H90 (rasio) | 0.0000 | 0.0435 |
| Inflasi moneter mingguan maksimum setelah H14 | 0.9092 | 0.0383 |

## baseline
Kriteria revisi: `{"profit": false, "ai": true, "inflation": false, "progression": false}`.
Hari tanpa input H1–60: 40; tanpa transaksi AI, rasio N/A.
Tonggak: kohor awal tipe aktif yang masih aktif H90; gagal = +∞, bukan dibuang.
- triseda: 11/11, median 1.0000 hari.
- pikap: 11/11, median 13.0000 hari.
- kasta2: 11/11, median 28.0000 hari.
- cor_hanggar: 0/11, median N/A (belum tercapai) hari.
- kasta3: 0/11, median N/A (belum tercapai) hari.
- b2g: 0/11, median N/A (belum tercapai) hari.
Santai K2: {'median': 64, 'reached': 11, 'cohort': 11}; kasta H90: {1: 0.7228260869565217, 2: 0.27717391304347827, 3: 0.0}.
Rasio lama: 0.59% K3; target 20% memerlukan
97.62% peran pemasok tambahan jika produktivitas setara.
Pengali utang H1/H7/H30: {"1": 1.126162419264, "7": 2.297244466024894, "30": 35.32083135698926}.
EV barang gelap / laba legal: `{"3.0000x_heat0": 0.8571428571428571, "3.0000x_heat15": -0.75, "3.0000x_heat50": -4.5, "3.0000x_heat65": -6.1071428571428585, "4.0000x_heat0": 1.457142857142857, "4.0000x_heat15": -0.45, "4.0000x_heat50": -4.9, "4.0000x_heat65": -6.807142857142859, "5.0000x_heat0": 2.057142857142857, "5.0000x_heat15": -0.15, "5.0000x_heat50": -5.3, "5.0000x_heat65": -7.507142857142858}`.

## fixed
Kriteria revisi: `{"profit": true, "ai": true, "inflation": true, "progression": true}`.
Hari tanpa input H1–60: 10; tanpa transaksi AI, rasio N/A.
Tonggak: kohor awal tipe aktif yang masih aktif H90; gagal = +∞, bukan dibuang.
- triseda: 11/11, median 1.0000 hari.
- pikap: 11/11, median 5.0000 hari.
- kasta2: 11/11, median 11.0000 hari.
- cor_hanggar: 11/11, median 12.0000 hari.
- kasta3: 8/11, median 80.0000 hari.
- b2g: 3/11, median N/A (belum tercapai) hari.
Santai K2: {'median': 44, 'reached': 11, 'cohort': 11}; kasta H90: {1: 0.7445652173913043, 2: 0.21195652173913043, 3: 0.043478260869565216}.
Rasio lama: 0.59% K3; target 5% memerlukan
88.69% peran pemasok tambahan jika produktivitas setara.
Pengali utang H1/H7/H30: {"1": 1.02, "7": 1.14868566764928, "30": 1.8113615841033548}.
EV barang gelap / laba legal: `{"3.5415x_heat0": 3.069585253456222, "3.5415x_heat15": 1.3, "3.5415x_heat50": -2.829032258064517, "3.5415x_heat65": -4.598617511520738}`.

## stress
Kriteria revisi: `{"profit": false, "ai": false, "inflation": true, "progression": false}`.
Hari tanpa input H1–60: 0; tanpa transaksi AI, rasio N/A.
Tonggak: kohor awal tipe aktif yang masih aktif H90; gagal = +∞, bukan dibuang.
- triseda: 7/11, median 1.0000 hari.
- pikap: 7/11, median 9.0000 hari.
- kasta2: 4/11, median N/A (belum tercapai) hari.
- cor_hanggar: 0/11, median N/A (belum tercapai) hari.
- kasta3: 1/11, median N/A (belum tercapai) hari.
- b2g: 1/11, median N/A (belum tercapai) hari.
Santai K2: {'median': 62, 'reached': 6, 'cohort': 11}; kasta H90: {1: 0.7445652173913043, 2: 0.20108695652173914, 3: 0.05434782608695652}.
Stress dimulai 60% K1 / 20% K2 / 20% K3 dengan modal usaha;
tonggak awal hasil fixture, bukan bukti progresi. Profit median K3:
62,671,683.4844 Rp/hari. AI maksimum: 63.63%.
Rasio lama: 0.59% K3; target 20% memerlukan
97.62% peran pemasok tambahan jika produktivitas setara.
Pengali utang H1/H7/H30: {"1": 1.126162419264, "7": 2.297244466024894, "30": 35.32083135698926}.
EV barang gelap / laba legal: `{"3.0000x_heat0": 0.8571428571428571, "3.0000x_heat15": -0.75, "3.0000x_heat50": -4.5, "3.0000x_heat65": -6.1071428571428585, "4.0000x_heat0": 1.457142857142857, "4.0000x_heat15": -0.45, "4.0000x_heat50": -4.9, "4.0000x_heat65": -6.807142857142859, "5.0000x_heat0": 2.057142857142857, "5.0000x_heat15": -0.15, "5.0000x_heat50": -5.3, "5.0000x_heat65": -7.507142857142858}`.

## Batas interpretasi
Lihat README dan docs/ECONOMY_SPEC.md: inflasi moneter ≠ perubahan IBN;
rasio kebutuhan peran ≠ porsi tonase AI yang teramati. Kas pemain churn dibekukan
namun tetap dihitung dalam uang beredar. Denominator tonggak adalah survivor H90;
bukan seluruh akun. Naik kasta tidak dipicu timer target atau kuota populasi.
AI diuji pada hari dengan input; hari tanpa input tidak punya ketergantungan AI.
Toleransi ~20%K2=15–25%, ~5%K3=3–7%; santai K2 H40–50. Lihat ECONOMY_SPEC.
