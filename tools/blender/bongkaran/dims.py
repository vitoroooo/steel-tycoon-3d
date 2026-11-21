"""Measurements of the half-demolished building, in metres. X runs along the street front, Z is up."""
BAY_X, BAY_Y = 4.5, 5.0            # column spacing
BAYS_X, BAYS_Y = 3, 2
STOREY, FLOORS = 3.2, 3
COLUMN, BEAM_DEPTH, SLAB = .4, .5, .14
WIDTH, DEPTH = BAY_X * BAYS_X, BAY_Y * BAYS_Y


def column_x(i):
    return -WIDTH / 2 + i * BAY_X


def column_y(j):
    return -DEPTH / 2 + j * BAY_Y


def standing(i, level):
    """How much of the frame survives: the wrecking started at the east end and the top."""
    return i + level <= BAYS_X
