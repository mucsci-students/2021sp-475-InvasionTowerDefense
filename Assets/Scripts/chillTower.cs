using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chillTower : Tower
{
    public ParticleSystem particles;
    public float spread = 8f;
    public float spreadScaling = 1.6f;

    private List<GameObject> bullets = new List<GameObject>();


    public override void Start()
    {
        base.Start(); 
        particles = base.bulletSpawnLocation.GetComponent<ParticleSystem>();
        var sh = particles.shape;
        sh.arc = spread;
        particles.Stop();
    }

    public override void Update()
    {
        base.Update(); 
        GameObject target = base.getTarget();
        // rotate firepoint to face target
        if (target != null) 
        { 
            Transform fireptTransform = base.bulletSpawnLocation.transform;
            Vector3 dir = target.transform.position - fireptTransform.position;
            float angle = Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg;
            fireptTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    public override void _upgrade ()
    {
        particles = base.bulletSpawnLocation.GetComponent<ParticleSystem>();
        var sh = particles.shape;
        spread = spread * spreadScaling;
        sh.arc = spread;
        base.bulletSpawnLocation.GetComponent<collisionHelper>().chillAmount *= 0.3f;
        base.bulletSpawnLocation.GetComponent<collisionHelper>().chillTime += 1;
    }

    public override void _ShowDetails ()
    {
        collisionHelper col = base.bulletSpawnLocation.GetComponent<collisionHelper>();
        var sh = particles.shape;
        float arc = sh.arc;

        string info = "Spread: " + arc + "\n";
        info += "Slows to " + (100-(col.chillAmount*100)) + "% for " + col.chillTime + "s\n";
        info += "Range: " + base.range + "\n";
        base.UIInfo.text = info;
    }

    public override void _Fire (GameObject target)
    {
        StartCoroutine("unPause");
    }

    IEnumerator unPause()
    {
        particles.Emit(5);
        particles.Play();
        yield return new WaitForSeconds(3);
    } 

    void onDestroy()
    {
        particles.Clear();
        particles.Stop();
    }

}
