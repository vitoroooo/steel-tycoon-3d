"""Assembles the 50 kg push cart."""
from . import box, load


def build():
    return box.walls() + box.running_gear() + load.sacks() + load.finds() + load.scale() + load.sign()
