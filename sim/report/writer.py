"""Reproducible CSV measurements and generated Indonesian report."""
import csv
import json
from report.metrics import summarize
from report.diagnostics import diagnostics


def csv_write(path, rows):
    if not rows:
        return
    with path.open('w', newline='') as handle:
        writer = csv.DictWriter(handle, fieldnames=rows[0].keys(), lineterminator='\n')
        writer.writeheader()
        writer.writerows(rows)


def display(value):
    return 'N/A (belum tercapai)' if value is None else f'{value:,.4f}'


def write(results, configs, output):
    output.mkdir(parents=True, exist_ok=True)
    summaries = {name: summarize(r) for name, r in results.items()}
    evidence = {name: diagnostics(c) for name, c in configs.items()}
    lines = ['# Laporan simulasi ekonomi Fase 0', '',
             'Seed 73; 1.000 pemain awal, 90 hari nyata, +20 pendatang/hari.',
             'Konfigurasi lengkap di config/. Angka berikut hasil model, bukan telemetri.', '',
             '| Ukuran | PRD awal | Usulan |', '|---|---:|---:|']
    rows = {}
    for name, r in results.items():
        s, e = summaries[name], evidence[name]
        csv_write(output / f'{name}_daily.csv', r['daily'])
        csv_write(output / f'{name}_weekly.csv', s['weekly'])
        rows[name] = {'Tunggu bandar 15 ton (median hari)': s['wait'],
                      'Porsi AI maksimum H1–60': s['max_ai'],
                      'Arbitrase NPC→AI (Rp/kg, pikap)': e['arbitrage_per_kg']}
        for k, value in s['profit'].items():
            rows[name][f'Laba harian median Kasta {k} (Rp)'] = value
        for key, value in s['milestones'].items():
            rows[name][f'Tonggak {key} (hari; hanya yang mencapai)'] = value['median']
        rows[name]['Inflasi moneter mingguan maksimum'] = max(w['monetary'] for w in s['weekly'])
    for key in rows['baseline']:
        lines.append(f'| {key} | {display(rows["baseline"][key])} | {display(rows["fixed"][key])} |')
    for name, s in summaries.items():
        lines += ['', f'## {name}', f'Kriteria Lampiran I: `{json.dumps(s["checks"])}`.',
                  f'Hari tanpa permintaan H1–60: {s["no_demand"]}; N/A tidak dihitung lolos AI.',
                  'Tonggak kohor aktif awal (tercapai/total; churn tersensor):']
        lines += [f'- {key}: {v["reached"]}/{v["cohort"]}, median {display(v["median"])} hari.'
                  for key, v in s['milestones'].items()]
        if name == 'stress':
            lines += ['Stress dimulai 60% K1 / 20% K2 / 20% K3 dengan modal usaha;',
                      'tonggak awal hasil fixture, bukan bukti progresi. Profit median K3:',
                      f'{display(s["profit"][3])} Rp/hari. AI maksimum: {s["max_ai"]:.2%}.']
        e = evidence[name]
        lines += [f'Piramida ideal: {e["pyramid_k3_share"]:.2%} K3; target 20% memerlukan',
                  f'{e["target_shortage"]:.2%} peran pemasok tambahan jika produktivitas setara.',
                  f'Pengali utang H1/H7/H30: {json.dumps(e["debt_multipliers"])}.',
                  f'EV barang gelap / laba legal: `{json.dumps(e["contraband_ev_legal_ratio"])}`.']
    lines += ['', '## Batas interpretasi',
              'Lihat README dan docs/ECONOMY_SPEC.md: inflasi moneter ≠ perubahan IBN;',
              'rasio kebutuhan peran ≠ porsi tonase AI yang teramati. Kas pemain churn dibekukan',
              'namun tetap dihitung dalam uang beredar. Median tonggak mengecualikan yang gagal;',
              'tidak boleh diklaim median seluruh kohor. Naik kasta tidak dipicu timer target.']
    (output / 'report.md').write_text('\n'.join(lines) + '\n')
    (output / 'summary.json').write_text(json.dumps(dict(metrics=summaries, diagnostics=evidence)) + '\n')
    return summaries, evidence
