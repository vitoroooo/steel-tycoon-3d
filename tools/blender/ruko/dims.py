"""Measurements of one ruko unit in metres. +Y is the street side, the origin is the footprint centre."""
WIDTH, DEPTH = 5.0, 9.0
HALF = WIDTH / 2
FRONT, BACK = DEPTH / 2, -DEPTH / 2
GROUND_FLOOR, UPPER_FLOOR = 3.5, 3.2    # storey heights
TERRACE = 1.1                           # the shopfront sits this far behind the upper floors
PLINTH = .18                            # floor level above the street
WALL = .15
PARAPET = .7
BALCONY = .9                            # how far the balcony reaches past the facade


def floor_z(level):
    """Height of the floor slab of a storey; level 0 is the shop."""
    return PLINTH if level == 0 else GROUND_FLOOR + (level - 1) * UPPER_FLOOR


def roof_z(floors):
    return GROUND_FLOOR + (floors - 1) * UPPER_FLOOR
