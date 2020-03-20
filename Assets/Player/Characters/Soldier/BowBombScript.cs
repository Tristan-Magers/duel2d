using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowBombScript : PhysicsObj
{
    private Vector3 PosSet;
    int countdown = -10, playernumber;
    bool bomb;

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

        if (bomb)
        {
            if (countdown < 0 && collision)
            {
                countdown = 40;
                vy = 0.2f;
                vx = 0;
                gravity /= 2;
            }

            if (countdown < 0)
            {
                Hits[HitCount + 1].Create(x - (ObWd / 2), x + (ObWd / 2), y - (ObHt / 2), y + (ObHt / 2), playernumber, 0.2f, 90, 19, 2.7f, 6, 1, 1);
            }
            else
            {
                float size = 2f - ((float)countdown / 12f);
                Hits[HitCount + 1].Create(x - size, x + size, y - size, y + size, -1, 0.2f, 90, 14, 0.5f, 6, 1, 1);
                transform.localScale = new Vector3(4f - ((float)countdown / 40f * 3f), 4f - ((float)countdown / 40f * 3f), 1);
            }

            if (countdown == 0)
            {
                Booms[BoomCount + 1].Create(x, y, 7, 0.8f, 3, 1, 0, -1, 1, 1);
                Destroy(gameObject);
            }
        }
        else
        {
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
                Booms[BoomCount + 1].Create(x, y, 6.5f, 0.7f, 2f, 1, 0, -1, 1f, 1.05f);
                Booms[BoomCount + 1].Create(x, y, 4.1f, 0f, 0, 2, 30, playernumber, 1, 1);
                Destroy(gameObject);
            }
        }

        countdown--;
    }

    private void LateUpdate()
    {
        if (Pause) return;

        BombHit(-99);
    }

    public void ChangeVol(float newX, float newY, int pn, bool isbomb)
    {
        vy = newY;
        vx = newX;
        playernumber = pn;
        bomb = isbomb;
    }
}
