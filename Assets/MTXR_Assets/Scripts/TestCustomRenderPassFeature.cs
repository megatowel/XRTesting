using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TestCustomRenderPassFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class TestFeatureSettings
    {
        public Vector3 Scale;
    }

    public TestFeatureSettings settings = new TestFeatureSettings();

    class CustomRenderPass : ScriptableRenderPass
    {
        private Vector3 _scale;
        public CustomRenderPass(Vector3 scale) 
        {
            _scale = scale;
        }

        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer testCommandBuffer = CommandBufferPool.Get("test");
            testCommandBuffer.Clear();
            testCommandBuffer.SetViewMatrix(renderingData.cameraData.GetViewMatrix() * Matrix4x4.Scale(_scale));
            context.ExecuteCommandBuffer(testCommandBuffer);
            testCommandBuffer.Clear();
            CommandBufferPool.Release(testCommandBuffer);
        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }
    }

    CustomRenderPass m_ScriptablePass;

    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass(settings.Scale);

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = RenderPassEvent.BeforeRenderingPrepasses;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


