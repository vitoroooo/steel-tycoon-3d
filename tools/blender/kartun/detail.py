"""One dial for mesh density: 1 for design renders, about .5 for the phone build."""
LEVEL = 1.0


def count(full, least=6):
    """An even segment count scaled by the dial, never below what keeps the form round."""
    return max(least, int(round(full * LEVEL / 2)) * 2)


def steps(full, least=1):
    """Bevel and curve subdivisions scaled by the dial."""
    return max(least, int(round(full * LEVEL)))
