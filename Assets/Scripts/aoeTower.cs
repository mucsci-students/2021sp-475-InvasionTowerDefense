using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aoeTower : Tower
{
    public GameObject bulletPrefab;
    public float bulletSpeed = 2f;
    public int bulletCount = 6;

    private List<GameObject> bullets = new List<GameObject>();

    public override void Start()
    {
        base.Start(); 
    }

    public override void Update()
    {
        base.Update(); 
        destroyBullet();
    }

    public override void _Fire (GameObject target)
    {
        float angle_ref = 360f/(float)bulletCount;
        float angle = angle_ref;

        for (int i=0; i<bulletCount; i++)
        {
            Transform spawn_loc = base.bulletSpawnLocation;
            GameObject bullet = (GameObject) Instantiate(bulletPrefab, spawn_loc.position, spawn_loc.rotation);
            bullets.Add(bullet);
            bullet.transform.Rotate(0,0,angle);
            
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.velocity = bullet.transform.up * bulletSpeed;
            angle += angle_ref;
        }

    }

    public override void _upgrade ()
    {
        bulletCount += 3;
        base.damage *= base.damageScaling;
    }

    public override void _ShowDetails ()
    {
        string info = "# of Bullets: " + bulletCount + "\n";
        info += "Damage: " + base.damage + "\n";
        info += "Range: " + base.range + "\n";
        base.UIInfo.text = info;
    }

    public void destroyBullet()
    {
        Vector2 tower_position = base.cartesianToIsometric (cullAngle.transform.position);

        for(int i=0; i < bullets.Count; i++) 
        {
            GameObject bullet = bullets[i];
            if (bullet == null)
            {
                bullets.RemoveAt(i);
                continue;
            }
            Vector2 bullet_pos = base.cartesianToIsometric (bullet.transform.position);
            float curDistance = Vector2.Distance (bullet_pos, tower_position) * 1.2f;
            if (curDistance >= base.range)
            {
                bullets.RemoveAt(i);
                Destroy(bullet);
            }
        }
    } 

}


/*
Vector3 enemyPos = target.transform.position;
            Vector3 start = base.bulletSpawnLocation.transform.position;
            Vector3 cullAngleLoc = base.cullAngle.transform.position;
            Vector3 lookDirection = cullAngleLoc - enemyPos;

            base.bulletSpawnLocation.transform.LookAt(target.transform, Vector3.up );

            Transform spawn_loc = base.bulletSpawnLocation.transform;
            GameObject bullet = (GameObject) Instantiate(bulletPrefab, spawn_loc.position, spawn_loc.rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.velocity = spawn_loc.up * bulletSpeed;

            base.lastShot = Time.time;
            base.damageTarget(target);
*/
