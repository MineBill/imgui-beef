using System;

namespace Setup;

class CMake
{
	public enum Configuration
	{
		Debug,
		Release,
	}

	public static void Build(StringView buildDir, Configuration config)
	{
		BuildHelper.ExecuteCmd(GetCMakePath(), scope $"--build {buildDir} --config {config.ToString(..scope String())}");
	}

	public static void Configure(StringView path, StringView buildDir)
	{
		BuildHelper.ExecuteCmd(GetCMakePath(), scope $"-S {path} -B {buildDir}");
	}

	private static StringView GetCMakePath()
	{
		return "cmake";
	}
}