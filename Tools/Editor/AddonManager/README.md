# MmoKitCE Addon Manager

<img width="64" height="64" alt="image" src="https://raw.githubusercontent.com/denariigames/mmokitce-addon-manager/refs/heads/master/images/LogoIcon.png" />

**Addon Manager** is a custom Unity Editor window designed for [**MmoKitCE**](https://github.com/denariigames/MmoKitCE) that provides a clean, professional interface for discovering, installing, and managing addons.

It functions as a private, curated "addon marketplace" directly inside the Unity Editor, making it easy to browse and install community or official addons without leaving your project.

## Features

<img width="260" height="475" alt="image" src="https://github.com/user-attachments/assets/94e477b5-09a7-4e3d-9af1-e3d2e8b07777" />

- **Dynamic Addon Discovery**
  - Addons are loaded from a central [manifest.json](https://github.com/denariigames/mmokitce-addon-manager/blob/master/manifest.json)
  - Shows updates available to installed addons
  - Clean, Unity Package Manager-inspired layout

- **Smart Filtering**
  - Category
  - Publisher (Core <img width="16" height="16" alt="image" src="https://raw.githubusercontent.com/denariigames/mmokitce-addon-manager/refs/heads/master/images/CoreBadge.png" /> or Community)
  - Status (All, Installed <img width="16" height="16" alt="image" src="https://raw.githubusercontent.com/denariigames/mmokitce-addon-manager/refs/heads/master/images/CheckIcon.png" />, Update Available <img width="16" height="16" alt="image" src="https://raw.githubusercontent.com/denariigames/mmokitce-addon-manager/refs/heads/master/images/UpdateIcon.png" />, Not Installed)
  - Recency (Anytime, This week <img width="16" height="16" alt="image" src="https://raw.githubusercontent.com/denariigames/mmokitce-addon-manager/refs/heads/master/images/NewBadge.png" />, This month)

- **Clean Project Structure**
  - Addons saved to MmoKitCE_addons folder, organized by category

- **Opt-In Analytics**
  - MmoKitCE Addon Manager collects completely anonymous usage statistics (addon downloads) to highlight the most popular addons in the community. No personal or project data is ever collected or transmitted.

- **Dependency Management (WIP)**
  - Initial support for indicating if addon requires core patch or has other addon dependencies.

## Contributing Addons (Easy Way)

**NEW**
[Addon Packager](https://github.com/denariigames/mmokitce-addons/tree/master/AddonPackager) is a custom Unity Editor tool designed for MmoKitCE community contributions. It simplifies the process of packaging your addons by:

- Exporting your addon folder as a .unitypackage with the required guid file
- Automatically generating a properly formatted package.json manifest

This makes it incredibly easy to share your addons with the MmoKitCE community. You can install Addon Packager directly from Addon Manager.

## Contributing Addons (Hard Way)

To prepare your addon:

- Bundle your assets as a .unitypackage with a guid file (*do not include the Assets folder!*)
  - The guid file should have a filename unique to the project. We recommend [GuidGenerator](https://guidgenerator.com/) to create one.
  -  `touch your-guid-id` at the command line so there is no extension (Windows equivalent: `echo.> your-guid-id`).
- Add a MmoKitCE compatible package.json with required fields (see below)
- Host on a public github repository
- Share your raw package.json URL on Discord
  - The raw URL links directly to the package.json content and should look like this: `https://raw.githubusercontent.com/<owner>/<repo>/refs/heads/main/package.json`

### MmoKitCE Compatible package.json

Each addon repository must have a MmoKitCE compatible package.json with these required fields:

- name (displayed in Addon Manager and used for addon folder name)
- guid (must be unique to the project and match guid filename in the unitypackage)
- author (format: string or object {"name": "yourname", "url": "yoururl"})
- version
- updateDate (format: "YYYY-MM-DD")
- packageUrl (link to the unitypackage, you can use either a release package or raw/refs/head)

Optional, but recommended:

- category (must match one of the approved categories below)
- description (supports \n for line breaks)
- screenshot (filename of screenshot file in repo at same root as package.json, e.g. screenshot.png)

Optional:

- patchFile (filename of patch file in repo at same root as package.json, e.g. myaddon.patch)
- dependencies (array of other addon guids, e.g. ["xxx", "yyy"])

A valid example package.json is [available here](https://github.com/denariigames/mmokitce-addon-manager/blob/master/example-package.json).

### Approved Categories

Only these categories appear in the filter dropdown:

- Demo
- Characters
- Monsters
- NPCs
- Combat
- Economy
- Items
- Gameplay
- UI
- MMO
- Integration
- Tools

Any other category value will be treated as "Uncategorized" and hidden from filters.
