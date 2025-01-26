using System;

namespace Setup;

class Program
{
	const StringView COMMIT = "9bc279c4d86c49bf032ae036670b06637079f656";

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

			System.IO.File.Copy("cmake-build/Debug/cimgui.lib", "../dist/Debug-Win64/cimgui.lib");
			System.IO.File.Copy("cmake-build/Release/cimgui.lib", "../dist/Release-Win64/cimgui.lib");
		} else
		{
			Runtime.Assert(false, "TODO");
		}
		return 0;
	}
}