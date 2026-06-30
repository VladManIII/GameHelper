# GameHelper Product Requirements Document (PRD)

## Overview
GameHelper is a .NET 10 desktop application designed to enhance the gaming experience by allowing users to manage customizable game presets. Each preset contains a configurable list of key binds and macros, enabling users to optimize controls and automate repetitive actions for various games.

## Goals
- Provide a user-friendly interface for managing multiple game presets.
- Allow users to create, edit, and delete presets for different games.
- Enable customization of key binds and macros within each preset.
- Support saving, loading, and switching between presets seamlessly.

## Features

### 1. Preset Management
- Create new game presets.
- Edit existing presets (rename, update settings).
- Delete presets.
- Import/export presets for sharing or backup.

### 2. Key Bind Customization
- Assign keyboard or mouse keys to in-game actions.
- Support for single key binds and key combinations.
- Visual mapping of assigned keys.

### 3. Macro Assignment
- Create macros (sequences of key presses and delays).
- Assign macros to specific keys within a preset.
- Edit and delete macros.

### 4. User Interface
- Modern, intuitive UI for preset and bind management.
- Preset selection menu.
- Bind and macro editor with validation and preview.

### 5. Persistence & Settings
- Save presets and settings locally.
- Option to reset to default settings.

## Non-Goals
- No online multiplayer integration.
- No cloud sync in the initial release.

## Success Metrics
- Users can create and switch between presets without errors.
- Key binds and macros execute reliably in supported games.
- Positive user feedback on usability and customization.
