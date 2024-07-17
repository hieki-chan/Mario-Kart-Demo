using UnityEngine;

namespace KartDemo.Rendering
{
    [ExecuteAlways]
    public class CustomPostProcessing : MonoBehaviour
    {
        public Material material;

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source, destination, material);
        }
    }
}