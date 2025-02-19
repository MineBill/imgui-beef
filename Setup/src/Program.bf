using System;

namespace Setup;

class Program
{
	const StringView COMMIT = "2e5db87e996af08b8b4162ca324ab0d7964abbf8";

	static int32 Main(String[] args)
	{
		BuildHelper.ExecuteCmd("git", "clone --depth 1 https://github.com/cimgui/cimgui Generator/cimgui");
		BuildHelper.ExecuteCmd("git", scope $"-C Generator/cimgui fetch --depth 1 origin {COMMIT}");
		BuildHelper.ExecuteCmd("git", scope $"-C Generator/cimgui checkout {COMMIT}");
		BuildHelper.ExecuteCmd("git", "-C Generator/cimgui submodule update --init");

		CMake.Configure(".", buildDir: "cmake-build");
		CMake.Build("cmake-build", .Debug);
		CMake.Build("cmake-build", .Release);

		let os = scope System.OperatingSystem();
		if (os.Platform == .Win32NT)
		{
			System.IO.Directory.CreateDirectory("../dist/Debug-Win64/");
			System.IO.Directory.CreateDirectory("../dist/Release-Win64/");

			// @TODO Maybe use cmake itself to make the copy, probably more robust and cross-platform.
			System.IO.File.Copy("cmake-build/Debug/cimgui.lib", "../dist/Debug-Win64/cimgui.lib");
			System.IO.File.Copy("cmake-build/Release/cimgui.lib", "../dist/Release-Win64/cimgui.lib");
		} else
		{
			Runtime.Assert(false, "TODO");
		}
		return 0;
	}
}