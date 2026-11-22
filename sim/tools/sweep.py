"""Parameter experiments; writes measurements, never changes acceptance bounds."""
import json
import sys
from copy import deepcopy
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parents[1]))
from config import load, merge
from model.simulation import run
from report.metrics import summarize


def evaluate(config):
    s = summarize(run(config))
    weekly = [w['monetary'] for w in s['weekly'] if w['day'] > config['targets']['inflation_after']]
    return dict(checks=s['checks'], mix=s['mix'], profit=s['profit'], ai=s['max_ai'],
                inflation=max(weekly), k2=s['milestones']['kasta2'], casual=s['casual'])


def main():
    base = load('fixed')
    root = Path(__file__).resolve().parents[1]
    grid = json.loads((root / 'config/calibration.json').read_text())
    rows = []
    for ambition in grid['ambition']:
        for preference in grid['bandar_preference']:
            c = deepcopy(base)
            merge(c, dict(role_goals=dict(aktif=[0, 1 - ambition, ambition],
                                         sedang=[1 - preference, preference, 0])))
            rows.append(dict(ambition=ambition, preference=preference, **evaluate(c)))
    for seed in grid['holdout_seeds']:
        c = deepcopy(base)
        c['seed'] = seed
        rows.append(dict(holdout_seed=seed, **evaluate(c)))
    (root / 'output/sweep.jsonl').write_text(''.join(json.dumps(r) + '\n' for r in rows))
    for row in rows:
        print(json.dumps(row))


if __name__ == '__main__':
    main()
