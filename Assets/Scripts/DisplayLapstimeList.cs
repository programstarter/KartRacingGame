using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class DisplayLapstimeList : MonoBehaviour
{
    // 0 = name; 1 = TotalTime; 2-6 = LapsTime;
    public Text[] player0;
    public Text[] player1;
    public Text[] player2;
    public Text[] player3;

    public Text[] lapTitles;

    private void Start()
    {
        Debug.Log("Start Booking Time");
        DisplayLapsTime();
    }

    void DisplayLapsTime()
    {
        List<string> places = Leaderboard.GetPlaces();
        List<float> totalTimes = Leaderboard.GetTotalTimes();
        List<float[]> lapsTimes = Leaderboard.GetLapsTimes();

        for (int i = 0; i < 2; i++)
        {
            if (i == 0)
            {
                //name
                player0[i].text = places[0];
                player1[i].text = places[1];
                player2[i].text = places[2];
                player3[i].text = places[3];
            }
            if (i == 1)
            {
                //TotalTime
                player0[i].text = MinuteSecond(totalTimes[0]);
                player1[i].text = MinuteSecond(totalTimes[1]);
                player2[i].text = MinuteSecond(totalTimes[2]);
                player3[i].text = MinuteSecond(totalTimes[3]);
            }
        }

        // if TotalLaps >= 2, show every laptime
        if (RaceMonitor.totalLaps >= 2)
        {
            // Open Lap Title (1,2,3,4,5)
            for (int i = 0; i < RaceMonitor.totalLaps; i++)
            {
                float[] singleLapTime = { lapsTimes[0][i], lapsTimes[1][i], lapsTimes[2][i], lapsTimes[3][i] };
                
                player0[i + 2].text = MinuteSecond(singleLapTime[0]);
                player1[i + 2].text = MinuteSecond(singleLapTime[1]);
                player2[i + 2].text = MinuteSecond(singleLapTime[2]);
                player3[i + 2].text = MinuteSecond(singleLapTime[3]);

                float smallest = singleLapTime.Min();
                int singleLapFastest = singleLapTime.ToList().IndexOf(smallest);
                Color yellow = new Color(241 / 255f, 214 / 255f, 51 / 255f, 255 / 255f);
                if (singleLapFastest == 0)
                    player0[i + 2].color = yellow;
                if (singleLapFastest == 1)
                    player1[i + 2].color = yellow;
                if (singleLapFastest == 2)
                    player2[i + 2].color = yellow;
                if (singleLapFastest == 3)
                    player3[i + 2].color = yellow;

                lapTitles[i].gameObject.SetActive(true);
                player0[i + 2].gameObject.SetActive(true);
                player1[i + 2].gameObject.SetActive(true);
                player2[i + 2].gameObject.SetActive(true);
                player3[i + 2].gameObject.SetActive(true);
            }
        }
    }

    string MinuteSecond(float ConversionValue)
    {
        float runTime = ConversionValue;
        int runTimeMin = (int)(runTime / 59);
        float runTimeSec = runTime - runTimeMin * 59;
        float runTimeMSec = (runTimeSec - Mathf.Floor(runTimeSec)) * 100;

        string secStr = runTimeSec.ToString("F0");
        if (runTimeSec < 10) secStr = "0" + secStr;
        string mSecStr = runTimeMSec.ToString("F0");
        if (runTimeMSec < 10) mSecStr = "0" + mSecStr;

        string answer = runTimeMin.ToString() + ":" + secStr + "." + mSecStr;
        return answer;
    }
}
