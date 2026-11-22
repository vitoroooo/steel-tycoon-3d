"""Who lives in Sukabaja. To add a citizen: one Look here, then export `warga:<name>`."""
from ..pengepul.look import Look
from . import tops, wear

LOOKS = {
    # Out shopping at Pasar Lama.
    'ibu': Look(shirt='yellow', long_sleeves=True, trousers='navy', long_legs=True, hands='skin', cuff='cream',
                sandal='orange', strap='cream', ears=False, top=tops.hijab, gear=wear.shopping_bag),
    # The neighbour who is always on his way to the musala.
    'tetangga': Look(shirt='cream', long_sleeves=False, trousers='teal', long_legs=True, hands='skin', cuff=None,
                     sandal='wood', strap='navy', ears=True, top=tops.peci, gear=wear.chest_pocket),
    # Hangs around the market lane in a football shirt.
    'pemuda': Look(shirt='red', long_sleeves=False, trousers='navy', long_legs=False, hands='skin', cuff=None,
                   sandal='light', strap='red', ears=True, top=tops.cropped, gear=wear.nothing),
    # A smelter hand: hard hat, vest, gloves, closed boots.
    'pekerja': Look(shirt='navy', long_sleeves=True, trousers='grey', long_legs=True, hands='light', cuff='yellow',
                    sandal='ink', strap=None, ears=True, top=tops.helmet, gear=wear.vest),
}
