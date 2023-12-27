# Unity Utils Repository

This repository contains a collection of utility scripts and tools for Unity game development, aimed at simplifying common tasks and enhancing workflow efficiency.

## Table of Contents

- [Scripts](#scripts)
  - [Audio](#audio)
  - [Input](#input)
  - [Pooling](#pooling)
  - [Procedural Generation](#procedural-generation)
  - [Singleton](#singleton)
  - [StateMachine](#statemachine)
  - [Tilemaps](#tilemaps)

## Scripts

### Audio

- **AudioData.cs**: Defines a data structure for handling audio-related information.
- **AudioDatabase.cs**: Manages a database of audio assets.
- **Audios/SFX_FOOTSTEP.asset**: Example audio asset for footsteps.

### Input

- **InputManager.cs**: Handles input management for various actions.
- **MainActions.cs**: Defines main actions for input.
- **MainActions.inputactions**: Input actions configuration file.

### Pooling

- **ExamplePooling.cs**: Demonstrates the usage of object pooling.
- **PoolerBase.cs**: Base class for object pooling implementation.

### Procedural Generation

- **MapGenerator.cs**: Generates maps procedurally.
- **SimplexNoise.cs**: Implements simplex noise for procedural generation.
- **WorldGenerator.cs**: Orchestrates the generation of a game world.

### Singleton

- **Singleton.cs**: Implements a simple singleton pattern.

### StateMachine

- **PlayerRunner.cs**: Manages the player's state machine.
- **State.cs**: Base class for defining states.
- **StateRunner.cs**: Manages the execution of states.
- **States/PlayerMotionState.cs**: Example player motion state.
- **States/STATE_PLAYER_MOTION.asset**: Serialized asset for the player motion state.

### Tilemaps

- **CustomSiblingRuleTile.cs**: Extends Unity's RuleTile to customize sibling tile behavior.
- **ExampleSiblingRuleTile.cs**: Demonstrates the usage of custom sibling rule tiles.

Feel free to explore and integrate these scripts into your Unity projects. Each folder contains a set of scripts that can be easily adapted for your specific needs. If you have any questions or suggestions, please don't hesitate to reach out.

Happy coding! 🚀