using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmosDebug : MonoBehaviour
{
    
    [ExecuteInEditMode]
    private void OnDrawGizmos()
    {
        GameObject pathParent = GameObject.FindGameObjectsWithTag("Path")[0];
        GameObject[] path = new GameObject[pathParent.transform.childCount];

        for(int i = 0; i < pathParent.transform.childCount; i++)
        {
            path[i] = pathParent.transform.GetChild(i).gameObject;
        }

        Gizmos.color = Color.red;
        foreach (GameObject node in path)
        {
            Gizmos.DrawSphere(node.transform.position, 0.05f);
        }
    }
}
