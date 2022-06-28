using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour {
    [SerializeField] public ProgressBarTextType TextType;
    [SerializeField] public TextMeshProUGUI Text;
    [SerializeField] public float Maximum, Current;
    [SerializeField] public Image Mask;

    public void Set(int amount) {
        Current = amount;
        var bar = Current / Maximum;
        Mask.fillAmount = bar;
        UpdateText();
    }
    
    public void Set(float amount, float maximum) {
        Current = amount;
        Maximum = maximum;
        var bar = Current / Maximum;
        Mask.fillAmount = bar;
        UpdateText();
    }

    public void Set(float amount, float maximum, string text) {
        Current = amount;
        Maximum = maximum;
        var bar = Current / Maximum;
        Mask.fillAmount = bar;
        Text.text = text;
    }

    private void UpdateText() {
        Text.text = TextType switch {
            ProgressBarTextType.VALUES => $"{Current:0} / {Maximum:0}",
            ProgressBarTextType.TIME_SECONDS => $"{Current:0} seconds",
            _ => Text.text
        };
    }
}
