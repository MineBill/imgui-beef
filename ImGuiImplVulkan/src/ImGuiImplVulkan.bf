using System;

namespace ImGui
{
    public static class ImGuiImplVulkan
    {
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
    
        [LinkName("ImGui_ImplVulkan_AddTexture")]
        private static extern VkDescriptorSet AddTextureImpl(VkSampler sampler, VkImageView image_view, VkImageLayout image_layout);
        public static VkDescriptorSet AddTexture(VkSampler sampler, VkImageView image_view, VkImageLayout image_layout) => AddTextureImpl(sampler, image_view, image_layout);
        
        [LinkName("ImGui_ImplVulkan_CreateFontsTexture")]
        private static extern bool CreateFontsTextureImpl();
        public static bool CreateFontsTexture() => CreateFontsTextureImpl();
        
        [LinkName("ImGui_ImplVulkan_DestroyFontsTexture")]
        private static extern void DestroyFontsTextureImpl();
        public static void DestroyFontsTexture() => DestroyFontsTextureImpl();
        
        [LinkName("ImGui_ImplVulkan_Init")]
        private static extern bool InitImpl(InitInfo* info);
        public static bool Init(InitInfo* info) => InitImpl(info);
        
        [LinkName("ImGui_ImplVulkan_LoadFunctions")]
        private static extern bool LoadFunctionsImpl(function void(char8* function_name, void* user_data) loader_func, void* user_data);
        public static bool LoadFunctions(function void(char8* function_name, void* user_data) loader_func, void* user_data = null) => LoadFunctionsImpl(loader_func, user_data);
        
        [LinkName("ImGui_ImplVulkan_NewFrame")]
        private static extern void NewFrameImpl();
        public static void NewFrame() => NewFrameImpl();
        
        [LinkName("ImGui_ImplVulkan_RemoveTexture")]
        private static extern void RemoveTextureImpl(VkDescriptorSet descriptor_set);
        public static void RemoveTexture(VkDescriptorSet descriptor_set) => RemoveTextureImpl(descriptor_set);
        
        [LinkName("ImGui_ImplVulkan_RenderDrawData")]
        private static extern void RenderDrawDataImpl(DrawData* draw_data, VkCommandBuffer command_buffer, VkPipeline pipeline);
        public static void RenderDrawData(DrawData* draw_data, VkCommandBuffer command_buffer, VkPipeline pipeline = null) => RenderDrawDataImpl(draw_data, command_buffer, pipeline);
        
        [LinkName("ImGui_ImplVulkan_SetMinImageCount")]
        private static extern void SetMinImageCountImpl(uint32 min_image_count);
        public static void SetMinImageCount(uint32 min_image_count) => SetMinImageCountImpl(min_image_count);
        
        [LinkName("ImGui_ImplVulkan_Shutdown")]
        private static extern void ShutdownImpl();
        public static void Shutdown() => ShutdownImpl();
    }
}