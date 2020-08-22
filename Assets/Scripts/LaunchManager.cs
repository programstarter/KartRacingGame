using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Photon.Realtime;
using Photon.Pun;

public class LaunchManager : MonoBehaviourPunCallbacks
{
    byte maxPlayersPerRoom = 4;
    bool isConnecting;
    public InputField playerName;
    public Slider lapSlider;
    public Text lapText;
    public Text feedbackText;
    string gameVersion = "1.2";

    public GameObject loadingScreenObj;
    public Slider loadingBar;
    public Text loadingText;
    AsyncOperation AsyncO;

    public Animator mainMenuFadeAnimator;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        if (PlayerPrefs.HasKey("PlayerName"))
            playerName.text = PlayerPrefs.GetString("PlayerName");
        isConnecting = false;
    }

    public void SetPlayerName(string name)
    {
        PlayerPrefs.SetString("PlayerName", name);
    }

    public void SetTotalLaps(Slider slider)
    {
        RaceMonitor.totalLaps = (int)slider.value;
        lapText.text = RaceMonitor.totalLaps.ToString();
    }

    public void ResetPlayerData()
    {
        PlayerPrefs.DeleteAll();
        playerName.text = "";
        RaceMonitor.totalLaps = 1;
        lapSlider.value = 1;
        lapText.text = "1";
    }

    public void ConnectSingle()
    {
        loadingScreenObj.SetActive(true);
        StartCoroutine(GetSceneLoadProgress());
    }

    IEnumerator GetSceneLoadProgress()
    {
        yield return new WaitForSeconds(0.1f);
        AsyncO = SceneManager.LoadSceneAsync("GameOne");
        AsyncO.allowSceneActivation = false;

        while (AsyncO.isDone == false)
        {
            loadingBar.value = AsyncO.progress;
            loadingText.text = (AsyncO.progress * 100) + "%";
            
            if (AsyncO.progress >= 0.9f)
            {
                loadingBar.value = 1;
                loadingText.text = "100%";
                mainMenuFadeAnimator.SetTrigger("FadeOut");
                float smoothWait = mainMenuFadeAnimator.runtimeAnimatorController.animationClips[0].length;
                yield return new WaitForSeconds(smoothWait);
                AsyncO.allowSceneActivation = true;
            }
            yield return null;
        }
    }

    public void ConnectNetwork()
    {
        feedbackText.text = "";
        isConnecting = true;

        PhotonNetwork.NickName = playerName.text;
        if (PhotonNetwork.IsConnected)
        {
            feedbackText.text += "\nJoining Room...";
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            feedbackText.text += "\nConnecting...";
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        if(isConnecting)
        {
            feedbackText.text += "\nOnConnectedToMaster...";
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        feedbackText.text += "\nFailed to join random room.";
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = this.maxPlayersPerRoom });
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        feedbackText.text += "\nDisconnected because" + cause;
        isConnecting = false;
    }

    public override void OnJoinedRoom()
    {
        feedbackText.text += "\nJoined Room with " + PhotonNetwork.CurrentRoom.PlayerCount + " players.";
        PhotonNetwork.LoadLevel("GameOne");
        mainMenuFadeAnimator.SetTrigger("FadeOut");
    }
}
