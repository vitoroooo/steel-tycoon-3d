"""Train on a bounded grid, freeze the winner, then measure it and control on unseen seeds."""
import json
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT))
from config import load
from tools.mobile_grid import candidates, rank
from tools.sweep import evaluate


def measure(config, name, seeds, split):
    return [dict(split=split, policy=name, seed=s, **evaluate(dict(config, seed=s))) for s in seeds]


def counts(rows):
    result = {k: sum(r['checks'][k] for r in rows) for k in rows[0]['checks']}
    return dict(result, joint=sum(all(r['checks'].values()) for r in rows))


def experiment(plan):
    train, holdout = plan['train'], plan['holdout']
    if not train or not holdout or len(set(train + holdout)) != len(train + holdout):
        raise ValueError('train and holdout must be non-empty, unique and disjoint')
    base = load('mobile')
    history, training = [], []
    best = None
    for name, knobs, config in candidates(base, plan):
        rows = measure(config, name, train, 'train')
        score = rank(rows, base['targets'])
        training.append(dict(policy=name, knobs=knobs, passes=counts(rows), score=score))
        history.extend(rows)
        if best is None or score > best[0]:
            best = score, name, config
    _, winner, config = best
    validation = {}
    for name, candidate in {'control': base, winner: config}.items():
        rows = measure(candidate, name, holdout, 'holdout')
        history.extend(rows)
        validation[name] = counts(rows)
    return history, dict(winner=winner, targets=base['targets'], train=training, holdout=validation)


def main():
    plan = json.loads((ROOT / 'config/mobile_calibration.json').read_text())
    rows, summary = experiment(plan)
    path = ROOT / 'output'
    path.mkdir(exist_ok=True)
    (path / 'mobile_trials.jsonl').write_text(''.join(json.dumps(r, allow_nan=False) + '\n' for r in rows))
    (path / 'mobile_calibration.json').write_text(json.dumps(summary, indent=2, allow_nan=False) + '\n')
    print(summary['winner'], summary['holdout'], flush=True)


if __name__ == '__main__':
    main()
