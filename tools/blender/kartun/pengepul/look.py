"""What one person wears. The body is shared; a Look only picks colours and what sits on the head and torso."""
from collections import namedtuple

# top and gear are functions returning parts; they live with the person who wears them.
Look = namedtuple('Look', 'shirt long_sleeves trousers long_legs hands cuff sandal strap ears top gear')
