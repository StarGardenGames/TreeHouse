using UnityEngine;
 
 public class LightingController : MonoBehaviour {
 
     public bool fog;
     public Color fogColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);
     public FogMode fogMode = FogMode.ExponentialSquared;
     public float fogDensity = 0.01f;
     
     public float linearFogStart = 0.0f;
     public float linearFogEnd = 300.0f;
     
     public Color ambientLight = new Color(0.2f, 0.2f, 0.2f, 1.0f);
     public Material skyboxMaterial;
     
     public float haloStrength = 0.5f;
     
     public float flareStrength = 1.0f;
     
     void Awake() {
         RenderSettings.fog = fog;
         RenderSettings.fogColor = fogColor;
         RenderSettings.fogMode = fogMode;
         RenderSettings.fogDensity = fogDensity;
         
         RenderSettings.fogStartDistance = linearFogStart;
         RenderSettings.fogEndDistance = linearFogEnd;
         
         RenderSettings.ambientLight = ambientLight;
         RenderSettings.skybox = skyboxMaterial;
         
         RenderSettings.haloStrength = haloStrength;
         
         RenderSettings.flareStrength = flareStrength;
     }
     
 }
