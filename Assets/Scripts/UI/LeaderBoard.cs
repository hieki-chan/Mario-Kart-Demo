using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class LeaderBoard : MonoBehaviour
{
    public LeaderboardPlayer leaderboardPlayerPrefab;
    public Transform playerList;

    List<LeaderboardPlayer> playerResults = new List<LeaderboardPlayer>();

    private void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            var go = Instantiate(leaderboardPlayerPrefab);
            go.gameObject.SetActive(false);
            go.transform.SetParent(playerList, false);
            playerResults.Add(go);
        }

        gameObject.SetActive(false);

        RaceManager.instance.OnRaceFinished += Show;
    }

    public void Show(PlayerRaceResult[] players)
    {
        gameObject.SetActive(true);
        StartCoroutine(ShowSequence(players));
    }

    WaitForSeconds wait = new WaitForSeconds(.1f);
    IEnumerator ShowSequence(PlayerRaceResult[] players)
    {
        for (int i = 0; i < players.Length; i++)
        {
            playerResults[i].Show(players[i]);
            yield return wait;
        }
    }
}