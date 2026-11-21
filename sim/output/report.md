# Laporan simulasi ekonomi Fase 0

Seed 73; 1.000 pemain awal, 90 hari nyata, +20 pendatang/hari.
Konfigurasi lengkap di config/. Angka berikut hasil model, bukan telemetri.

| Ukuran | PRD awal | Usulan |
|---|---:|---:|
| Tunggu bandar 15 ton (median hari) | 1.0000 | 1.0000 |
| Porsi AI maksimum H1–60 | 0.3519 | 0.0000 |
| Arbitrase NPC→AI (Rp/kg, pikap) | 400.0000 | -260.0000 |
| Laba harian median Kasta 1 (Rp) | 2,360,349.0740 | 33,689,191.9905 |
| Laba harian median Kasta 2 (Rp) | -16,424,831.8471 | 7,971,989.3474 |
| Laba harian median Kasta 3 (Rp) | N/A (belum tercapai) | N/A (belum tercapai) |
| Tonggak triseda (hari; hanya yang mencapai) | 1.0000 | 1.0000 |
| Tonggak pikap (hari; hanya yang mencapai) | 13.0000 | 5.0000 |
| Tonggak kasta2 (hari; hanya yang mencapai) | 28.0000 | 14.0000 |
| Tonggak cor_hanggar (hari; hanya yang mencapai) | N/A (belum tercapai) | 17.0000 |
| Tonggak kasta3 (hari; hanya yang mencapai) | N/A (belum tercapai) | N/A (belum tercapai) |
| Tonggak b2g (hari; hanya yang mencapai) | N/A (belum tercapai) | N/A (belum tercapai) |
| Inflasi moneter mingguan maksimum | 2.8781 | 7.4697 |

## baseline
Kriteria Lampiran I: `{"profit": false, "ai": false, "inflation": false, "progression": false}`.
Hari tanpa permintaan H1–60: 40; N/A tidak dihitung lolos AI.
Tonggak kohor aktif awal (tercapai/total; churn tersensor):
- triseda: 200/200, median 1.0000 hari.
- pikap: 21/200, median 13.0000 hari.
- kasta2: 12/200, median 28.0000 hari.
- cor_hanggar: 0/200, median N/A (belum tercapai) hari.
- kasta3: 0/200, median N/A (belum tercapai) hari.
- b2g: 0/200, median N/A (belum tercapai) hari.
Piramida ideal: 0.59% K3; target 20% memerlukan
97.62% peran pemasok tambahan jika produktivitas setara.
Pengali utang H1/H7/H30: {"1": 1.126162419264, "7": 2.297244466024894, "30": 35.32083135698926}.
EV barang gelap / laba legal: `{"3.0000x_heat0": 0.8571428571428571, "3.0000x_heat15": -0.75, "3.0000x_heat50": -4.5, "3.0000x_heat65": -6.1071428571428585, "4.0000x_heat0": 1.457142857142857, "4.0000x_heat15": -0.45, "4.0000x_heat50": -4.9, "4.0000x_heat65": -6.807142857142859, "5.0000x_heat0": 2.057142857142857, "5.0000x_heat15": -0.15, "5.0000x_heat50": -5.3, "5.0000x_heat65": -7.507142857142858}`.

## fixed
Kriteria Lampiran I: `{"profit": false, "ai": false, "inflation": false, "progression": false}`.
Hari tanpa permintaan H1–60: 13; N/A tidak dihitung lolos AI.
Tonggak kohor aktif awal (tercapai/total; churn tersensor):
- triseda: 200/200, median 1.0000 hari.
- pikap: 46/200, median 5.0000 hari.
- kasta2: 20/200, median 14.0000 hari.
- cor_hanggar: 16/200, median 17.0000 hari.
- kasta3: 0/200, median N/A (belum tercapai) hari.
- b2g: 0/200, median N/A (belum tercapai) hari.
Piramida ideal: 0.59% K3; target 20% memerlukan
97.62% peran pemasok tambahan jika produktivitas setara.
Pengali utang H1/H7/H30: {"1": 1.012060160240192, "7": 1.08753765844059, "30": 1.4328141955603715}.
EV barang gelap / laba legal: `{"3.5415x_heat0": 3.069585253456222, "3.5415x_heat15": 1.3, "3.5415x_heat50": -2.829032258064517, "3.5415x_heat65": -4.598617511520738}`.

## stress
Kriteria Lampiran I: `{"profit": false, "ai": false, "inflation": true, "progression": false}`.
Hari tanpa permintaan H1–60: 0; N/A tidak dihitung lolos AI.
Tonggak kohor aktif awal (tercapai/total; churn tersensor):
- triseda: 123/200, median 1.0000 hari.
- pikap: 17/200, median 9.0000 hari.
- kasta2: 44/200, median 1.0000 hari.
- cor_hanggar: 0/200, median N/A (belum tercapai) hari.
- kasta3: 35/200, median 1.0000 hari.
- b2g: 1/200, median 16.0000 hari.
Stress dimulai 60% K1 / 20% K2 / 20% K3 dengan modal usaha;
tonggak awal hasil fixture, bukan bukti progresi. Profit median K3:
62,671,683.4844 Rp/hari. AI maksimum: 63.63%.
Piramida ideal: 0.59% K3; target 20% memerlukan
97.62% peran pemasok tambahan jika produktivitas setara.
Pengali utang H1/H7/H30: {"1": 1.126162419264, "7": 2.297244466024894, "30": 35.32083135698926}.
EV barang gelap / laba legal: `{"3.0000x_heat0": 0.8571428571428571, "3.0000x_heat15": -0.75, "3.0000x_heat50": -4.5, "3.0000x_heat65": -6.1071428571428585, "4.0000x_heat0": 1.457142857142857, "4.0000x_heat15": -0.45, "4.0000x_heat50": -4.9, "4.0000x_heat65": -6.807142857142859, "5.0000x_heat0": 2.057142857142857, "5.0000x_heat15": -0.15, "5.0000x_heat50": -5.3, "5.0000x_heat65": -7.507142857142858}`.

## Batas interpretasi
Lihat README dan docs/ECONOMY_SPEC.md: inflasi moneter ≠ perubahan IBN;
rasio kebutuhan peran ≠ porsi tonase AI yang teramati. Kas pemain churn dibekukan
namun tetap dihitung dalam uang beredar. Median tonggak mengecualikan yang gagal;
tidak boleh diklaim median seluruh kohor. Naik kasta tidak dipicu timer target.
