using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayLeaderboard : MonoBehaviour
{
    public Text first;
    public Text second;
    public Text third;
    public Text fourth;

    void LateUpdate()
    {
        List<string> places = Leaderboard.GetPlaces();
        first.text = places[0];
        second.text = places[1];
        third.text = places[2];
        fourth.text = places[3];
    }
}
