"""Fixed-seed acceptance measurements for the candidate and small policy tweaks."""
import json
import sys
from copy import deepcopy
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parents[1]))
from config import load, merge
from tools.sweep import evaluate


def main():
    root = Path(__file__).resolve().parents[1]
    grid = json.loads((root / 'config/calibration.json').read_text())
    rows = []
    for name, patch in grid['tweaks'].items():
        for seed in grid['robustness_seeds']:
            c = merge(deepcopy(load('fixed')), patch)
            c['seed'] = seed
            row = dict(tweak=name, seed=seed, **evaluate(c))
            rows.append(row)
        measurements = [r for r in rows if r['tweak'] == name]
        rates = {key: sum(r['checks'][key] for r in measurements)
                 for key in measurements[0]['checks']}
        print(name, rates, 'all:', sum(all(r['checks'].values()) for r in measurements),
              flush=True)
    (root / 'output/robustness.jsonl').write_text(
        ''.join(json.dumps(r) + '\n' for r in rows))


if __name__ == '__main__':
    main()
