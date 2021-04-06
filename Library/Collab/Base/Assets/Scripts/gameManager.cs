using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class gameManager : MonoBehaviour
{
    [Header("UI Components")]
    [Space(5)]
    public GameObject pauseScreen;
    public GameObject defaultUI;
    public GameObject buyTowerUI;
    public Text waveNum;
    public Text enemysToSpawn;
    public Text moneyUI;
    public Text healthTxt;
    public Text TimeRemaining;
    [Space(20)]


    [Header("Wave settings")]
    [Space(5)]
    public int waveSize = 5;
    public float scaleAmount = 0.2f;
    public float scaleDecay = 0.3f;
    public float spawnDelay = .5f;
    public int waveNumber = 0;
    public int timeBetweenWaves = 25;
    public float tankPercentage = 0.25f;
    public float fastPercentage = 0.25f;
    public float bossPercentage = 0.1f;
    public float basePercentage = 0.4f;
    [Space(20)]

    public float money = 0;
    public GameObject baseEnemy, fastEnemy, tankEnemy, bossEnemy;
    public GameObject spawnPoint;
    public int enemiesAlive = 0;
    public int remaining = 0;
    public int timeCounter = 0;

    [Header("Tower Placement")]
    [Space(5)]
    public GameObject whole_map;
    public GameObject block;
    [Space(20)]

    [Header("Tower Info Panel")]
    [Space(5)]
    public Text towerName;
    public Text towerInfo;
    [Space(20)]

    public List<GameObject> path;
    public GameObject[] Towers;
    public float health = 100;


    private bool building = false;
    private GameObject towerBeingPlaced = null;
    private Camera cam;
    private GameObject[] allTowers;
    private bool isPaused = false;

    private List<GameObject> enemiesToSpawn = new List<GameObject>();

    void Start()
    {
        GameObject pathParent = GameObject.FindGameObjectsWithTag("Path")[0];
        for(int i = 0; i < pathParent.transform.childCount; i++)
        {
            path.Add(pathParent.transform.GetChild(i).gameObject);
        }

        cam = Camera.main;
        money = 1200.00f;
        StartCoroutine("waveManager");
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
        }

        if(!isPaused)
        {
            Time.timeScale = 1;
            pauseScreen.SetActive(false);
            defaultUI.SetActive(true);
            buyTowerUI.SetActive(true);
        } else {
            Time.timeScale = 0;
            pauseScreen.SetActive(true);
            defaultUI.SetActive(false);
            buyTowerUI.SetActive(false);
        }

        waveNum.text = "Wave " + waveNumber;
        enemysToSpawn.text = enemiesAlive + " alive, " + remaining + " left to spawn";
        moneyUI.text = "Money: $" + money;
        healthTxt.text = "Health: " + health;

        string t = "Time until next wave: ";
        t += timeCounter.ToString();
        t += "s";

        TimeRemaining.text = t;

        if (health <= 0)
        {
            health = 0;
            GameObject scoreHolder = GameObject.FindGameObjectsWithTag("Score")[0];
            scoreHolder.GetComponent<scoreHolder>().roundsSurvived = waveNumber;
            SceneManager.LoadScene("Game Over");
        }
        allTowers = GameObject.FindGameObjectsWithTag("Tower");
        
        checkIfMouseOverTower();

        
        //bool res = placeTower(path[0]);
        if (building && towerBeingPlaced != null) 
        {
            placeTower(towerBeingPlaced);
        }
    }

    public void resume()
    {
        isPaused = false;
    }

    void checkIfMouseOverTower()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 5f;
        Vector2 v = cam.ScreenToWorldPoint(mousePosition);
        Collider2D[] hits = Physics2D.OverlapPointAll(v);

        // reset all to false
        foreach (GameObject tower in allTowers)
        {
            tower.GetComponent<Tower>().mouseOver = false;
        }

        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D hit = hits[i];
            Tower t = hit.transform.gameObject.GetComponent<Tower>();
            bool isOverUI = hit.transform.gameObject.layer == 5;

            if (t)
            {
                t.mouseOver = true;
            }
            if (isOverUI)
            {
                Transform parent = hit.transform.parent;
                Tower parentT = parent.GetComponent<Tower>();
                if (parentT)
                {
                    parentT.mouseOver = true;
                }
            }
        }
    }

    /*
        basic logic
            - wait 'timeBetweenWaves' seconds between each wave or until player hits start wave
            - spawn 'waveSize' # of enemies at set interval ('spawnDelay' seconds)
            - after, increase waveSize for next wave (by 'scaleAmount')
    */
    private IEnumerator escapeManager ()
    {
        while (true)
        {
            if(!isPaused)
            {
                Time.timeScale = 1;
                pauseScreen.SetActive(false);
                defaultUI.SetActive(true);
                buyTowerUI.SetActive(true);
            } else {
                Time.timeScale = 0;
                pauseScreen.SetActive(true);
                defaultUI.SetActive(false);
                buyTowerUI.SetActive(false);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    private IEnumerator waveManager ()
    {
        while (true)
        {
            timeCounter = timeBetweenWaves;
            while (timeCounter > 0)
            {   
                timeCounter--;
                yield return new WaitForSeconds(1f);
            }

            print ("Starting Wave " + waveNumber);
            print (waveSize + " Enemies");
            yield return startWave ();
            // wait for all enemies to die
            while (enemiesAlive > 0)
            {   
                yield return new WaitForSeconds(1f);
            }

            if (waveNumber < 25 && waveNumber % 5 == 0)
            {
                spawnDelay /= 1.5f;
                scaleAmount = scaleAmount * scaleDecay;
            }

            if (waveNumber <= 5)
            {
                money += (float)waveNumber * 150;
            }
            waveSize = (int)(waveSize * (1+scaleAmount));
        }
    }

    private IEnumerator startWave () 
    {
        remaining = waveSize;
        waveNumber++;
        int queueCount = 0;

        if (waveNumber >= 5)
        {
            for (int i = 0; i < Mathf.Floor(waveSize * tankPercentage); i++)
            {
                enemiesToSpawn.Add(tankEnemy);
                queueCount++;
            }
        }
        if (waveNumber >= 8)
        {
            for (int i = 0; i < Mathf.Floor(waveSize * fastPercentage); i++)
            {
                enemiesToSpawn.Add(fastEnemy);
                queueCount++;
            }
            for (int i = 0; i < Mathf.Floor(waveSize * bossPercentage); i++)
            {
                enemiesToSpawn.Add(bossEnemy);
                queueCount++;
            }
        }
        
        for (int i = queueCount; i < waveSize; i++)
        {
            enemiesToSpawn.Add(baseEnemy);
            queueCount++;
        }
        enemiesAlive = 0;
        while (enemiesToSpawn.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, enemiesToSpawn.Count);
            GameObject curEnemy = enemiesToSpawn[randomIndex];
            Instantiate(curEnemy, spawnPoint.transform.position, curEnemy.transform.rotation);
            enemiesToSpawn.RemoveAt(randomIndex);
            remaining--;
            enemiesAlive++;
            yield return new WaitForSeconds(spawnDelay);
        }

        enemiesToSpawn.Clear();
    }

    public void startNextWave()
    {
        timeCounter = 0;
    }

    // i hope someone reads this joke...
    public void iBuyTower(string name)
    {
        Tower curTowerScript = null;
        GameObject curTower = null;
        // if we are already placing another tower remove it and spawn new type
        if (building)
        {
            Destroy(towerBeingPlaced);
            towerBeingPlaced = null;
        }

        switch (name)
        {
            case "laser":
                curTower = Towers[0];
                break;
            case "aoe":
                curTower = Towers[1];
                break;
            case "chill":
                curTower = Towers[2];
                break;
            default:
                return;
        }

        curTowerScript = curTower.GetComponent<Tower>();

        float cost = curTowerScript.cost;
        float dps = curTowerScript.damage * ( 1f/curTowerScript.damageDelay );
        double roundedDPS = Math.Round(dps, 2, MidpointRounding.AwayFromZero);

        string info = "Damage Per Second: " + roundedDPS + "\n";
        float roundedRange = curTowerScript.range*(10f);

        info += "Range: " + roundedRange.ToString() + "m\n";
        info += "Price: $" + cost + "\n";
        towerInfo.text = info;
        towerName.text = curTowerScript.towerName;

        if (money >= cost)
        {
            Vector3 mouse = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f); 
            Vector2 cursor_point = Camera.main.ScreenToWorldPoint(mouse);
            towerBeingPlaced = (GameObject) Instantiate(curTower, cursor_point, curTower.transform.rotation);
            towerBeingPlaced.GetComponent<Tower>().beingPlaced = true;

            building = true;
        }
    }

    public bool placeTower(GameObject gmobj)
    {
        if (Input.GetMouseButtonDown(1))
        {
            Destroy(towerBeingPlaced);
            towerBeingPlaced = null;
            return false;
            // minimize tower info menu?
        }
        if (towerBeingPlaced == null)
        {
            return false;
        }

        bool overSomeTower = false;
        foreach (GameObject tower in allTowers)
        {
            GameObject tower_cull_angle = tower.GetComponent<Tower>().cullAngle;
            isMouseOver tower_blocked = tower_cull_angle.GetComponent<isMouseOver>();
            bool isBeingPlaced = tower.GetComponent<Tower>().beingPlaced;
            if (tower_blocked && tower_blocked.over && !isBeingPlaced)
            {
                overSomeTower = true;
                break;
            }
        }

        isMouseOver blocked = block.GetComponent<isMouseOver>();
        isMouseOver on_map = whole_map.GetComponent<isMouseOver>();
        
        Vector3 mouse = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
        Vector2 cursor_point = Camera.main.ScreenToWorldPoint(mouse);
        towerBeingPlaced.transform.position = cursor_point; 

        if (on_map.over && !blocked.over && !overSomeTower)
        {
            towerBeingPlaced.GetComponent<Tower>().rangeColor = Color.green;
            
            if (Input.GetMouseButtonDown(0))
            {
                towerBeingPlaced.GetComponent<Tower>().beingPlaced = false;
                towerBeingPlaced.GetComponent<Tower>().rangeColor = Color.black;
                money -= towerBeingPlaced.GetComponent<Tower>().cost;
                towerBeingPlaced = null;
                building = false;
                return true;
            }
        } else {
            towerBeingPlaced.GetComponent<Tower>().rangeColor = Color.red;
        }
        return false;
    }


    // // debug/visual
    // void OnDrawGizmosSelected()
    // {
    //     Gizmos.color = Color.red;
    //     for (int i=0; i < path.Count; i++)
    //     {
    //         GameObject node = path[i];
    //         Gizmos.DrawSphere(node.transform.position, 0.1f);
    //     }
    // }

    
}
