"""Copy a Microsoft Rocketbox avatar (MIT) into the Unity project.

Usage: prepare_rocketbox.py <rocketbox checkout> <category/avatar> [clip ...]
Textures are converted from 12 MB TGA to JPG/PNG so the repository stays small.
"""
import hashlib
import json
import shutil
import sys
from pathlib import Path

from PIL import Image

TARGET = Path(__file__).resolve().parents[2] / "unity/Assets/ThirdParty/Rocketbox"
REPO_URL = "https://github.com/microsoft/Microsoft-Rocketbox"


def sha256(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def convert(source: Path, folder: Path) -> Path:
    image = Image.open(source)
    kind = source.stem.split("_")[-1]
    if image.mode == "RGBA":
        target = folder / (source.stem + ".png")
        image.save(target, optimize=True)
    elif kind == "normal":
        target = folder / (source.stem + ".png")
        image.resize((1024, 1024), Image.LANCZOS).save(target, optimize=True)
    else:
        size = 2048 if kind == "color" else 1024
        target = folder / (source.stem + ".jpg")
        image.convert("RGB").resize((size, size), Image.LANCZOS).save(target, quality=90)
    return target


def main() -> None:
    checkout, avatar, clips = Path(sys.argv[1]), sys.argv[2], sys.argv[3:]
    name = avatar.split("/")[-1]
    source = checkout / "Assets/Avatars" / avatar
    folder = TARGET / name
    (folder / "Textures").mkdir(parents=True, exist_ok=True)
    records = {}
    model = source / "Export" / (name + ".fbx")
    shutil.copy(model, folder / model.name)
    records[f"{name}/{model.name}"] = {"source": f"Assets/Avatars/{avatar}/Export/{model.name}",
                                       "sha256": sha256(model)}
    for texture in sorted((source / "Textures").glob("*.tga")):
        out = convert(texture, folder / "Textures")
        records[f"{name}/Textures/{out.name}"] = {
            "source": f"Assets/Avatars/{avatar}/Textures/{texture.name}",
            "source_sha256": sha256(texture), "sha256": sha256(out)}
    (TARGET / "Animations").mkdir(exist_ok=True)
    for clip in clips:
        path = checkout / "Assets/Animations" / clip
        out = TARGET / "Animations" / (path.name.replace(".max.fbx", ".fbx"))
        shutil.copy(path, out)
        records[f"Animations/{out.name}"] = {"source": f"Assets/Animations/{clip}", "sha256": sha256(path)}
    manifest = TARGET / "checksums.json"
    data = json.loads(manifest.read_text()) if manifest.exists() else {"repository": REPO_URL, "files": {}}
    data["files"].update(records)
    manifest.write_text(json.dumps(data, indent=2, sort_keys=True) + "\n")
    print(f"{name}: {len(records)} files")


if __name__ == "__main__":
    main()
