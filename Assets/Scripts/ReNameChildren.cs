using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ReNameChildren : MonoBehaviour
{
    private void Awake()
    {
        Transform[] a = this.GetComponentsInChildren<Transform>();
        int number = 0;
        foreach(Transform b in a)
        {
            if(b.gameObject != this.gameObject)
            {
                b.gameObject.name = "" + number;
                number++;
            }
        }
    }
}