using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class laserTower : Tower
{

    private LineRenderer laser;

    public override void Start()
    {
        base.Start(); 

        laser = gameObject.GetComponent<LineRenderer>();
        laser.startWidth = 0.06f;
        laser.endWidth = 0.025f;
        laser.useWorldSpace = true;
        laser.sortingOrder = -1;
        base.rangeScaling = 1.25f;
    }

    public override void Fire (GameObject target)
    {
        Vector3 enemyPos = target.transform.position;
        Vector3 start = base.bulletSpawnLocation.position;
        Vector3 cullAngleLoc = base.cullAngle.transform.position;
        Vector3 lookDirection = cullAngleLoc - enemyPos;

        // positive if laser is behind tower, negative if in front
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        if (angle > 0)
        {
            laser.sortingOrder = 6;
        } else {
            laser.sortingOrder = -1;
        }

        // now set starting point of laser a bit down path for better effect
        lookDirection.Normalize();
        List<Vector3> points = new List<Vector3>();
        start += lookDirection*-0.2f;

        points.Add(start);
        points.Add(enemyPos);
        laser.SetPositions(points.ToArray());

        if (Time.time - lastShot > damageDelay)
        { 
            base.lastShot = Time.time;
            base.damageTarget(target);
        }
    }

    public override void noTarget()
    {
        base.EraseLineRenderer(laser);
    }
}
