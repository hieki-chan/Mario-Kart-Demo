using TMPro;
using UnityEngine;

public class LapView : MonoBehaviour
{
    public TextMeshProUGUI lapText;

    public void UpdateLap(int currentLap, int maxLap)
    {
        lapText.text = $"Lap: {currentLap} / {maxLap}";
    }
}