using UnityEngine;
using KartDemo;
using UnityEngine.Events;

/// <summary>
/// Check point Component is just holding data, most logic is in <see cref="RaceManager"/>
/// </summary>
public class CheckPoint : MonoBehaviour
{
    public bool isStartFinish;
    public bool isLast;
    public int order;

    public UnityAction<CheckPoint, PlayerTrack> OnPlayerCheck;

    private void Start()
    {
        order = transform.GetSiblingIndex();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PlayerConfig.PLAYER_TAG))
        {
            if(other.TryGetComponent<PlayerTrack>(out var track))
            {
                OnPlayerCheck?.Invoke(this, track);
            }
        }
    }

    public bool IsNext(int otherCheckPointOrder)
    {
        return order > otherCheckPointOrder;
    }
}
