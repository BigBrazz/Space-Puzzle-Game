
using UnityEngine;
using UnityEngine.UI;

public class AreaZone : MonoBehaviour
{
    [SerializeField] private string areaName = "Unknown Area";
    [SerializeField] private Text areaNameText;
    [SerializeField] private float displayDuration = 3f;

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
        if (!other.CompareTag("Player") || areaNameText == null)
            return;

        areaNameText.text = areaName;
        areaNameText.gameObject.SetActive(true);
        showing = true;
        hideTimer = displayDuration;
    }
}
