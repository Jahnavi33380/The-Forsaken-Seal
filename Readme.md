````markdown
# The Forsaken Seal

**Course:** SER 594 / SER 494 – Final Group Project  
**Engine:** Unity 
---

## 1. Overview

**The Forsaken Seal** is a Unity-built game developed as our final group project.  
The project demonstrates:

- A main menu and playable main level
- Custom gameplay scripts (no heavy reliance on premade scripting assets)
- Environment design with textured, presentable areas suitable for in-class demonstration

---

## 2. How to Run the Game

### Option A – Play the Built Version (Recommended for Grading)

1. Open the folder:

   ```text
   FinalBuiltGame/
````
2. Inside, you’ll find the platform-specific build (e.g., `TheForsakenReal.exe` on Windows, or equivalent for your OS).
(FinalBuiltGame > The Forsaken Real.exe)
3. **Do not move** the executable away from its data folder.
4. Double-click the executable to launch the game and follow the on-screen prompts.

> This is the version we will use for the in-class presentation and is the easiest way to play the game.

---

## 3. Opening the Unity Project

If you want to inspect the full Unity project, assets, and code:

1. Open **Unity Hub**.

2. Click **Add project from disk**.

3. Select the `Game/` folder from this package:

   ```text
   Game/
     ├─ Assets/
     ├─ Packages/
     ├─ ProjectSettings/
     └─ (other Unity project files)
   ```

4. Open the project in Unity.

5. Look under `Assets/Scenes/` for the main menu and gameplay scenes.

---

## 4. Project Structure

```text
TheForsakenSeal/
├─ FinalBuiltGame/        # Playable build (double-click the executable here)
└─ Game/             # Full Unity project
   ├─ Assets/
   │  ├─ Scripts/    # Core gameplay scripts (player, interactions, UI, etc.)
   │  ├─ Scenes/     # Main menu + main level(s)
   │  ├─ Art/        # Models, textures, materials (may include asset packs)
   │  ├─ Audio/      # Music and SFX
   │  └─ ...         # Other asset subfolders used in the project
   ├─ Packages/
   ├─ ProjectSettings/
   └─ ...
```

> **Scripts:**
>
> * Primary gameplay code can be found under `Game/Assets/Scripts/`.
> * Additional scripts may also be nested within specific feature/scene folders inside `Assets/` (e.g., for UI, enemies, level-specific logic).

---

## 5. Controls (Default PC Layout)

> These are the default controls used during our presentation.

* **Move:** `W / A / S / D` or Arrow Keys
* **Camera Look:** Mouse
* **Jump / Action:** `Space`
* **Pause / Open Menu:** `Esc`

---

## 6. Notes & Known Limitations

* The build is configured and tested for classroom demonstration; performance may vary slightly on different machines.
* Some minor visual or polish issues may remain, but there are **no known A-grade-blocking bugs** (the game is fully playable from start to finish).

---

## 7. Team & Attribution

* **Project Title:** *The Forsaken Seal*
* **Team Members:** *Ashish Thanga, Bhawan Kumar Chinnavada, Jahnavi Gona, Harish Balamundi, Ritik Zambre*

Art assets, models, and audio may include a mix of:

* Original work created by the team
* Third-party asset packs (used for visuals only; scripting work is ours)

Any third-party assets are included solely for educational purposes as part of this course project.

```
```
