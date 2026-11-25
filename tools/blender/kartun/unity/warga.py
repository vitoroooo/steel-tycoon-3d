"""The citizens as jointed parts: the pengepul's joints in another Look. Export with `warga:<name>`."""
from ..warga.looks import LOOKS
from . import pengepul


def target(name):
    return 'Characters', 'Warga_' + name


def nodes(name):
    return pengepul.nodes(LOOKS[name])
