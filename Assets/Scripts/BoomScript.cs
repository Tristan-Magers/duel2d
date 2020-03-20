using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomScript : ArenaObj
{

    int Last = -1;
    public bool active = false;

    // Start is called before the first frame update
    void Start()
    {
        //gameObject.GetComponent<MeshRenderer>().material.renderQueue = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Pause) return;

        //if (Arena.Global.Reset) Destroy(gameObject);
        if (Last > 0 && Last < 8) transform.localScale = transform.localScale * .8f;

        if (Last == 0)
        {
            transform.position = new Vector3(0, 1000, 0);
            active = false;
        }
        else Last--;
    }

    public void StartBoom()
    {
        active = true;
        Last = 15;
    }
}
