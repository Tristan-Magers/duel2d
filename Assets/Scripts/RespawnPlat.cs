using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPlat : ArenaObj
{
    int last = -1;
    int ID;
    public float vx, vy;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Pause) return;

        if (last == 0 || ResetCycle)
        {
            Plats[ID].active = false;
            Destroy(gameObject);
        }
            
        last--;
    }

    private void LateUpdate()
    {
        if (Pause) return;

        Vector3 OrgPos = transform.position;

        OrgPos.x += vx;
        OrgPos.y += vy;

        transform.position = OrgPos;

        vx = 0;
        vy = 0;
    }

    public void SetVals(int a, int b)
    {
        ID = a;
        last = b;
    }
    
    public void ChangeLast(int lasttimer)
    {
        last = lasttimer;
    }

}
