using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardPlayer : MonoBehaviour
{
    public TextMeshProUGUI positionText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI timeText;

    public Image background;
    public Color firstPlaceColor = Color.white;
    public Color secondPlaceColor = Color.white;
    public Color thirdPlaceColor = Color.white;
    public Color otherPlaceColor = Color.white;

    public RectTransform animatorUI;
    public float posXStart;
    public float time = .5f;

    public void Show(PlayerRaceResult player)
    {
        gameObject.SetActive(true);
        positionText.text = player.position.ToString();
        nameText.text = $"{player.name} {(player.isMine ? "(You)" : string.Empty)}";

        timeText.text = player.finished ? string.Format("{0:00}:{1:00}", (int)player.time / 60, player.time % 60) :
            $"{player.lapCout}/{RaceManager.instance.LapCount}";

        background.color = player.position switch
        {
            1 => firstPlaceColor,
            2 => secondPlaceColor,
            3 => thirdPlaceColor,
            _ => otherPlaceColor,
        };
    }

    private void OnEnable()
    {
        animatorUI.position = new Vector3(posXStart, animatorUI.position.y, animatorUI.position.z);
        animatorUI.LeanMoveLocalX(0, time).setEase(LeanTweenType.easeInQuad);
    }
}