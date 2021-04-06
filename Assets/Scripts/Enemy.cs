using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 100;
    public float health;
    public float moveSpeed = 4f;
    public float damage = 5;
    //public HealthBar healthBar;
    public List<GameObject> path;
    public GameObject gameManager;
    public GameObject healthbar;


    public int goalNum = 0;
    public float distFromGoal = 0;
    public float goldWorth = 30;
    public bool chilled = false;

    private GameObject currentGoal;

    void Start()
    {
        health = maxHealth;
        gameManager = GameObject.FindGameObjectsWithTag("GameManager")[0];
        GameObject pathParent = GameObject.FindGameObjectsWithTag("Path")[0];
        for(int i = 0; i < pathParent.transform.childCount; i++)
        {
            path.Add(pathParent.transform.GetChild(i).gameObject);
        }
        currentGoal = path[0];
        goalNum = 0;
        distFromGoal = 999;
    }

    // store order spawned so that we can target last?
    
    void Update()
    {
        Move();
        healthbar.transform.localScale = new Vector3((float)health/maxHealth, 1f, 1f);
    }

    void Move()
    {
        if (!currentGoal){ return; }

        Transform goalPos = currentGoal.transform;
        float dist = Vector2.Distance(transform.position, goalPos.position);
        if(dist > 0.1f)
        {
            transform.position = Vector3.MoveTowards (transform.position, goalPos.position, Time.deltaTime * moveSpeed);
            distFromGoal = dist;
        } else {
            if (path.Count > 1)
            {
                path.RemoveAt (0);
                currentGoal = path[0];
                goalNum++;
                distFromGoal = Vector2.Distance(transform.position, currentGoal.transform.position);
            } else {
                // at end of path
                gameManager.GetComponent<gameManager>().enemiesAlive -= 1;
                gameManager.GetComponent<gameManager>().health -= damage;
                Destroy(gameObject);
            }
        }
    }

    void Die()
    {
        gameManager.GetComponent<gameManager>().enemiesAlive -= 1;
        gameManager.GetComponent<gameManager>().money += goldWorth;
        Destroy(gameObject);
    }

    public void takeDamage (float amt)
    {
        health -= amt;
        if(health <= 0)
        {
            health = 0;
            Die();
        } 
        else if (health > maxHealth)
        {
            health = maxHealth;
        }
        return;
    }

}
