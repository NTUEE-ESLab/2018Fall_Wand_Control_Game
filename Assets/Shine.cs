using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shine : MonoBehaviour
{
    float time ;
    // Start is called before the first frame update
    void Start()
    {
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
        time += Time.deltaTime;
        if (time > 0.3f)
            Destroy(gameObject);

        //time = 0f;
    }
}
