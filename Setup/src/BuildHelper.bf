using System;
using System.Diagnostics;
using System.IO;

namespace Setup;

public static class BuildHelper
{
	public static void ExecuteCmd(StringView cmd, StringView args, String stdOut = null, String stdErr = null)
	{
		var info = scope ProcessStartInfo()
			{
				UseShellExecute = false,
				RedirectStandardOutput = stdOut != null,
				RedirectStandardError = stdErr != null
			};

		info.SetFileName(cmd);
		info.SetArguments(args);
		SpawnedProcess process = scope .();

		if (process.Start(info) case .Err)
		{
			Console.WriteLine(scope $"Failed to execute: {cmd}");
			return;
		}

		FileStream outputStream = scope .();
		FileStream errStream = scope .();
		if (stdOut != null)
			process.AttachStandardOutput(outputStream);

		if (stdErr != null)
			process.AttachStandardOutput(errStream);

		if (stdOut != null)
		{
			StreamReader reader = scope .(outputStream);

			if (reader.ReadToEnd(stdOut) case .Err)
				Console.WriteLine("Failed to read process output");
		}

		if (stdErr != null)
		{
			StreamReader reader = scope .(errStream);

			if (reader.ReadToEnd(stdOut) case .Err)
				Console.WriteLine("Failed to read process output");
		}

		process.WaitFor();
	}
}