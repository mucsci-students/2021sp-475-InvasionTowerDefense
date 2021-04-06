using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    public float timeToLive = 6f;
    public float damage = 1.2f;

    private float delay = 0.1f;
    private float lastShrink = 0;
    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine("selfDestruct");
    }

    void Update()
    {
        float scale = 0.03f;

        float pos = gameObject.transform.rotation.eulerAngles.z - 180;
        float test = 0;

        if (pos < 0) 
        {
            test = Remap(pos, 0, -180, -scale, scale);
        } else {
            test = Remap(pos, 0, 180, -scale, scale);
        }

        if (pos < -90 || pos > 90)
        {
            //print(pos);
            this.GetComponent<SpriteRenderer>().sortingOrder = -10;
        }

        if (Time.time - lastShrink > delay)
        {
            transform.localScale += new Vector3(-test, -test, -test);
            lastShrink = Time.time;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject gmobj = collision.gameObject;
        if (gmobj.layer != 12)
        {
            if (gmobj.layer == 8) // 8 = Enemies
            {
                gmobj.GetComponent<Enemy>().takeDamage(damage);
                Destroy(gameObject);
            }
        }
    }

    IEnumerator selfDestruct()
    {
        yield return new WaitForSeconds(timeToLive);
        Destroy(gameObject);
    } 

    public static float Remap (float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
