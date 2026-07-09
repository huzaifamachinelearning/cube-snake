
# Cube Snake

A 3D arcade game developed in **Unity** using **C#**, where players control a numbered cube and strategically grow stronger by absorbing and collecting other cubes. The game combines merge mechanics inspired by powers of two with snake-like growth gameplay.

## Gameplay

- The player starts as a cube with a numerical value.
- When the player collides with a cube of the **same value**, the cubes merge:
  - The player's value doubles.
  - The player's color changes to reflect the new level.
- When the player collides with a cube of a **lower value**, that cube joins the player's chain, forming a snake-like trail.
- The chain grows longer as more lower-valued cubes are collected.
- Colliding with a cube of a **higher value** results in a game over.
- Hitting obstacles or level boundaries also ends the game.

## Features

- Power-of-two progression system.
- Dynamic cube merging mechanics.
- Snake-style follower chain.
- Color changes based on cube value.
- Physics-based collision detection.
- Obstacle and boundary game-over system.
- Smooth real-time gameplay built with Unity.

## Technologies Used

- **Unity**
- **C#**
- Unity Physics & Collision System
- Unity UI

## Controls

| Action | Control |
|----------|----------|
| Move Left | A / Left Arrow |
| Move Right | D / Right Arrow |
| Forward Movement | Automatic |

*(Update controls if your implementation differs.)*

## Game Rules

1. Merge with cubes of equal value to increase your strength.
2. Collect lower-valued cubes to grow your snake chain.
3. Avoid cubes with higher values.
4. Avoid obstacles and map boundaries.
5. Survive as long as possible and achieve the highest cube value.

## Screenshots

Add screenshots of gameplay here.

```text
screenshots/gameplay1.png
screenshots/gameplay2.png
```

## Future Improvements

- Multiple levels and environments.
- Power-ups and special abilities.
- High-score leaderboard.
- Sound effects and background music.
- Mobile platform support.
- Additional cube skins and visual effects.

## Installation

1. Clone the repository:

```bash
git clone https://github.com/huzaifamachinelearning/cube-snake.git
```

2. Open the project in Unity.
3. Load the main scene.
4. Press Play to run the game.

