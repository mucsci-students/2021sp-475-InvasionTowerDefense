using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scoreHolder : MonoBehaviour
{
    public int roundsSurvived = 0;

    public void Start()
    {
        DontDestroyOnLoad (transform.gameObject);
    }
}
