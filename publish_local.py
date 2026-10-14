from __future__ import annotations

import argparse
import shutil
import subprocess
import sys
from pathlib import Path


MOD_NAME = "UnsmoothSurface"
DEFAULT_STEAM_MODS_DIR = Path(
    r"C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods"
)
PROJECT_FILE = Path("Source") / "LambdaTenEleven.UnsmoothSurface" / "LambdaTenEleven.UnsmoothSurface.csproj"
SUPPORTED_VERSIONS = ("1.6", "1.7")
PUBLISH_DIRS = ("About", *SUPPORTED_VERSIONS, "Defs", "Languages", "Patches", "Textures")
PUBLISH_FILES = ("LoadFolders.xml",)
ASSEMBLY_NAME = "LambdaTenEleven.UnsmoothSurface.dll"
EXCLUDED_FILE_SUFFIXES = {".pdb"}


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(
        description="Build and publish this RimWorld mod to the local Steam install."
    )
    parser.add_argument(
        "--steam-mods-dir",
        type=Path,
        default=DEFAULT_STEAM_MODS_DIR,
        help=f"RimWorld Mods directory. Default: {DEFAULT_STEAM_MODS_DIR}",
    )
    parser.add_argument(
        "--configuration",
        default="Release",
        help="dotnet build configuration. Default: Release",
    )
    return parser.parse_args()


def copy_runtime_tree(source: Path, destination: Path) -> None:
    def ignore_dev_files(_directory: str, names: list[str]) -> set[str]:
        return {
            name
            for name in names
            if Path(name).suffix.lower() in EXCLUDED_FILE_SUFFIXES
        }

    shutil.copytree(source, destination, ignore=ignore_dev_files)


def main() -> int:
    args = parse_args()
    repo_root = Path(__file__).resolve().parent
    project_file = repo_root / PROJECT_FILE
    steam_mods_dir = args.steam_mods_dir.resolve()
    destination = steam_mods_dir / MOD_NAME

    if not project_file.is_file():
        print(f"Project file was not found: {project_file}", file=sys.stderr)
        return 1

    if not steam_mods_dir.is_dir():
        print(f"RimWorld Mods directory was not found: {steam_mods_dir}", file=sys.stderr)
        return 1

    print(f"Building {project_file} ({args.configuration})...")
    subprocess.run(
        ["dotnet", "build", str(project_file), "--configuration", args.configuration],
        cwd=repo_root,
        check=True,
    )

    for directory_name in PUBLISH_DIRS:
        source = repo_root / directory_name
        if not source.is_dir():
            print(f"Required mod directory was not found: {source}", file=sys.stderr)
            return 1

    required_files = [repo_root / name for name in PUBLISH_FILES]
    required_files.extend(
        repo_root / version / "Assemblies" / ASSEMBLY_NAME
        for version in SUPPORTED_VERSIONS
    )
    for source in required_files:
        if not source.is_file():
            print(f"Required mod file was not found: {source}", file=sys.stderr)
            return 1

    if destination.exists():
        if destination.resolve().parent != steam_mods_dir:
            print(f"Refusing to remove unexpected path: {destination}", file=sys.stderr)
            return 1

        print(f"Removing existing local mod: {destination}")
        shutil.rmtree(destination)

    print(f"Publishing to: {destination}")
    destination.mkdir()

    for directory_name in PUBLISH_DIRS:
        source = repo_root / directory_name
        copy_runtime_tree(source, destination / directory_name)

    for file_name in PUBLISH_FILES:
        shutil.copy2(repo_root / file_name, destination / file_name)

    print("Done.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
