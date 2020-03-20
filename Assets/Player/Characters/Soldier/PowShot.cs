using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowShot : PhysicsObj
{
    private Vector3 PosSet;
    int countdown = -10, playernumber;

    // Start is called before the first frame update
    void Start()
    {
        ObHt = 1;
        ObWd = 1;
        w = 0.8f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Pause) return;

        if (ResetCycle) Destroy(gameObject);

        Vector3 p = transform.position;

        PosSet = p;

        y = p.y;
        x = p.x;

        Move();

        PosSet.x = x;
        PosSet.y = y;

        transform.position = PosSet;

        if (countdown < 0 && collision)
        {
            countdown = 11;
            vy = 0.19f;
            vx = 0;
        }

        if (countdown >= 0)
        {
            transform.localScale = new Vector3(3f - ((float)countdown / 18f * 2f), 3f - ((float)countdown / 18f * 2f), 1);
        }

        if (countdown == 0)
        {
            Booms[BoomCount + 1].Create(x, y, 4.2f, 0.5f, 2f, 1, 0, -1, 1f, 1.05f);
            Booms[BoomCount + 1].Create(x, y, 1.7f, 0f, 0, 2, 16, playernumber, 1, 1);
            Destroy(gameObject);
        }

        countdown--;
    }

    private void LateUpdate()
    {
        if (Pause) return;

        BombHit(-99);
    }

    public void ChangeVol(float newX, float newY, int pn)
    {
        vy = newY;
        vx = newX;
        playernumber = pn;
    }
}
