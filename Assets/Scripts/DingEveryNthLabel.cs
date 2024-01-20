using TMPro;
using UnityEngine;

public class DingEveryNthLabel : MonoBehaviour
{
    private TextMeshProUGUI _label;

    private void Start()
    {
        _label = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateText(float value)
    {
        int nth = Mathf.RoundToInt(value);

        switch (nth)
        {
            case 0:
                _label.text = "ding disabled";
                break;
            case 1:
                _label.text = "ding every time";
                break;
            case 2:
                _label.text = "ding every other";
                break;
            case 3:
                _label.text = "ding every 3rd";
                break;
            default:
                _label.text = $"ding every {nth}th";
                break;
        }
    }
}