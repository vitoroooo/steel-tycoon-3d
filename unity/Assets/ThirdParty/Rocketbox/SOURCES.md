# Microsoft Rocketbox Avatar Library — MIT
Source: https://github.com/microsoft/Microsoft-Rocketbox (commit 0943055db6ec570bcef9f2c8b41c9e5467c808f9)
License text: LICENSE.md in this folder (MIT, Copyright (c) 2020 Microsoft).

| Folder | Used for | Upstream path |
|---|---|---|
| Male_Adult_09 | Player (pengepul) | Assets/Avatars/Adults/Male_Adult_09 |
| Animations | idle, walk, run | Assets/Animations/all_animations_max_motextr_* |

Changes: FBX bytes are unchanged. Textures were converted from TGA to JPG/PNG and the
normal and specular maps resized to 1024 px by tools/assets/prepare_rocketbox.py.
checksums.json records the SHA256 of every upstream file and of every converted file.
To add an avatar, run that script and add a line to CharacterCatalog.
