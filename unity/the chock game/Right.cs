using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Right : MonoBehaviour
{
    // Start is called before the first frame update
    float arrow_pos;
    float time1 = 0;
    float time2 = 0;
    public bool left = true;



    void Start()
    {
        arrow_pos = Random.Range(0, 4);
        if (arrow_pos > 1)
            left = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //gameObject.transform.position += new Vector3(0, -0.05f, 0);
        float temp = Time.deltaTime;
        time2 += temp;

        gameObject.transform.localScale += new Vector3(0.001f, 0.001f, 0);
        //gameObject.transform.position += new Vector3(arrow_pos * 0.045f - 0.07f, -0.05f, 0);

        float deltatime = time2 * time2 - time1 * time1;
        float a = (arrow_pos * 1.8f) - 1.8f*1.5f;
        gameObject.transform.position += new Vector3(a * deltatime, -2f * deltatime, 0);

        time1 = time2;
    }
}
