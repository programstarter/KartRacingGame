using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Speedometer : MonoBehaviour
{
    private const float MAX_SPEED_ANGLE = -80;
    private const float ZERO_SPEED_ANGLE = 80;
    [SerializeField] private Transform needleTransform;
    [SerializeField] private Transform centerTransform;

    private float speedMax = 100;

    public Transform speedLabelTemplateTransform;

    // Start is called before the first frame update
    void Start()
    {
        CreateSpeedLabels();
    }

    private void CreateSpeedLabels()
    {
        int LabelAmout = 10;
        float totalAngleSize = ZERO_SPEED_ANGLE - MAX_SPEED_ANGLE + 10;
        for(int i = 1; i <= LabelAmout; i++)
        {
            Transform speedLabelTransform = Instantiate(speedLabelTemplateTransform, transform);
            float speedLabelNormalized = (float)i / (float)LabelAmout;
            float speedLabelAngle = 0 + speedLabelNormalized * totalAngleSize;
            speedLabelTransform.eulerAngles = new Vector3(0, 0, speedLabelAngle);
            speedLabelTransform.Find("SpeedLabelText").GetComponent<Text>().text = Mathf.RoundToInt((1 - speedLabelNormalized) * speedMax).ToString();
            speedLabelTransform.Find("SpeedLabelText").eulerAngles = Vector3.zero;
            speedLabelTransform.gameObject.SetActive(true);

            // speed >= 70, imply too high speed
            if(i <= 3)
            {
                speedLabelTransform.Find("dashImage").GetComponent<Image>().color = Color.red;
            }
        }

        needleTransform.SetAsLastSibling();
        centerTransform.SetAsLastSibling();
    }

    public void SpeedNeedleRotation(float speed)
    {
        float totalAngleSize = ZERO_SPEED_ANGLE - MAX_SPEED_ANGLE;

        float speedNormalized = speed / speedMax;

        float rotationValue = ZERO_SPEED_ANGLE - speedNormalized * totalAngleSize;
        needleTransform.eulerAngles = new Vector3(0, 0, rotationValue);
    }
}
