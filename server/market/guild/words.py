"""The word filter of the chat (PRD 8.1)."""
import re

from ..clock import RULES


def clean(text):
    for word in RULES["bannedWords"]:
        text = re.sub(re.escape(word), "*" * len(word), text, flags=re.IGNORECASE)
    return text
