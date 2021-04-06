using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class Navigation : MonoBehaviour
{
    public void sceneTransition(string name) {
        SceneManager.LoadScene(name, LoadSceneMode.Single);
        return;
    }
}
