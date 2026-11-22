"""The key beside the plan: districts, street classes, notes and a scale bar."""
from labels import INK, font
from plan import SCALE, area_ratio
from ways import COLOURS

STREETS = [('lingkar', 'Jalan lingkar 18 m, 4 lajur + median (trailer)'), ('arteri', 'Jalan arteri 14 m, 4 lajur (tronton)'),
           ('kolektor', 'Jalan kolektor 9 m (engkel, pikap)'), ('rel', 'Rel kereta + perlintasan'),
           ('kali', 'Kali Sukabaja + jembatan')]
NOTES = ['Garis tipis di dalam blok = gang 2,6-4 m', '(jalan kaki, gerobak, motor bak).', '',
         'Untuk multiplayer: semua pemain berada di kota', 'yang sama. Kavling lapak dan kavling pabrik',
         'disediakan beberapa petak, satu per pemain;', 'pasar, pabrik, dan terminal dipakai bersama.', '',
         'Kotak merah = peta yang ada sekarang.']


def draw(draw_on, plan, left, top, places):
    y = top
    draw_on.text((left, y), 'DENAH SUKABAJA', fill=INK, font=font(30, True)); y += 44
    draw_on.text((left, y), 'usulan kedua, belum dibangun', fill=INK, font=font(15)); y += 40
    for name, colour in plan['areas'].values():
        draw_on.rectangle([left, y, left + 34, y + 22], fill=colour, outline=INK)
        draw_on.text((left + 46, y + 2), name, fill=INK, font=font(15)); y += 32
    y += 14
    for kind, text in STREETS:
        draw_on.rectangle([left, y + 5, left + 34, y + 5 + max(4, plan['widths'][kind] * .7)], fill=COLOURS[kind])
        draw_on.text((left + 46, y + 2), text, fill=INK, font=font(14)); y += 30
    y += 14
    for line in NOTES + ['%d tempat, luas kira-kira %.0fx peta sekarang.' % (places, area_ratio(plan))]:
        draw_on.text((left, y), line, fill=INK, font=font(14)); y += 22
    y += 16
    draw_on.line([left, y, left + 100 * SCALE, y], fill=INK, width=4)
    draw_on.text((left, y + 8), '100 meter', fill=INK, font=font(14))
