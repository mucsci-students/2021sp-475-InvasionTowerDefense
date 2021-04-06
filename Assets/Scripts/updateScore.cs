using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class updateScore : MonoBehaviour
{
    public Text score;
    // Start is called before the first frame update
    void Start()
    {
        GameObject scoreHolder = GameObject.FindGameObjectsWithTag("Score")[0];
        score.text = "Number of Rounds Survived:\n" + scoreHolder.GetComponent<scoreHolder>().roundsSurvived;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
