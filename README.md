# Escape Room Co-op

A first-person, networked co-op escape room built in Unity. Up to 4 players join the same room over the internet using a shareable room code and work together to solve puzzles, race a shared countdown timer, and escape before time runs out.

No dedicated servers or manual IP/port forwarding — players connect through Unity Relay using a 6-character join code, so hosting a game is as simple as clicking "Host" and sharing the code.

## Gameplay

- **First-person co-op** — each player controls their own character with mouse-look and WASD movement, rendered with a humanoid model and synced walking animation.
- **Networked room code matchmaking** — the host generates a Relay allocation and join code; clients connect with that code, no port forwarding required.
- **Three puzzle archetypes:**
  - A single-player combination lock (find a clue, unlock a door).
  - A two-player synchronized pressure-plate puzzle — both players must stand on separate plates at the same time to open a barrier.
  - An information-asymmetry puzzle — one player reads a clue on one side of the room, the code has to be entered on a lock across the map, forcing players to communicate.
- **Shared, synced countdown timer** with win ("You escaped!") and lose ("Time's up!") end states.
- **Pause menu** (Esc) that freezes the game and timer for all connected players and shows the room code with a one-click copy button.

## Technical highlights

- **Unity Netcode for GameObjects (NGO)** for player synchronization, RPCs, and networked state (`NetworkVariable<T>` for puzzle/lock/game state).
- **Unity Relay via the unified Multiplayer Services package** — anonymous authentication, allocation creation/joining, and `RelayServerData` wiring into Unity Transport.
- **Owner-authoritative movement and animation.** NGO's built-in `NetworkTransform` and `NetworkAnimator` default to server authority; this project uses custom subclasses (`ClientNetworkTransform`, `ClientNetworkAnimator`) overriding `OnIsServerAuthoritative()` so each client's own movement/animation is authoritative and replicates correctly to everyone else.
- **Server-authoritative puzzle and game state** — interactions go through a shared `Interactable` base class and server RPCs, with `NetworkVariable<bool>`/`OnValueChanged` driving synced visuals (locks, plates, barriers) on every client.
- **Distance-based interaction detection** instead of physics triggers for the multi-player pressure-plate puzzle, polled server-side every frame for reliability across clients.
- **Clean session lifecycle** — returning to the main menu (voluntarily or via disconnect) tears down and destroys the persisted `NetworkManager` before reloading the scene, avoiding duplicate-singleton issues.

## Tech stack

- Unity 6000.5.x (URP)
- Unity Netcode for GameObjects
- Unity Transport
- Unity Relay / Multiplayer Services
- Unity Authentication (anonymous)
- TextMesh Pro

## Running it locally

1. Open the project in Unity 6000.5.x or later.
2. Open `Assets/Scenes/SampleScene.unity`.
3. Press Play, click **Host Olarak Başlat** (Host) — a room code is generated.
4. On another machine/build, click **Katıl** (Join) and enter the code to connect.

## Status

A solo/small-team learning project built from the ground up (including no prior Unity/networking experience going in) to explore real-time multiplayer game development with Unity Netcode for GameObjects and Unity Gaming Services.
