# Dear ImGui bindings for the Beef Programming Language 

> [!WARNING]
> Modified fork of https://github.com/RogueMacro/imgui-beef

This is a personal and updated version of https://github.com/RogueMacro/imgui-beef that includes CMakeLists file to properly and easily build cimgui.
The bindings generation is triggered manually when cimgui updates.

The project also attempts to utilize the `Setup` directory to provide better integration with the Beef package manager.

## Building
If used from the Beef IDE, the `Setup` project will be used to pull the required cimgui commit, and build it using CMake.

To do this, `cmake` and `git` **MUST** be available on your system (more specifically on your `PATH`).