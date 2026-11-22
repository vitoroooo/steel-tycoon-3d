"""Turns the few points a street is written with into the smooth line it is drawn and built as."""

STEPS = 14   # pieces between two written points


def _between(p0, p1, p2, p3, t):
    """Catmull-Rom: the curve passes through p1 and p2, and leaves each in the direction of its neighbours."""
    t2, t3 = t * t, t * t * t
    return tuple(.5 * (2 * b + (c - a) * t + (2 * a - 5 * b + 4 * c - d) * t2 + (3 * b - a - 3 * c + d) * t3)
                 for a, b, c, d in zip(p0, p1, p2, p3))


def smooth(points, closed=False):
    """A dense polyline through every written point; closed joins the last point back to the first."""
    points = [tuple(p) for p in points]
    count = len(points)
    if count < 3:
        return points

    def at(i):
        return points[i % count] if closed else points[min(max(i, 0), count - 1)]

    line = []
    for i in range(count if closed else count - 1):
        for step in range(STEPS):
            line.append(_between(at(i - 1), at(i), at(i + 1), at(i + 2), step / STEPS))
    line.append(at(0) if closed else points[-1])
    return line


def length(line):
    return sum(((a[0] - b[0]) ** 2 + (a[1] - b[1]) ** 2) ** .5 for a, b in zip(line, line[1:]))
