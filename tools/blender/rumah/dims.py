"""Measurements of one kampung house in metres. +Y is the lane side, the origin is the footprint centre."""
import math

WIDTH = 6.0
HALF = WIDTH / 2
FRONT, BACK = 2.2, -3.4          # the masonry body
PORCH = 1.6                      # tiled terrace in front of the body
KITCHEN = 1.8                    # lean-to behind the body
PLINTH = .25
EAVE = PLINTH + 2.8              # top of the walls
PITCH = math.radians(30)
OVERHANG = .5
MID = (FRONT + BACK) / 2         # the ridge runs along X above this line
SPAN = (FRONT - BACK) / 2
RIDGE = EAVE + SPAN * math.tan(PITCH)
WALL = .15
