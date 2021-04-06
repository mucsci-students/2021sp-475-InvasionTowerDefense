using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collisionHelper : MonoBehaviour
{
    public float chillAmount = 0.75f;
    public float chillTime = 2;

    void OnParticleCollision(GameObject other)
    {
        print(other.transform);
        if (other.layer == 8) // 8 == Enemies
        {
            Enemy curEnemy = other.GetComponent<Enemy>();
            if (!curEnemy.chilled)
            {
                StartCoroutine(chillEnemy(curEnemy));
            }
            
        }
    }

    IEnumerator chillEnemy(Enemy enemy)
    {
        enemy.chilled = true;
        float oldMovespeed = enemy.moveSpeed;
        enemy.moveSpeed *= chillAmount;
        yield return new WaitForSeconds(chillTime); 
        enemy.moveSpeed = oldMovespeed;
        enemy.chilled = false;
    }
}
