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

    private void UpdateText() {
        Text.text = TextType switch {
            ProgressBarTextType.VALUES => $"{Current:0} / {Maximum:0}",
            ProgressBarTextType.TIME_SECONDS => $"{Current:0} seconds",
            _ => Text.text
        };
    }
}
