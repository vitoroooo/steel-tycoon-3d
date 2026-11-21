"""Measurements of the tracked excavator, in metres. +Y is forward, Z is up, origin under the turntable."""
TRACK_LENGTH, TRACK_WIDTH, TRACK_HEIGHT, TRACK_GAUGE = 3.7, .5, .78, 1.05   # gauge: centre line to each track
DECK = 1.0                       # top of the turntable, where the house sits
HOUSE_WIDTH, HOUSE_LENGTH, HOUSE_HEIGHT = 2.5, 3.1, 1.0
BOOM_FOOT = (.3, .95, DECK + .5)                 # the pin the whole arm swings on
ELBOW, BOOM_TIP = (.3, 3.5, 4.3), (.3, 5.5, 3.7)
STICK_END = (.3, 6.25, 1.75)
CHISEL_TIP = (.3, 6.55, .32)
