cmake_minimum_required(VERSION 3.20)
# cmake_policy(CMP0091 NEW)
project(BeefImGui)

# add_subdirectory(Generator/cimgui)

# add_definitions(IMGUI_STATIC)
add_library(cimgui STATIC
    Generator/cimgui/cimgui.cpp
    Generator/cimgui/imgui/imgui.cpp
    Generator/cimgui/imgui/imgui_draw.cpp
    Generator/cimgui/imgui/imgui_demo.cpp
    Generator/cimgui/imgui/imgui_widgets.cpp
    Generator/cimgui/imgui/imgui_tables.cpp

    Generator/cimgui/imgui/backends/imgui_impl_vulkan.cpp
    Generator/cimgui/imgui/backends/imgui_impl_glfw.cpp
)

target_include_directories(cimgui PUBLIC Generator/cimgui)
target_include_directories(cimgui PUBLIC Generator/cimgui/imgui)

set(CMAKE_MSVC_RUNTIME_LIBRARY "MultiThreaded$<$<CONFIG:Debug>:Debug>" CACHE INTERNAL "")

if (WIN32)
    target_compile_definitions(cimgui PUBLIC "-DIMGUI_IMPL_API=extern \"C\" __declspec\(dllexport\)")
else(WIN32)
    target_compile_definitions(cimgui PUBLIC "-DIMGUI_IMPL_API=extern \"C\" ")
endif(WIN32)

target_compile_definitions(cimgui PUBLIC -DCIMGUI_USE_VULKAN -DCIMGUI_USE_GLFW -DIMGUI_IMPL_VULKAN_NO_PROTOTYPES)

find_package(Vulkan REQUIRED)

set(GLFW_VERSION 3.4)
include(FetchContent)
	FetchContent_Declare(
    glfw
    URL https://github.com/glfw/glfw/archive/refs/tags/${GLFW_VERSION}.tar.gz)
	#GIT_REPOSITORY https://github.com/glfw/glfw )

FetchContent_GetProperties(glfw)
if (NOT glfw_POPULATED)
	set(FETCHCONTENT_QUIET NO)
    FetchContent_Populate(glfw)
	set(GLFW_BUILD_DOCS OFF CACHE BOOL "" FORCE)
	set(GLFW_BUILD_TESTS OFF CACHE BOOL "" FORCE)
	set(GLFW_BUILD_EXAMPLES OFF CACHE BOOL "" FORCE)
	set(BUILD_SHARED_LIBS OFF CACHE BOOL "" FORCE)
	add_subdirectory(${glfw_SOURCE_DIR} ${glfw_BINARY_DIR} EXCLUDE_FROM_ALL)
endif()

target_link_libraries(cimgui Vulkan::Vulkan glfw)

# copy cmake-build\Debug\cimgui.lib ..\dist\Debug-Win64\
# copy cmake-build\Release\cimgui.lib ..\dist\Release-Win64\