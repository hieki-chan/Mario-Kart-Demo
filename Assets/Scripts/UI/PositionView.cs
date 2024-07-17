using UnityEngine;
using TMPro;

public class PositionView : MonoBehaviour
{
    const string MPH = " mph";
    const string firstPlace = "1st";
    const string secondPlace = "2nd";
    const string thirdPlace = "3rd";
    public TextMeshPro speedText;
    public TextMeshPro positionText;

    public void UpdateSpeed(int position)
    {
        speedText.text = position + MPH;
    }

    public void UpdatePosition(int position)
    {
        positionText.text = position switch
        {
            1 => firstPlace,
            2 => secondPlace,
            3 => thirdPlace,
            _ => $"{position}th"
        };
    }
}