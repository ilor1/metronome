using TMPro;
using UnityEngine;

public class BeatsPerMinuteLabel : MonoBehaviour
{
    private TextMeshProUGUI _label;
    
    private void Start()
    {
        _label = GetComponent<TextMeshProUGUI>();
    }
    
    public void UpdateText(float value)
    {
        int bpm = Mathf.RoundToInt(value);
        _label.text = $"{bpm} beats/min";
    }
}
