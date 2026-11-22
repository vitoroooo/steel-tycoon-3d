"""The key beside the plan: districts, street classes and a scale bar."""
from blocks import font, INK
from plan import SCALE
from ways import COLOURS

STREETS = [('lingkar', 'Jalan lingkar 18 m, 4 lajur + median (trailer)'), ('arteri', 'Jalan arteri 14 m, 4 lajur (tronton)'),
           ('kolektor', 'Jalan kolektor 9 m (engkel, pikap)'), ('rel', 'Rel kereta + perlintasan'),
           ('kali', 'Kali Sukabaja + jembatan')]
NOTES = ['Gang 2,6-4 m di dalam tiap blok', '(jalan kaki, gerobak, motor bak).', '',
         'Kotak merah = peta yang ada sekarang.', 'Luas baru kira-kira 19x peta sekarang.']


def draw(draw_on, plan, left, top):
    y = top
    draw_on.text((left, y), 'DENAH SUKABAJA', fill=INK, font=font(30, True)); y += 44
    draw_on.text((left, y), 'usulan, belum dibangun', fill=INK, font=font(15)); y += 40
    for name, colour in plan['areas'].values():
        draw_on.rectangle([left, y, left + 34, y + 22], fill=colour, outline=INK)
        draw_on.text((left + 46, y + 2), name, fill=INK, font=font(15)); y += 32
    y += 14
    for kind, text in STREETS:
        draw_on.rectangle([left, y + 5, left + 34, y + 5 + max(4, plan['widths'][kind] * .7)], fill=COLOURS[kind])
        draw_on.text((left + 46, y + 2), text, fill=INK, font=font(14)); y += 30
    y += 14
    for line in NOTES:
        draw_on.text((left, y), line, fill=INK, font=font(14)); y += 22
    y += 16
    draw_on.line([left, y, left + 100 * SCALE, y], fill=INK, width=4)
    draw_on.text((left, y + 8), '100 meter', fill=INK, font=font(14))
