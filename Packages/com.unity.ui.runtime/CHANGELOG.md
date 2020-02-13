# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.0.4] - 2020-01-21

### Added

- Hot reload: update the gameview display when assets are modified externally
- Now updates animations, schedulers and bindings at runtime
- Custom inspector for UIElementsSystemsInspector
- Support custom elements

### Fixed

- Multiple small fixes and optimizations
- Remove struct interface that was causing boxing allocation
- Fixed some events not getting pooled properly
- Add preserve attribute to prevent stripping
- Make PanelRenderer reloads itself automatically when UXML or USS files are modified externally
- Make PanelRenderer execute in edit mode.

## [0.0.3] - 2019-09-17

### Added

- PointerStationaryEvent
- Menu items to easily create a Panel GameObject with the right components.
- Added support for change tracking and live updates for the UIBuilder

## [0.0.2] - 2019-08-06

### Added

- Added new property to PanelRenderer to specify the main style sheet

### Changed

- Changed Unity style sheet to reflect Unity 2019.3
- Changed TaskList sample

### Fixed

- Fixed the root VisualElement of the panel to be marked as ":root" for USS selectors

## [0.0.1] - 2019-07-26

### Added

- Documentation

### Changed

- Remove the UIE prefix from all components.
- Renamed the package to com.unity.ui.runtime
