using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaBomb : PhysicsObj
{

    int countdown = -1;
    private Vector3 PosSet;
    public Sprite Triggered;

    // Start is called before the first frame update
    void Start()
    {
        ObHt = 1.4f;
        ObWd = 1.4f;

        w = 0.85f;
  
        gravity = .01f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Pause) return;

        Vector3 p = transform.position;

        PosSet = p;

        y = p.y;
        x = p.x;

        Move();

        PosSet.x = x;
        PosSet.y = y;

        transform.position = PosSet;

        if (ResetCycle) Destroy(gameObject);

        if (countdown == 0)
        {
            Booms[BoomCount + 1].Create(x, y, 3.8f, 0.3f, 1, 2, 30, -101, 1, 1);
        }

        if (countdown == 0)
        {
            Booms[BoomCount + 1].Create(x, y, 7.5f, 1.5f, 4, 1, 0, -101, 1.2f, .8f);
            Destroy(gameObject);
        }

        countdown--;
    }

    private void LateUpdate()
    {
        if (Pause) return;

        BombHit(-101);
    }

    public void trigger()
    {
        if (countdown < 0)
        {
            countdown = 24;
            gameObject.GetComponent<SpriteRenderer>().sprite = Triggered;
        }
    }
}
