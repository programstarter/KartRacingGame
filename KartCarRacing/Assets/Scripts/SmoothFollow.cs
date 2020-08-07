using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmoothFollow : MonoBehaviour
{
    Transform[] target;
    public static Transform playerCar;
    public float distance = 8.0f;
    public float height = 1.5f;
    public float heightOffset = 1.0f;
    public float heightDamping = 4.0f;
    public float rotationDamping = 2.0f;
    public RawImage rearCamView;
    public RawImage minimapCamView;
    int index = 0;

    int FP = -1;    //exchange camera view between first person and third person

    public Text speedText;
    public Text lapTimeSecondText;
    public Text lapTimeMinuteText;
    public Text lapTimeMicrosecondText;

    public GameObject showWrongMessage;
    bool isShowWrong = false;

    void Start()
    {
        if (PlayerPrefs.HasKey("FP"))
            FP = PlayerPrefs.GetInt("FP");

        if (target == null)
        {
            GameObject[] cars = GameObject.FindGameObjectsWithTag("Car");
            target = new Transform[cars.Length];
            for (int i = 0; i < cars.Length; i++)
            {
                target[i] = cars[i].transform;
                if (target[i] == playerCar)
                    index = i;
            }
            target[index].Find("RearCamera").gameObject.GetComponent<Camera>().targetTexture = (rearCamView.texture as RenderTexture);
            target[index].Find("MinimapCamera").gameObject.GetComponent<Camera>().targetTexture = (minimapCamView.texture as RenderTexture);
        }
    }

    
    void LateUpdate()
    {
        if (target == null)
        {
            GameObject[] cars = GameObject.FindGameObjectsWithTag("Car");
            target = new Transform[cars.Length];
            for(int i = 0; i < cars.Length; i++)
            {
                target[i] = cars[i].transform;
            }
            target[index].Find("RearCamera").gameObject.GetComponent<Camera>().targetTexture = (rearCamView.texture as RenderTexture);
            target[index].Find("MinimapCamera").gameObject.GetComponent<Camera>().targetTexture = (minimapCamView.texture as RenderTexture);
            return;
        }
            
            
        if(FP == 1)
        {
            transform.position = target[index].position - target[index].forward * 0.4f + target[index].up;
            transform.LookAt(target[index].position + target[index].forward * 3);
        }
        else
        {
            float wantedRotationAngle = target[index].eulerAngles.y;
            float wantedHeight = target[index].position.y + height;

            float currentRotationAngle = transform.eulerAngles.y;
            float currentHeight = transform.position.y;

            currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
            currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

            Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

            transform.position = target[index].position;
            transform.position -= currentRotation * Vector3.forward * distance;

            transform.position = new Vector3(transform.position.x,
                                    currentHeight + heightOffset,
                                    transform.position.z);

            transform.LookAt(target[index]);
        }
    }

    private void Update()
    {
        speedText.text = target[index].parent.gameObject.GetComponent<Drive>().currentSpeed.ToString("F0");
        float runTime = target[index].gameObject.GetComponent<CheckpointManager>().lapTime;
        int runTimeMin = (int)(runTime / 59);
        float runTimeSec = runTime - runTimeMin * 59;
        lapTimeMinuteText.text = runTimeMin.ToString();
        lapTimeSecondText.text = runTimeSec.ToString("F0");
        lapTimeMicrosecondText.text = ((runTimeSec - Mathf.Floor(runTimeSec)) * 100).ToString("F0");

        if (Input.GetKeyDown(KeyCode.P))
        {
            FP = FP * -1;
            PlayerPrefs.SetInt("FP", FP);
        }

        if(Input.GetKeyDown(KeyCode.T))
        {
            target[index].Find("RearCamera").gameObject.GetComponent<Camera>().targetTexture = null;
            target[index].Find("MinimapCamera").gameObject.GetComponent<Camera>().targetTexture = null;
            index++;
            if (index > target.Length - 1) index = 0;
            target[index].Find("RearCamera").gameObject.GetComponent<Camera>().targetTexture = (rearCamView.texture as RenderTexture);
            target[index].Find("MinimapCamera").gameObject.GetComponent<Camera>().targetTexture = (minimapCamView.texture as RenderTexture);
        }
    }

    private void FixedUpdate()
    {
        isShowWrong = target[index].gameObject.GetComponent<CheckpointManager>().IsWrongWay;
        if (isShowWrong)
        {
            if (!showWrongMessage.activeInHierarchy)
                showWrongMessage.SetActive(true);
        }
        else
        {
            showWrongMessage.SetActive(false);
        }
    }
}
