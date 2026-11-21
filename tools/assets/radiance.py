"""Reads Radiance .hdr (RGBE, run-length encoded) files into a float brightness image with numpy only."""
import numpy as np


def _scanline(data, at, width):
    """Decodes one new-style RLE scanline; returns the four channel rows and the next offset."""
    at += 4   # 2, 2, width high byte, width low byte
    row = np.empty((4, width), dtype=np.uint8)
    for channel in range(4):
        x = 0
        while x < width:
            count = data[at]
            if count > 128:                       # a run of one repeated value
                row[channel, x:x + count - 128] = data[at + 1]
                x, at = x + count - 128, at + 2
            else:                                 # a literal stretch
                row[channel, x:x + count] = np.frombuffer(data, np.uint8, count, at + 1)
                x, at = x + count, at + 1 + count
    return row, at


def brightness(path):
    """Sum of the linear RGB channels per pixel, top row first."""
    data = path.read_bytes()
    header_end = data.index(b'\n\n') + 2
    size_end = data.index(b'\n', header_end)
    _, height, _, width = data[header_end:size_end].split()   # "-Y height +X width"
    height, width, at = int(height), int(width), size_end + 1
    image = np.empty((height, width), dtype=np.float32)
    for y in range(height):
        row, at = _scanline(data, at, width)
        scale = np.ldexp(1.0, row[3].astype(np.int32) - 136)
        image[y] = row[:3].astype(np.float32).sum(axis=0) * scale
    return image
