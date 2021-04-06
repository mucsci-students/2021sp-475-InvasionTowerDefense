using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{
    public enum Mode 
    {
        First,
        Last,
        LowestHP,
        Closest,
    }
    [Header("UI Components")]
    [Space(10)]
    public GameObject canvas;
    public Text UIInfo;
    public Button upgradeButton;
    [Space(20)]

    public Transform bulletSpawnLocation;
    // this is used in atanh2 function to determine wether laser is behind tower sprite or not
    public GameObject cullAngle;

    [Header("Tower Stats")]
    [Space(10)]
    public float cost = 600;
    public float sellPrice = 0;
    public float damage = 3f;
    public float damageDelay = 0.1f;
    public float range = 10f;
    public int level = 1;
    public int maxLevel = 3;
    public Mode targetingMode = Mode.First;
    public float damageScaling = 1.5f;
    public float rangeScaling = 1.5f;
    [Space(20)]

    [Header("Level Sprites")]
    [Space(10)]
    public GameObject[] levelSprites;
    [Space(20)]

    public string towerName = "name";
    // private variables
    [HideInInspector]
    public LineRenderer tower_range;
    [HideInInspector]
    public float lastShot;
    //[HideInInspector]
    public bool mouseOver = false;
    [HideInInspector]
    public bool detailsUp = false;
    [HideInInspector]
    public bool beingPlaced = false;
    [HideInInspector]
    public Color rangeColor = Color.black;
    //[HideInInspector]


    private gameManager manager;
    private float[] costPerLevel = {0, 1000, 2500, 5000};



    public virtual void Start()
    {
        range /= 10;
        lastShot = 0;
        sellPrice = cost * 0.75f; 

        tower_range = cullAngle.GetComponent<LineRenderer>();
        tower_range.startWidth = 0.05f;
        tower_range.endWidth = 0.05f;
        
        GameObject obj = GameObject.FindGameObjectsWithTag("GameManager")[0];
        manager = obj.GetComponent<gameManager>();
    }

    public virtual void Update()
    {
        tower_range.startColor = rangeColor;
        tower_range.endColor = rangeColor;
        if (beingPlaced)
        {
            HideDetails();
            ShowRange();
            return;
        }
        
        if (mouseOver)
        {
            ShowRange();
        } else {
            if (tower_range)
            {
                EraseLineRenderer(tower_range);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if(mouseOver)
            {
                ShowDetails();
            } else {
                HideDetails();
            }
        }

        if (detailsUp)
        {
            ShowRange();
        }

        GameObject target = getTarget();
        if (target != null) 
        { 
            Fire (target);
        } else {
            noTarget ();
        }
    }

    public void EraseLineRenderer (LineRenderer l)
    {
        Vector3[] zero_len = new Vector3[] { new Vector3(0,0,0), new Vector3(0,0,0) };
        l.positionCount = 2;
        l.SetPositions(zero_len);
        return; 
    }

    public virtual void _Fire(GameObject target)
    {
        print("Implement _Fire in subclass");
    }

    public virtual void Fire(GameObject target)
    {
        if (Time.time - lastShot > damageDelay)
        {
            lastShot = Time.time;
            _Fire(target);
            damageTarget(target);
        }
    }

    public virtual void noTarget()
    {
        return;//print("Implement notarget in subclass");
    }


    public void ShowRange()
    {
        tower_range.startWidth = 0.05f;
        tower_range.endWidth = 0.05f;

        // # of lines the circle is composed of
        int segments = 50;
        float radius = range;
        tower_range.positionCount = segments + 1;
        float x, y;

        float angle = 25;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Cos (Mathf.Deg2Rad * angle)*radius;
            y = Mathf.Sin (Mathf.Deg2Rad * angle)*radius;
            Vector3 pointOnCircle = new Vector3(x, y, 0);
            pointOnCircle = cartesianToIsometric (pointOnCircle);

            // idk why this needs to be done but removing it flips the render.
            float tmp = pointOnCircle.x;
            pointOnCircle.x = pointOnCircle.y;
            pointOnCircle.y = tmp;

            tower_range.SetPosition(i, pointOnCircle);
            angle += (360f / segments);
        }

    }

    public void ShowDetails ()
    {
        _ShowDetails();
        
        if (level < maxLevel)
        {
            upgradeButton.gameObject.GetComponentInChildren<Text>().text = "Upgrade (" + costPerLevel[level] + ")";
        } else {
            upgradeButton.gameObject.GetComponentInChildren<Text>().text = "Upgrade";
        }

        detailsUp = true;
        canvas.SetActive(true);
    }

    public virtual void _ShowDetails()
    {
        float dps = damage * ( 1f/damageDelay );
        string info = "Damage Per Second: " + dps + "\n";
        info += "Range: " + (range*(10f)) + "m\n";
        info += "Sale Price: $" + sellPrice;
        UIInfo.text = info;
    }

    public void HideDetails ()
    {
        detailsUp = false;
        canvas.SetActive(false);
        if (tower_range)
        {
            EraseLineRenderer(tower_range);
        }
    }

    public void Upgrade()
    {
        print("Cost: " + costPerLevel[level]);
        if (level < maxLevel && manager.money >= costPerLevel[level])
        {
            // remove old sprites
            Transform oldSpriteHolder = transform.Find("Sprites");
            Destroy(oldSpriteHolder.gameObject);
            // add new sprite holder
            GameObject newSpriteHolder = new GameObject("Sprites");
            newSpriteHolder.transform.SetParent (transform);
            // add new sprites to holder
            GameObject newSprites = (GameObject)Instantiate(levelSprites[level], transform.position, transform.rotation, newSpriteHolder.transform);
            // update bullet spawn point
            bulletSpawnLocation = newSprites.transform.Find("fire point");
            // move details UI up so its not in the way
            canvas.transform.position = canvas.transform.position + new Vector3(0, 0.4f, 0);
           
            // update details UI
            _upgrade();
            manager.money -= costPerLevel[level];
            sellPrice += costPerLevel[level] * 0.5f;
            level++;
            ShowDetails ();
        }
    }

    public virtual void _upgrade()
    {
         // update stats
        damage *= damageScaling;
        range *= rangeScaling;
        
    }

    public void Sell()
    {
        manager.money += sellPrice;
        Destroy(gameObject);
    }

    public void DropdownValueChanged(Dropdown change)
    {
        switch (change.value)
        {
            case 0:
                targetingMode = Mode.First;
                break;
            case 1:
                targetingMode = Mode.Last;
                break;
            case 2:
                targetingMode = Mode.LowestHP;
                break;
            case 3:
                targetingMode = Mode.Closest;
                break;
            default:
                targetingMode = Mode.First;
                break;
        }
    }

    public void damageTarget(GameObject target)
    {
        Enemy targ = target.GetComponent<Enemy>();
        if (targ) 
        {
            targ.takeDamage(damage);
        }
        return;
    }

    public GameObject getTarget()
    {
        switch (targetingMode)
        {
            case Mode.First:
                return TargetFurthestOnPath ();
            case Mode.Last:
                return TargetEarliestOnPath ();
            case Mode.LowestHP:
                return TargetLowestHealth ();
            case Mode.Closest:
                return TargetClosestEnemyInRange ();
            default:
                return null;
        }
    }

    
    public GameObject TargetClosestEnemyInRange()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector2 position = cartesianToIsometric (cullAngle.transform.position);

        foreach (GameObject enemy in enemies)
        {
            Vector2 enemy_pos = cartesianToIsometric (enemy.transform.position);
            // Why mult by 1.2? Im done trying to solve this mathmatically and this result is passably accurate
            float curDistance = Vector2.Distance (enemy_pos, position) * 1.2f; 

            if (curDistance < distance && curDistance < range)
            {
                closest = enemy;
                distance = curDistance;
            }
        }
        return closest;
    }

    public GameObject TargetLowestHealth()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject target = null;
        float health = Mathf.Infinity;
        Vector2 position = cartesianToIsometric (cullAngle.transform.position);

        foreach (GameObject enemy in enemies)
        {
            Vector2 enemy_pos = cartesianToIsometric (enemy.transform.position);
            // Why mult by 1.2? Im done trying to solve this mathmatically and this result is passably accurate
            float curDistance = Vector2.Distance (enemy_pos, position) * 1.2f; 
            int goalNum_cur = enemy.GetComponent<Enemy>().goalNum; 
            float distToGoal_cur = enemy.GetComponent<Enemy>().distFromGoal; 
            float health_cur = enemy.GetComponent<Enemy>().health; 
            
            if (curDistance < range && health_cur < health )
            {
                target = enemy;
                health = health_cur;
            }
        }
        return target;
    }
