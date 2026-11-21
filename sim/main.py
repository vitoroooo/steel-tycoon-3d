"""Run PRD and proposed parameter sets with a common random seed."""
from pathlib import Path
from config import load
from model.simulation import run
from report.writer import write


def main():
    root = Path(__file__).resolve().parent
    configs = {name: load(name) for name in ('baseline', 'fixed', 'stress')}
    results = {name: run(config) for name, config in configs.items()}
    summaries, _ = write(results, configs, root / 'output')
    for name, summary in summaries.items():
        print(f'{name}: {summary["checks"]}')
        print(f'  milestones: {summary["milestones"]}')
        print(f'  day90 shares: {summary["mix"]}; casual K2: {summary["casual"]}')
    print('Generated sim/output/report.md and daily/weekly CSVs')


if __name__ == '__main__':
    main()
