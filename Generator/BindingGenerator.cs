﻿using ImGuiBeefGenerator.ImGui;
using System.Collections.Generic;
using System.IO;
using System;
using Utf8Json;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ImGuiBeefGenerator
{
	public class BindingGenerator
	{
		private readonly List<IBinding> Bindings = new List<IBinding>();
		private int InstanceMethods = 0;
		private int Constructors = 0;
		private int Destructors = 0;

		public void Initialize()
		{
			Console.WriteLine("Initializing");

            const string basePath = "Native/cimgui/generator/output";

			var structsAndEnums = ReadBindingData($"{basePath}/structs_and_enums.json");

			List<ImGuiTypeDef> typedefs = ImGuiTypeDef.From(ReadBindingData($"{basePath}/typedefs_dict.json"));
			List<ImGuiEnum> enums = ImGuiEnum.From(structsAndEnums["enums"], ref typedefs);
			Bindings.AddRange(typedefs);
			Bindings.AddRange(enums);

			List<ImGuiMethodDefinition> methods = ImGuiMethodDefinition.From(ReadBindingData($"{basePath}/definitions.json"));
			InstanceMethods = methods.Count(b => b is ImGuiInstanceMethodDefinition);
			Constructors = methods.Count(b => b is ImGuiConstructorDefinition);
			Destructors = methods.Count(b => b is ImGuiDestructorDefinition);

			Bindings.AddRange(ImGuiStruct.From(structsAndEnums["structs"], ref methods));
			Bindings.AddRange(methods);
			Bindings.AddRange(ImGuiImplStruct.From(ReadBindingData($"{basePath}/impl_definitions.json")));
		}

		public Dictionary<string, string> Generate(bool includeGenerationInfo = false)
		{
			Console.WriteLine("Generating");

			var files = new Dictionary<string, string>();

            var file = new StringBuilder();
			if (includeGenerationInfo)
			{
				var info =
                    $"""
                     // -- GENERATION INFORMATION --
                     // Date: {DateTime.Now}
                     // Constructors: {Constructors}
                     // Destructors: {Destructors}
                     // Enums: {Bindings.Count(b => b is ImGuiEnum)}
                     // Global methods: {Bindings.Count(b => b is ImGuiGlobalMethodDefinition)}
                     // Instance methods: {InstanceMethods}
                     // Structs: {Bindings.Count(b => b is ImGuiStruct)}
                     // Typedefs: {Bindings.Count(b => b is ImGuiTypeDef)}

                     """;
                file.Append(info);
            }

			string secondLine = File.ReadLines("Generator/cimgui/cimgui.h").ElementAt(1);
			Regex pattern = new Regex(@"\d+(\.\d+)+");
			Match match = pattern.Match(secondLine);
			string version = match.Value;

			var text =
                $$"""
                using System;

                namespace ImGui;
                public static class ImGui
                {
                    public static char8* VERSION = "{{version}}";
                    public static int VERSION_NUM = {{version.Replace(".", "")}}00;
                    public static bool CHECKVERSION()
                    {
                        bool result = DebugCheckVersionAndDataLayout(VERSION, sizeof(IO), sizeof(Style), sizeof(Vec2), sizeof(Vec4), sizeof(DrawVert), sizeof(DrawIdx));
                        Runtime.Assert(result);
                        return result;
                    }

                    public static mixin ASSERT(bool condition) { Runtime.Assert(condition); }
                    public static mixin ASSERT(bool condition, String errorMsg) { Runtime.Assert(condition, errorMsg); }

                    public static DrawCallback* DrawCallback_ResetRenderState = (.)(void*)-1;

                    [CRepr]
                    public struct FILE
                    {
                        void* _Placeholder;
                    }

                    [CRepr]
                    public enum DockRequestType
                    {
                        None,
                        Dock,
                        Undock,
                        Split
                    }

                    [CRepr]
                    public struct DockRequest
                    {
                        DockRequestType Type;
                        Window* DockTargetWindow;
                        DockNode* DockTargetNode;
                        Window* DockPayload;
                        Dir DockSplitDir;
                        float DockSplitRatio;
                        bool DockSplitOuter;
                        Window* UndockTargetWindow;
                        DockNode* UndockTargetNode;
                    }

                    [CRepr]
                    public struct DockNodeSettings
                    {
                        ID             ID;
                        ID             ParentNodeId;
                        ID             ParentWindowId;
                        ID             SelectedWindowId;
                        char           SplitAxis;
                        char           Depth;
                        DockNodeFlags  Flags;
                        Vec2ih         Pos;
                        Vec2ih         Size;
                        Vec2ih         SizeRef;
                    }

                    [CRepr]
                    public struct DockPreviewData
                    {
                        DockNode FutureNode;
                        bool IsDropAllowed;
                        bool IsCenterAvailable;
                        bool IsSidesAvailable;
                        bool IsSplitDirExplicit;
                        DockNode* SplitNode;
                        Dir SplitDir;
                        float SplitRatio;
                        Rect[(.) Dir.COUNT] DropRectsDraw;
                    }

                    public static void FullscreenDockspace()
                    {
                        var viewport = ImGui.GetMainViewport();
                        ImGui.SetNextWindowPos(viewport.Pos);
                        ImGui.SetNextWindowSize(viewport.Size);
                        ImGui.SetNextWindowViewport(viewport.ID);

                        ImGui.PushStyleVar(.WindowPadding, .(0, 0));
                        ImGui.PushStyleVar(.WindowRounding, 0.0f);
                        ImGui.PushStyleVar(.WindowBorderSize, 0.0f);
                        ImGui.WindowFlags windowFlags = .MenuBar | .NoDocking | .NoTitleBar | .NoResize | .NoMove | .NoBringToFrontOnFocus | .NoNavFocus;
                        ImGui.Begin("MainDockspaceWindow", null, windowFlags);
                        ImGui.PopStyleVar(3);

                        ImGui.ID dockspaceId = ImGui.GetID("MainDockspace");
                        ImGui.DockSpace(dockspaceId);
                        ImGui.End();
                    }

                    // -- Auto-Generated --

                """;

            file.Append(text);
            foreach (var binding in Bindings.Where(b => !(b is ImGuiImplStruct)))
            {
                file.Append($"    {binding.Serialize()}\n");
            }

            file.Append('}');

			files.Add("src/ImGui.Gen.bf", file.ToString());

			files["ImGuiImplGlfw/src/ImGuiImplGlfw.bf"] = GenerateImplFile("ImGuiImplGlfw", Bindings);
            files["ImGuiImplVulkan/src/ImGuiImplVulkan.bf"] = GenerateImplFile("ImGuiImplVulkan", Bindings);
            // files["ImGuiImplOpenGL2/src/ImGuiImplOpenGL2.bf"] = GenerateImplFile("ImGuiImplOpenGL2", Bindings);
			// files["ImGuiImplOpenGL3/src/ImGuiImplOpenGL3.bf"] = GenerateImplFile("ImGuiImplOpenGL3", Bindings);
			// files["ImGuiImplSDL/src/ImGuiImplSDL.bf"] = GenerateImplFile("ImGuiImplSDL2", Bindings);

			return files;
		}

		private dynamic ReadBindingData(string file)
		{
			var stream = new FileStream(file, FileMode.Open);
			return JsonSerializer.Deserialize<dynamic>(stream);
		}

		private static string GenerateImplFile(string implName, List<IBinding> bindings)
		{
			var implFile =
$@"using System;

namespace ImGui
{{";
			implFile += bindings.Single(b => b is ImGuiImplStruct s && s.Name == implName).Serialize().Replace("\n", "\n    ");
			implFile = implFile.Remove(implFile.Length - 4);
			implFile += "}";
			return implFile;
		}
	}
}
