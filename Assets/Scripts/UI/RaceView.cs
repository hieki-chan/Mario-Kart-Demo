using TMPro;
using UnityEngine;

public class RaceView : MonoBehaviour
{
    [Header("Race Start")]
    public TextMeshProUGUI countdownText;
    public float scaleCoutdownAniTime = .25f;
    public TextMeshProUGUI headerText;
    public RectTransform line;
    public float inOutTime = .33f;
    public AudioSource source;
    public AudioClip startClip;
    public AudioClip finalLapClip;

    [Header("Race FInished")]
    public GameObject raceFinishedPanel;

    [Header("Time")]
    public TextMeshProUGUI timeText;

    public void UpdateTime(float timeInSeconds)
    {
        timeText.text = string.Format("Time: {0:00}:{1:00}", (int)timeInSeconds / 60, timeInSeconds % 60);
    }

    public void SetText(string val)
    {
        countdownText.text = val;
        countdownText.rectTransform.localScale = Vector3.zero;
        LeanTween.scale(countdownText.rectTransform, Vector3.one, scaleCoutdownAniTime).setEaseInOutBack();
    }

    public void LineAnimationIn()
    {
        LeanTween.scale(line, new Vector3(0, 1, 1), inOutTime);
    }

    public void LineAnimationOut()
    {
        line.localScale = new Vector3(0, 1, 1);
        LeanTween.scale(line, new Vector3(1, 1, 1), inOutTime);
    }

    public void SetActiveHeader(bool state)
    {
        headerText.gameObject.SetActive(state);
    }

    public void PlayStartSound()
    {
        source.PlayOneShot(startClip);
    }

    public void OnFinalLap()
    {
        source.clip = finalLapClip;
        source.Play();
    }

    public void OnRaceFinished()
    {
        raceFinishedPanel.SetActive(true);
    }
}