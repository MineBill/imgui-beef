cmake_minimum_required(VERSION 3.20)
project(BeefImGui)

set(CMAKE_MSVC_RUNTIME_LIBRARY "MultiThreaded$<$<CONFIG:Debug>:Debug>" CACHE INTERNAL "")

include(FetchContent)
FetchContent_Declare(
    cimgui_repo
    GIT_REPOSITORY https://github.com/cimgui/cimgui
    GIT_TAG        2e5db87e996af08b8b4162ca324ab0d7964abbf8
)
FetchContent_Populate(cimgui_repo)

add_library(cimgui STATIC
    ${cimgui_repo_SOURCE_DIR}/cimgui.cpp
    ${cimgui_repo_SOURCE_DIR}/imgui/imgui.cpp
    ${cimgui_repo_SOURCE_DIR}/imgui/imgui_draw.cpp
    ${cimgui_repo_SOURCE_DIR}/imgui/imgui_demo.cpp
    ${cimgui_repo_SOURCE_DIR}/imgui/imgui_widgets.cpp
    ${cimgui_repo_SOURCE_DIR}/imgui/imgui_tables.cpp

    ${cimgui_repo_SOURCE_DIR}/imgui/backends/imgui_impl_vulkan.cpp
    ${cimgui_repo_SOURCE_DIR}/imgui/backends/imgui_impl_glfw.cpp
)

target_include_directories(cimgui PUBLIC ${cimgui_repo_SOURCE_DIR})
target_include_directories(cimgui PUBLIC ${cimgui_repo_SOURCE_DIR}/imgui)

if (WIN32)
    target_compile_definitions(cimgui PUBLIC "-DIMGUI_IMPL_API=extern \"C\" __declspec\(dllexport\)")
else(WIN32)
    target_compile_definitions(cimgui PUBLIC "-DIMGUI_IMPL_API=extern \"C\" ")
endif(WIN32)

target_compile_definitions(cimgui PUBLIC -DCIMGUI_USE_VULKAN -DCIMGUI_USE_GLFW -DIMGUI_IMPL_VULKAN_NO_PROTOTYPES)

find_package(Vulkan REQUIRED)

set(GLFW_VERSION 3.4)
FetchContent_Declare(
    glfw
    URL https://github.com/glfw/glfw/archive/refs/tags/${GLFW_VERSION}.tar.gz
)

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

add_custom_target(copy_libs ALL
    COMMENT "Copying library files"
)

function(copy_library target destination)
    if(CMAKE_SYSTEM_NAME STREQUAL "Windows")
        set(PLATFORM_SUFFIX "Win64")
    elseif(CMAKE_SYSTEM_NAME STREQUAL "Linux")
        set(PLATFORM_SUFFIX "Linux64")
    else()
        message(FATAL_ERROR "Unsupported platform: ${CMAKE_SYSTEM_NAME}")
    endif()
    add_custom_command(TARGET copy_libs POST_BUILD
        COMMAND ${CMAKE_COMMAND} -E make_directory "${CMAKE_SOURCE_DIR}/${destination}/$<CONFIG>-${PLATFORM_SUFFIX}"
        COMMAND ${CMAKE_COMMAND} -E copy $<TARGET_FILE:${target}> "${CMAKE_SOURCE_DIR}/${destination}/$<CONFIG>-${PLATFORM_SUFFIX}"
        COMMENT "Copying ${target} library to ${CMAKE_SOURCE_DIR}/${destination}/$<CONFIG>-${PLATFORM_SUFFIX}"
    )
endfunction()

copy_library(cimgui "../dist")