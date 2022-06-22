using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour {
    [SerializeField] public ProgressBarTextType TextType;
    [SerializeField] public TextMeshProUGUI Text;
    public float Maximum;
    public float Current;
    public Image Mask;

    public void Set(int amount) {
        Current = amount;
        var bar = Current / Maximum;
        Mask.fillAmount = bar;
        UpdateText();
    }

    private void UpdateText() {
        switch (TextType) {
            case ProgressBarTextType.VALUES:
                Text.text = $"{Current:0} / {Maximum:0}";
                break;
            
            case ProgressBarTextType.TIME_SECONDS:
                Text.text = $"{Current:0} seconds";
                break;
        }
    }
}
