﻿using System.Collections.Generic;
using System.Linq;

namespace ImGuiBeefGenerator.ImGui
{
    public class ImGuiImplStruct : IBinding
    {
        public string Name { get; }
        public List<ImGuiMethodDefinition> Methods { get; }

        public ImGuiImplStruct(string name)
        {
            Name = name;
            Methods = new List<ImGuiMethodDefinition>();
        }

        public static List<ImGuiImplStruct> From(Dictionary<string, object> implDefinitions)
        {
            var structs = new List<ImGuiImplStruct>();

            foreach (var definition in implDefinitions)
            {
                var structName = definition.Key.Replace("ImGui_Impl", "ImGuiImpl");
                var definitionName = structName.Substring(structName.IndexOf('_') + 1);
                structName = structName.Replace($"_{definitionName}", "");

                if (!structs.Any(s => s.Name == structName))
                    structs.Add(new ImGuiImplStruct(structName));

                var implStruct = structs.Single(s => s.Name == structName);
                var variation = (Dictionary<string, object>) (definition.Value as List<object>).First();
                variation["funcname"] = definitionName;
                implStruct.Methods.Add(ImGuiMethodDefinition.FromVariation(variation));
            }

            return structs;
        }

        public string Serialize()
        {
            var serialized =
$@"
public static class {Name}
{{";

            if (Name == "ImGuiImplGlfw")
            {
                serialized +=
@"
    private typealias GLFWwindow = GLFW.GlfwWindow;
    private typealias GLFWmonitor = GLFW.GlfwMonitor;
";
            }
            else if (Name.StartsWith("ImGuiImplVulkan"))
            {
                serialized +=
@"
    private typealias DrawData = ImGui.DrawData;
    private typealias VkDescriptorSet = Bulkan.VkDescriptorSet;
    private typealias VkSampler = Bulkan.VkSampler;
    private typealias VkImageView = Bulkan.VkImageView;
    private typealias VkImageLayout = Bulkan.VkImageLayout;
    private typealias VkCommandBuffer = Bulkan.VkCommandBuffer;
    private typealias VkPipeline = Bulkan.VkPipeline;

    [CRepr]
    public struct InitInfo
    {
        public Bulkan.VkInstance                       Instance;
        public Bulkan.VkPhysicalDevice                 PhysicalDevice;
        public Bulkan.VkDevice                         Device;
        public uint32                                  QueueFamily;
        public Bulkan.VkQueue                          Queue;
        public Bulkan.VkDescriptorPool                 DescriptorPool;
        public Bulkan.VkRenderPass                     RenderPass;
        public uint32                                  MinImageCount;
        public uint32                                  ImageCount;
        public Bulkan.VkSampleCountFlags               MSAASamples;
        public Bulkan.VkPipelineCache                  PipelineCache;
        public uint32                                  Subpass;
        public uint32                                  DescriptorPoolSize;
        public bool                                    UseDynamicRendering;
        public Bulkan.VkPipelineRenderingCreateInfo    PipelineRenderingCreateInfo;
        public Bulkan.VkAllocationCallbacks*           Allocator;
        public function void(Bulkan.VkResult)          CheckVkResultFn;
        public Bulkan.VkDeviceSize                     MinAllocationSize;
    }
";
            }
            else if (Name.StartsWith("ImGuiImplOpenGL"))
            {
                serialized +=
@"
    private typealias char = char8;
    private typealias DrawData = ImGui.DrawData;

	[LinkName(""gladLoadGL"")]
    private static extern int GladLoadGL();
";
            }
            else if (Name == "ImGuiImplSDL2")
            {
                serialized +=
@"
    private typealias SDL_Window = SDL2.SDL.Window;
    private typealias SDL_Event = SDL2.SDL.Event;
    private typealias SDL_Renderer = SDL2.SDL.Renderer;

    [CRepr]
    public enum GamepadMode {
        AutoFirst,
        AutoAll,
        Manual
    }
";
            }

            foreach (var method in Methods)
                serialized += method.Serialize().Replace("\n", "\n    ");

            serialized = serialized.Remove(serialized.Length - 4);
            serialized += "}\n";
            return serialized;
        }
    }
}
