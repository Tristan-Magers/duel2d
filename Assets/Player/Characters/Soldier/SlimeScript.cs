using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeScript : PhysicsObj
{
    private Vector3 PosSet;
    bool move;

    // Start is called before the first frame update
    void Start()
    {
        ObHt = 1f;
        ObWd = 1f;

        move = false;
        w = 1.4f;
        gravity = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Pause) return;

        if (move)
        {
            Vector3 p = transform.position;

            PosSet = p;

            y = p.y;
            x = p.x;

            PosSet.z = 0;

            Move();

            PosSet.x = x;
            PosSet.y = y;

            transform.position = PosSet;
        } 
        
        move = false;
    }

    public void SlimeMove(Vector3 Going, float Ydif, float Xdif, int Ratio)
    {
        Vector3 current = transform.position;
        Going.y += Ydif;
        Going.x += Xdif;
        Vector3 dif = Going - current;

        Going = (dif / Ratio);

        vx = Going.x;
        vy = Going.y;

        move = true;

        if (vx > 1) vx = 1;
        if (vy > 1) vy = 1;

        if (vx < -1) vx = -1;
        if (vy < -1) vy = -1;
    }
}
