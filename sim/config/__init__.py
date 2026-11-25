"""Scenario configuration, with recursive overrides."""
import json
from pathlib import Path

ROOT = Path(__file__).resolve().parent


def merge(base, overrides):
    for key, value in overrides.items():
        if isinstance(value, dict):
            merge(base[key], value)
        else:
            base[key] = value
    return base


def load(name="baseline"):
    config = {}
    for path in sorted(ROOT.glob('baseline_*.json')):
        config.update(json.loads(path.read_text()))
    if name != "baseline":
        merge(config, json.loads((ROOT / f"{name}.json").read_text()))
    return config
