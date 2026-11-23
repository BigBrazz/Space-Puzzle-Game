using UnityEngine;
using UnityEngine.UI;

public class AreaZone : MonoBehaviour
{
    [SerializeField] private string areaName = "Unknown Area";
    [SerializeField] private Text areaNameText;
    [SerializeField] private float displayDuration = 3f;

    [Header("Environment")]
    [SerializeField] private Material skyboxMaterial;
    [SerializeField] private bool useFog = true;
    [SerializeField] private Color fogColor = Color.gray;
    [SerializeField] private float fogDensity = 0.01f;
    [SerializeField] private Light directionalLight;
    [SerializeField] private Color lightColor = Color.white;
    [SerializeField] private float lightIntensity = 1f;
    [SerializeField] private Color ambientColor = Color.gray;

    private float hideTimer = 0f;
    private bool showing = false;

    private void Update()
    {
        if (!showing || areaNameText == null)
            return;

        hideTimer -= Time.deltaTime;
        if (hideTimer <= 0f)
        {
            showing = false;
            areaNameText.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (areaNameText != null)
        {
            areaNameText.text = areaName;
            areaNameText.gameObject.SetActive(true);
            showing = true;
            hideTimer = displayDuration;
        }

        ApplyEnvironment();
    }

    private void ApplyEnvironment()
    {
        if (skyboxMaterial != null)
            RenderSettings.skybox = skyboxMaterial;

        RenderSettings.fog = useFog;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = fogDensity;
        RenderSettings.fogMode = FogMode.Exponential;

        RenderSettings.ambientSkyColor = ambientColor;

        if (directionalLight != null)
        {
            directionalLight.color = lightColor;
            directionalLight.intensity = lightIntensity;
        }

        DynamicGI.UpdateEnvironment();
    }
}
