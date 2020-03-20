using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NSpecBomb : PhysicsObj
{
    private Vector3 PosSet;
    int countdown = 4, playernumber;

    // Start is called before the first frame update
    void Start()
    {
        ObHt = .5f;
        ObWd = .5f;

        vx = -.06f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Pause) return;

        Vector3 p = transform.position;

        PosSet = p;

        y = p.y;
        x = p.x;

        BombHit(playernumber);

        Move();

        PosSet.x = x;
        PosSet.y = y;

        transform.position = PosSet;

        if (ResetCycle) Destroy(gameObject);

        if (countdown == 1)
        {
            Booms[BoomCount + 1].Create(x, y, 2.2f, 0.1f, 1, 2, 22, playernumber, 1, 1);
        }

        if (countdown == 0)
        {
            Booms[BoomCount + 1].Create(x, y, 4.9f, 0.51f, 2, 1, 0, -1, 1f, 1.13f);
            Destroy(gameObject);
        }

        countdown--;
    }

    public void SetImm(int player)
    {
        playernumber = player;
    }
}
