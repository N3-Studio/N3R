using System;
using System.Collections.Generic;
using System.Linq;
using ToonRP.FrameData;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace ToonRP
{
    public class ToonRenderPipeline : RenderPipeline
    {
        private readonly ToonRPAsset _settings;
        
        private RenderGraph _renderGraph;
        private ToonRenderGraphRecorder _renderGraphRecorder;
        private ContextContainer _contextContainer;
        
        private RTHandle _currentBackbuffer;

        public ToonRenderPipeline(ToonRPAsset asset)
        {
            _settings = asset;
            InitializeRenderPipeline();
            ConfigureRenderPipeline();
        }

        private void InitializeRenderPipeline()
        {
            RTHandles.Initialize(Screen.width, Screen.height);
            _renderGraph = new RenderGraph("ToonRPRenderGraph");
            _renderGraphRecorder = new ToonRenderGraphRecorder();
            _contextContainer = new ContextContainer();
        }

        private void ConfigureRenderPipeline()
        {
            GraphicsSettings.useScriptableRenderPipelineBatching = _settings.useSrpBatcher;
            Shader.globalRenderPipeline = _settings.pipelineName;
        }

        protected override void Dispose(bool disposing)
        {
            CleanRenderGraph();
        }

        private void CleanRenderGraph()
        {
            _renderGraph.Cleanup();
            _renderGraph = null;
            _contextContainer?.Dispose();
            _renderGraphRecorder = null;
            
            _currentBackbuffer?.Release();
        }

        protected override void Render(ScriptableRenderContext context, List<Camera> cameras)
        {
            
            BeginContextRendering(context, cameras);

            foreach (var camera in cameras.Where(camera => camera.enabled))
            {
                RenderCamera(context, camera);
            }
            
            _renderGraph.EndFrame();
            
            EndContextRendering(context, cameras); 
        }

        private void RenderCamera(ScriptableRenderContext context, Camera camera)
        {
            BeginCameraRendering(context, camera);

            if (!PrepareFrameData(context, camera))
            {
                EndCameraRendering(context, camera);
                return;
            }
            
            var cmd = CommandBufferPool.Get(camera.name);
            
            context.SetupCameraProperties(camera);
            
            RTHandles.SetReferenceSize(camera.pixelWidth, camera.pixelHeight);
            _currentBackbuffer ??= RTHandles.Alloc(BuiltinRenderTextureType.CameraTarget, "BackBuffer Color");
            
            RecordAndExecuteRenderGraph(context, camera, cmd);
            
            context.ExecuteCommandBuffer(cmd);
            
            CommandBufferPool.Release(cmd);
            context.Submit();
            
            EndCameraRendering(context, camera);
        }

        private bool PrepareFrameData(ScriptableRenderContext context, Camera camera)
        {
            if (!camera.TryGetCullingParameters(out var cullingParameters))
                return false;

            var cullingResults = context.Cull(ref cullingParameters);
            var cameraData = _contextContainer.GetOrCreate<CameraData>();
            cameraData.Camera = camera;
            cameraData.CullingResults = cullingResults;
            
            return true;
        }

        private void RecordAndExecuteRenderGraph(ScriptableRenderContext context, Camera camera, CommandBuffer cmd)
        {
            var renderGraphParameters = new RenderGraphParameters
            {
                commandBuffer = cmd,
                scriptableRenderContext = context,
                currentFrameIndex = Time.frameCount,
                executionId = camera.GetInstanceID()
            };

            try
            {
                _renderGraph.BeginRecording(renderGraphParameters);

                SetupResources(_renderGraph, camera);

                _renderGraphRecorder.RecordRenderGraph(_renderGraph, _contextContainer);

                _renderGraph.EndRecordingAndExecute();
            }
            catch (Exception e)
            {
                if (_renderGraph.ResetGraphAndLogException(e))
                    throw;
            }
        }

        private void SetupResources(RenderGraph renderGraph, Camera camera)
        {
            var resources = _contextContainer.GetOrCreate<ResourceData>();
            
            var backBufferFormat = SystemInfo.GetGraphicsFormat(DefaultFormat.LDR);
            var importInfo = new RenderTargetInfo
            {
                width = camera.pixelWidth,
                height = camera.pixelHeight,
                volumeDepth = 1,
                msaaSamples = 1,
                format = backBufferFormat,
                bindMS = false
            };
            
            resources.BackBufferColor = renderGraph.ImportTexture(_currentBackbuffer, importInfo);

            var width = camera.pixelWidth;
            var height = camera.pixelHeight;

            var colorDesc = new TextureDesc(width, height)
            {
                colorFormat = GraphicsFormat.R8G8B8A8_SRGB,
                clearBuffer = true,
                clearColor = camera.backgroundColor,
                name = "Color"
            };
            resources.ColorTexture = renderGraph.CreateTexture(colorDesc);
            
            var depthDesc = new TextureDesc(width, height)
            {
                clearBuffer = true,
                depthBufferBits = DepthBits.Depth32,
                name = "Depth"
            };
            resources.DepthTexture = renderGraph.CreateTexture(depthDesc);
            
            for (var i = 0; i < resources.GBuffer.Length; i++)
            {
                var gBufferDesc = new TextureDesc(width, height)
                {
                    colorFormat = GraphicsFormat.R8G8B8A8_UNorm,
                    clearBuffer = true,
                    clearColor = Color.black,
                    name = $"GBuffer_{i}"
                };
                resources.GBuffer[i] = renderGraph.CreateTexture(gBufferDesc);
            }
        }
    }
}