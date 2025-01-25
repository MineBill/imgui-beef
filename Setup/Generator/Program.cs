using System;
using System.IO;

namespace ImGuiBeefGenerator
{
    class Program
    {
        public static void Main()
        {
            Console.WriteLine($"Working directory: {Directory.GetCurrentDirectory()}");
            var generator = new BindingGenerator();
            generator.Initialize();

            var outputFiles = generator.Generate(true);

            Console.WriteLine("Writing output files");

            foreach (var file in outputFiles)
                File.WriteAllText($"../{file.Key}", file.Value);
        }
    }
}
