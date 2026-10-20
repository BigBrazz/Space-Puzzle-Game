
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractionPrompt : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text actionLabel;
    [SerializeField] private Image progressFill;
    private Image image;
    public void Show(string actionText, bool requiresHold)
    {
        gameObject.SetActive(true);
        if (actionLabel != null)
            actionLabel.text = $"[E] {actionText}";

        if (progressFill != null)
        {
            progressFill.gameObject.SetActive(requiresHold);
            progressFill.fillAmount = 0f;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetProgress(float progress)
    {
        if (progressFill != null)
            progressFill.fillAmount = Mathf.Clamp01(progress);
    }
}
