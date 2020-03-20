using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBehave : ArenaObj
{
    public float xpos, ypos;
    public int Health, x, y, type, state, countdown = -1, platnumber;
    public bool Collide = true;

    public Material sprite_hard_12;
    public Material sprite_hard_11;
    public Material sprite_hard_10;
    public Material sprite_hard_9;
    public Material sprite_hard_8;
    public Material sprite_hard_7;
    public Material sprite_hard_6;
    public Material sprite_hard_5;
    public Material sprite_hard_4;
    public Material sprite_hard_3;
    public Material sprite_hard_2;
    public Material sprite_hard_1;

    public Material sprite_block_5;
    public Material sprite_block_4;
    public Material sprite_block_3;
    public Material sprite_block_2;
    public Material sprite_block_1;

    public Material sprite_plat_3;
    public Material sprite_plat_2;
    public Material sprite_plat_1;
    public Material sprite_plat_0;

    // Start is called before the first frame update
    void Start()
    {
        if (type == 0) Health = 5;
        if (type == 1)
        {
            Health = 3;
            platnumber = PlatCount + 1;
            Plats[PlatCount + 1].Create(PlatObj, xpos, ypos + 7.25f + (state *3), 3, .5f, -1 , 1);
        }
        if (type == 2) Health = 12;

        gameObject.GetComponent<MeshRenderer>().enabled = true;
        gameObject.GetComponent<MeshRenderer>().material = sprite_block_5;

        transform.rotation = Quaternion.Euler(new Vector3(0, -180, 0));

        //gameObject.GetComponent<MeshRenderer>().material.renderQueue = -1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LateUpdate()
    {
        if (Pause) return;

        Health -= changeHealth[x, y];
        changeHealth[x, y] = 0;

        if (type == 0) NormBlockManage();
        if (type == 1) PlatBlockManage();
        if (type == 2) HardBlockManage();
    }

    private void NormBlockManage()
    {
        if (Health == 5) gameObject.GetComponent<MeshRenderer>().material = sprite_block_5;

        if (Health == 4) gameObject.GetComponent<MeshRenderer>().material = sprite_block_4;

        if (Health == 3) gameObject.GetComponent<MeshRenderer>().material = sprite_block_3;

        if (Health == 2) gameObject.GetComponent<MeshRenderer>().material = sprite_block_2;

        if (Health == 1) gameObject.GetComponent<MeshRenderer>().material = sprite_block_1;

        if (Health <= 0)
        {
            Collide = false;
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            //Destroy(gameObject);
        }
    }

    private void HardBlockManage()
    {
        if (Health == 12) gameObject.GetComponent<MeshRenderer>().material = sprite_hard_12;

        if (Health == 11) gameObject.GetComponent<MeshRenderer>().material = sprite_hard_11;

        if (Health == 10) gameObject.GetComponent<MeshRenderer>().material = sprite_hard_10;

        if (Health == 9) gameObject.GetComponent<MeshRenderer>().material = sprite_hard_9;

        if (Health == 8) gameObject.GetComponent<MeshRenderer>().material = sprite_hard_8;

        if (Health == 7) gameObject.GetComponent<MeshRenderer>().material = sprite_hard_7;
    
        if (Health == 6) gameObject.GetComponent<MeshRenderer>().material = sprite_hard_6;

        if (Health == 5) gameObject.GetComponent<MeshRenderer>().material = sprite_hard_5;

        if (Health == 4) gameObject.GetComponent<MeshRenderer>().material = sprite_hard_4;

        if (Health == 3) gameObject.GetComponent<MeshRenderer>().material = sprite_hard_3;

        if (Health == 2) gameObject.GetComponent<MeshRenderer>().material = sprite_hard_2;

        if (Health == 1) gameObject.GetComponent<MeshRenderer>().material = sprite_hard_1;

        if (Health <= 0)
        {
            Collide = false;
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            //Destroy(gameObject);
        }
    }

    private void PlatBlockManage()
    {
        countdown--;

        if (Health == 3)
        {
            gameObject.GetComponent<MeshRenderer>().material = sprite_plat_3;
        }

        if (Health == 2)
        {
            gameObject.GetComponent<MeshRenderer>().material = sprite_plat_2;
        }

        if (Health == 1)
        {
            gameObject.GetComponent<MeshRenderer>().material = sprite_plat_1;
        }

        if (Health <= 0 && countdown < 0)
        {
            gameObject.GetComponent<MeshRenderer>().material = sprite_plat_0;

            countdown = 100;
        }

        if (countdown > 0 && state > -2)
        {
            Plats[platnumber].ChangePos(0, -.03f);
        }

        if (countdown == 0)
        {
            state--;
            
            if (state == -2)
            {
                Plats[platnumber].SetTimer(0);
            }
            if (state == -3)
            {
                Collide = false;
                gameObject.GetComponent<MeshRenderer>().enabled = false;
            }
            else
            {
                Health = 3;
            }
        }
    }

}
