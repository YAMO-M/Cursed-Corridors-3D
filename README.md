# Cursed Corridors 3D

COMP307 Graphics and Modelling group project. A third-person survival maze game built in Unity (Universal Render Pipeline).

## Group Members

- Kgaugelo Mphahlele (224135430)
- Laborona Lecheko (223004822)
- Bayanda Mtshali (224028765)
- Ammar Mohamed Elsaid (222128715)

## About the Game

You are trapped in a dark stone maze patrolled by zombies. Find your way to the exit before the timer runs out, and survive the zombies chasing you.

## Requirements

- Unity **6000.5.10f1** (use this exact version; opening in another version rewrites project files)
- Windows Build Support (IL2CPP) module if you want to build
- Git and **Git LFS** (models, textures and audio are stored with LFS)

## How to Run (Unity Editor)

1. Install Git LFS once: `git lfs install`
2. Clone the repo: `git clone https://github.com/YAMO-M/Cursed-Corridors-3D.git`
3. If assets look broken or empty, run `git lfs pull` in the project folder.
4. Open Unity Hub, click **Add > Add project from disk**, and select the cloned folder.
5. Open with Unity 6000.5.9f1. The first import takes several minutes.
6. Open the scene `Assets/Scenes/MainMenu.unity` and press **Play**.
   The game flow is MainMenu -> SampleScene (the maze).

## How to Run (Built .exe)

1. Download and unzip the build folder from the OneDrive link provided in the submission.
2. Keep all files in the folder together and run the `.exe`.

## Controls

| Action | Key |
|---|---|
| Move | W A S D |
| Look (left/right) | Mouse |
| Run | Shift |
| Interact / collect | M |

## Win and Lose Conditions

**Lose**
- **Caught by zombies:** zombies chase you when you are in range and drain health on each attack. At 0 health the lose screen shows "Caught".
- **Time ran out:** a 5-minute countdown is shown on screen. When it reaches 00:00 the lose screen shows "Time's Up".

**Win**
- Reach the exit to reach the win screen.

After a win or loss, use the on-screen button to restart.

## Project Structure

- `Assets/Scenes/` - `MainMenu` and `SampleScene` (the maze level)
- `Assets/_Project/Scripts/` - game scripts (GameManager, EndScreenUI, MainMenu, TimerUI, PlayerHealth, ZombieChaseAI, camera and interaction scripts)
- `Assets/_Project/Prefabs/` - maze kit, player, zombie and other prefabs

## Scripts Overview

- **GameManager** - tracks game state, countdown timer, collectibles and fires win/lose events.
- **EndScreenUI** - listens to GameManager and shows the win or lose panel.
- **TimerUI** - displays the countdown on the HUD.
- **PlayerHealth** - health and health bar; calls the lose condition at 0.
- **ZombieChaseAI** - NavMesh-based detection, chase and attack behaviour.
- **MainMenu** - start and quit buttons.


## Notes

- The maze geometry (walls, floor, corners, exit) was built by hand from primitives on a 4m grid.
- Pickups use an `IInteractable` interface so any interactable object works with the same interaction code.
