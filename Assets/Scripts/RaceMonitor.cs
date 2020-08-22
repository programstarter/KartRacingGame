using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Realtime;
using Photon.Pun;

public class RaceMonitor : MonoBehaviourPunCallbacks
{
    public static bool racing = false;
    public GameObject[] countDownItems;
    public static int totalLaps = 1;

    CheckpointManager[] carsCPM;
    public GameObject gameOverPanel;
    public GameObject HUD;

    public GameObject[] carPrefabs;
    public Transform[] spawnPoints;

    public GameObject startRace;
    public GameObject waitingText;

    int playerCar;

    public AudioSource backgroundAS;
    public AudioSource gameOverAS;

    public Animator gameFadeAnimator;

    // Start is called before the first frame update
    void Start()
    {
        racing = false;

        Leaderboard.Reset();

        foreach (GameObject pic in countDownItems)
            pic.SetActive(false);
        
        gameOverPanel.SetActive(false);
        gameOverAS.Stop();

        startRace.SetActive(false);
        waitingText.SetActive(false);

        playerCar = PlayerPrefs.GetInt("PlayerCar");
        int randomStartPos = Random.Range(0, spawnPoints.Length);
        Vector3 startPos = spawnPoints[randomStartPos].position;
        Quaternion startRot = spawnPoints[randomStartPos].rotation;
        GameObject pCar = null;

        if (PhotonNetwork.IsConnected)
        {
            startPos = spawnPoints[PhotonNetwork.LocalPlayer.ActorNumber - 1].position;
            startRot = spawnPoints[PhotonNetwork.LocalPlayer.ActorNumber - 1].rotation;

            if(NetworkedPlayer.LocalPlayerInstance == null)
            {
                pCar = PhotonNetwork.Instantiate(carPrefabs[playerCar].name, startPos, startRot, 0);
            }

            if(PhotonNetwork.IsMasterClient)
            {
                startRace.SetActive(true);
                totalLaps = 1;
            }
            else
            {
                waitingText.SetActive(true);
                totalLaps = 1;
            }
        }
        else
        {
            pCar = Instantiate(carPrefabs[playerCar]);
            pCar.transform.position = startPos;
            pCar.transform.rotation = startRot;

            foreach (Transform t in spawnPoints)
            {
                if (t == spawnPoints[randomStartPos]) continue;
                GameObject car = Instantiate(carPrefabs[Random.Range(0, carPrefabs.Length)]);
                car.transform.position = t.position;
                car.transform.rotation = t.rotation;
            }
            StartGame();
        }

        SmoothFollow.playerCar = pCar.gameObject.GetComponent<Drive>().rb.gameObject.transform;
        pCar.GetComponent<AIController>().enabled = false;
        pCar.GetComponent<Drive>().enabled = true;
        pCar.GetComponent<PlayerController>().enabled = true;
    }

    IEnumerator PlayCountDown()
    {
        yield return new WaitForSeconds(gameFadeAnimator.runtimeAnimatorController.animationClips[0].length);
        foreach(GameObject pic in countDownItems)
        {
            pic.SetActive(true);
            yield return new WaitForSeconds(1);
            pic.SetActive(false);
        }
        racing = true;
        HUD.SetActive(true);
    }

    private void LateUpdate()
    {
        if (!racing)
        {
            HUD.SetActive(false);
            return;
        }

        int finishedCount = 0;
        foreach(CheckpointManager cpm in carsCPM)
        {
            if (cpm.lap == totalLaps + 1)
                finishedCount++;
        }
        if(finishedCount == carsCPM.Length)
        {
            if (backgroundAS.volume > 0)
            {
                backgroundAS.volume -= 0.4f * Time.deltaTime;
                StartCoroutine(GameOverTransition(1 / 0.4f));
            }
        }
    }

    IEnumerator GameOverTransition(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        HUD.SetActive(false);
        GameObject[] playerNameUI = GameObject.FindGameObjectsWithTag("PlayerNameUI");
        for (int i = 0; i < playerNameUI.Length; i++)
            playerNameUI[i].SetActive(false);
        backgroundAS.Stop();
        if (!gameOverAS.isPlaying) gameOverAS.Play();
        if (!gameOverPanel.gameObject.activeInHierarchy) gameOverPanel.SetActive(true);
    }

    IEnumerator GameFadeOut_RestartLevel()
    {
        gameFadeAnimator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(gameFadeAnimator.runtimeAnimatorController.animationClips[1].length);

        if (PhotonNetwork.IsConnected)
            photonView.RPC("RestartGame", RpcTarget.All, null);
        else
            SceneManager.LoadScene("GameOne");
    }

    IEnumerator GameFadeOut_BackToMainMenu()
    {
        gameFadeAnimator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(gameFadeAnimator.runtimeAnimatorController.animationClips[1].length);

        if (PhotonNetwork.IsConnected)
            PhotonNetwork.LeaveRoom();
        else
            SceneManager.LoadScene("MainMenu");
    }

    [PunRPC]
    public void RestartGame()
    {
        PhotonNetwork.LoadLevel("GameOne");
    }

    public void RestartLevel()
    {
        racing = false;
        StartCoroutine(GameFadeOut_RestartLevel());
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("MainMenu");
    }

    public void BackToMainMenu()
    {
        StartCoroutine(GameFadeOut_BackToMainMenu());
    }

    public void BeginGame()
    {
        string[] aiNames = { "Howard", "Ben", "James", "Edward", "Frank", "Daniel" };

        for (int i = PhotonNetwork.CurrentRoom.PlayerCount; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            Vector3 startPos = spawnPoints[i].position;
            Quaternion startRot = spawnPoints[i].rotation;
            int r = Random.Range(0, carPrefabs.Length);

            object[] instanceData = new object[1];
            instanceData[0] = (string)aiNames[Random.Range(0, aiNames.Length)];

            GameObject AIcar = PhotonNetwork.Instantiate(carPrefabs[r].name, startPos, startRot, 0, instanceData);
            AIcar.GetComponent<AIController>().enabled = true;
            AIcar.GetComponent<Drive>().enabled = true;
            AIcar.GetComponent<Drive>().networkName = (string)instanceData[0];
            AIcar.GetComponent<PlayerController>().enabled = false;
        }

        if(PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("StartGame", RpcTarget.All, null);
        }
    }

    [PunRPC]
    public void StartGame()
    {
        StartCoroutine(PlayCountDown());
        startRace.SetActive(false);
        waitingText.SetActive(false);

        GameObject[] cars = GameObject.FindGameObjectsWithTag("Car");
        carsCPM = new CheckpointManager[cars.Length];
        for (int i = 0; i < cars.Length; i++)
            carsCPM[i] = cars[i].GetComponent<CheckpointManager>();
    }
}
