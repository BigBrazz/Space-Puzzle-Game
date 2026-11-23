using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIOpenOptions
{
    public bool CloseOthers { get; }
    public bool CloseWhenAnotherOpens { get; }

    public UIOpenOptions(
        bool closeOthers = false,
        bool closeWhenAnotherOpens = false)
        
    {
        CloseOthers = closeOthers;
        CloseWhenAnotherOpens = CloseWhenAnotherOpens;
    }
}

public class UIManager : MonoBehaviour
{
    private readonly Dictionary<GameObject, UIOpenOptions> openUIElements = new Dictionary<GameObject, UIOpenOptions>();
    UIOpenOptions defaultOptions = new UIOpenOptions(closeOthers: false, closeWhenAnotherOpens: true);

    public void OpenUIElement(GameObject uiElement, UIOpenOptions options = null)
    {
        if (uiElement == null) return;
        if (options == null) options = defaultOptions;
        if (options.CloseOthers) CloseAllUIElements();
        openUIElements[uiElement] = options;
        uiElement.SetActive(true);
    }

    public void CloseUIElement(GameObject uiElement)
    {
        if (uiElement == null) return;
        openUIElements.Remove(uiElement);
        uiElement.SetActive(false);
    }

    public void CloseAllUIElements()
    {
        foreach (var uiElement in openUIElements.Keys)
        {
            if (uiElement != null)
            {
                uiElement.SetActive(false);
            }
        }
        openUIElements.Clear();
    }
}