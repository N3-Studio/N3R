using System;
using System.Collections.Generic;
using ToonRP.FrameData;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace ToonRP
{
    public class ToonRenderPipeline : RenderPipeline
    {

        private RenderGraph _renderGraph;
        private ToonRenderGraphRecorder _renderGraphRecorder;
        private ContextContainer _contextContainer;

        public ToonRenderPipeline()
        {
            InitializeRenderPipeline();
        }

        private void InitializeRenderPipeline()
        {
            _renderGraph = new RenderGraph("ToonRPRenderGraph");
            _renderGraphRecorder = new ToonRenderGraphRecorder();
            _contextContainer = new ContextContainer();
        }

        protected override void Dispose(bool disposing)
        {
            CleanRenderGraph();
        }

        private void CleanRenderGraph()
        {
            _contextContainer?.Dispose();
            _renderGraphRecorder = null;
            _renderGraph.Cleanup();
            _renderGraph = null;
        }

        protected override void Render(ScriptableRenderContext context, List<Camera> cameras)
        {
            
            BeginContextRendering(context, cameras);

            foreach (var camera in cameras)
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

                _renderGraphRecorder.RecordRenderGraph(_renderGraph, _contextContainer);

                _renderGraph.EndRecordingAndExecute();
            }
            catch (Exception e)
            {
                if (_renderGraph.ResetGraphAndLogException(e))
                    throw;
            }
        }
    }
}