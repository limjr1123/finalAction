using System;
using UnityEngine;
using UnityEngine.UI;

public class ToggleManager : Singleton<ToggleManager>
{
    public Color32 selectedColor = new Color32(100, 100, 100, 255);
    public Color normalColor = Color.white;

    void Start()
    {
        Toggle[] allToggles = FindObjectsByType<Toggle>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (Toggle toggle in allToggles)
        {
            if (toggle.GetComponent<CharacterToggleUI>() != null)
                continue;

            toggle.transition = Selectable.Transition.None;
            Image toggleImage = toggle.GetComponent<Image>();

            UpdateColor(toggle, toggleImage);

            toggle.onValueChanged.AddListener((bool isSelected) =>
            {
                UpdateColor(toggle, toggleImage);
            });
        }
    }

    void UpdateColor(Toggle toggle, Image toggleImage)
    {
        toggleImage.color = toggle.isOn ? selectedColor : normalColor;
    }
}