//                                  *********************************************************************************************************************
    public GameObject TargetFurthestOnPath ()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject target = null;
        float distToGoal = Mathf.Infinity;
        int goalNum = -1;
        Vector2 position = cartesianToIsometric (cullAngle.transform.position);

        foreach (GameObject enemy in enemies)
        {
            Vector2 enemy_pos = cartesianToIsometric (enemy.transform.position);
            // Why mult by 1.2? Im done trying to solve this mathmatically and this result is passably accurate
            float curDistance = Vector2.Distance (enemy_pos, position) * 1.2f; 
            int goalNum_cur = enemy.GetComponent<Enemy>().goalNum; 
            float distToGoal_cur = enemy.GetComponent<Enemy>().distFromGoal; 
            
            if (curDistance < range && goalNum_cur >= goalNum && distToGoal_cur < distToGoal)
            {
                target = enemy;
                goalNum = goalNum_cur;
                distToGoal = distToGoal_cur;
            }
        }
        return target;
    }

    public GameObject TargetEarliestOnPath ()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject target = null;
        float distToGoal = Mathf.NegativeInfinity;
        int goalNum = 999;
        Vector2 position = cartesianToIsometric (cullAngle.transform.position);

        foreach (GameObject enemy in enemies)
        {
            Vector2 enemy_pos = cartesianToIsometric (enemy.transform.position);
            // Why mult by 1.2? Im done trying to solve this mathmatically and this result is passably accurate
            float curDistance = Vector2.Distance (enemy_pos, position) * 1.2f; 
            int goalNum_cur = enemy.GetComponent<Enemy>().goalNum; 
            float distToGoal_cur = enemy.GetComponent<Enemy>().distFromGoal; 
            
            if (curDistance < range && goalNum_cur <= goalNum && distToGoal_cur > distToGoal)
            {
                target = enemy;
                goalNum = goalNum_cur;
                distToGoal = distToGoal_cur;
            }
        }
        return target;
    }

    public Vector2 cartesianToIsometric(Vector3 point)
    {
        float x = point.x * 0.64f;
        float y = point.y * 1.28f;
        return new Vector2(x, y);
    }
}



/*  May come back to this

Range over which height varies.
float heightScale = 0.5f;
// Distance covered per second along X axis of Perlin plane.
float xScale = 0.5f;
float detail = 10;
float m = (enemyPos.y - start.y)/(enemyPos.x - start.x);
float b = enemyPos.y - m * enemyPos.x;
float d = Mathf.Sqrt( Mathf.Pow((enemyPos.x - start.x), 2) + Mathf.Pow((enemyPos.y - start.y), 2) );
points.Add(start);
add some noise to the line for effect;
print("Start: " + start);
print("Enemy: " + enemyPos);
for (int i=1; i<=detail; i++)
{
    float amnt = i/detail;
    //float val_on_line = m*(start.x + amnt*d);
    Vector2 pos = (Vector2)start + (Vector2)lookDirection*(amnt*d);
    //print(amnt + ": " + pos);

    float height = heightScale * Mathf.PerlinNoise(pos.x * xScale, pos.y);
    //pos.y += height;
    points.Add(new Vector3(pos.x, pos.y, start.z));
}
points.Add(enemyPos);
laser.SetPositions(points.ToArray());

*/