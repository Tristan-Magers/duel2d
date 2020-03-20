using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : CharacterControl
{
    public GameObject PowBombOrg;
    public GameObject BombOrg;
    public GameObject NSpecBombOrg;

    public GameObject MegaBombOrg;
    GameObject MyMegaBomb;
    bool CanMegaBomb = true;

    public GameObject SlimeOrg;
    GameObject Slime;
    int SlimeCool = 0;
    bool SlimeCanBoom = true;

    public Sprite SlimeNorm;
    public Sprite SlimeBoom;

    public int SlimeFrame = 0;
    public int SlimeAttack = 0;

    float BowAngle = 0;
    int BowChargeTime = -1;

    public Sprite sprite_stand;
    public Sprite sprite_stun;
    public Sprite sprite_jab;
    public Sprite sprite_upper;
    public Sprite sprite_dash;
    public Sprite sprite_nair;
    public Sprite sprite_jab_start;
    public Sprite sprite_upper_start;
    public Sprite sprite_dash_start;
    public Sprite sprite_nair_start;
    public Sprite sprite_selfd;
    public Sprite sprite_selfd_end;
    public Sprite sprite_fair;
    public Sprite sprite_fair_start;
    public Sprite sprite_fair_start2;
    public Sprite sprite_dair;
    public Sprite sprite_dair_start;
    public Sprite sprite_throw;
    public Sprite sprite_throw2;
    public Sprite sprite_uair;
    public Sprite sprite_uair_start;
    public Sprite sprite_crouch;
    public Sprite sprite_power1;
    public Sprite sprite_power2;
    public Sprite sprite_power3;
    public Sprite sprite_power4;
    public Sprite sprite_power5;
    public Sprite sprite_power6;
    public Sprite sprite_prejump;
    public Sprite sprite_stack1;
    public Sprite sprite_stack2;
    public Sprite sprite_stack3;
    public Sprite sprite_stack4;
    public Sprite sprite_stack5;
    public Sprite sprite_stack6;
    public Sprite sprite_stack7;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Soldier start");

        Slime = Instantiate(SlimeOrg, transform.position, transform.rotation);

        if (attackstun < 0 || hitstun > 0) Slime.GetComponent<SpriteRenderer>().sprite = SlimeNorm;

        StartCharacter();
    }

    // Update is called once per frame
    void Update()
    {
        if (Pause) return;

        UpdateCharacter();

        if (attackstun <= 0 && hitstun <= 0 && startFrames <= 0)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = sprite_stand;
            if (crouch) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_crouch;
            if (prejump > 0 || genstun > 0) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_prejump;
        }

        if (hitstun > 0) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_stun;

        if (triggerAttack) AttackTrigger();
        if (triggerSpecial) SpeciaTrigger();

        if (attackstun > 0 || hitstun > 0)
        {
            if (attackstun > 0 && hitstun <= 0) AttackControl();
        }
        else
        {
            Facing();
        }        

        SlimeMech();

        if (!CanMegaBomb) ManageMegaBomb();

        if (ResetCycle) Reset();
    }

    private void AttackTrigger()
    {
        SlimeFrame = 0;

        switch (attackType)
        {
            case -10:
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = sprite_dair_start;

                    break;
                }
            case 0:
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = sprite_stack1;

                    break;
                }
            case 1:
                {
                    if (rightCon > 0) faceright = true;
                    else faceright = false;

                    if (SlimeCool > 0 || energy <= 0) attackstun = 21;
                    attackLength = attackstun;

                    gameObject.GetComponent<SpriteRenderer>().sprite = sprite_jab_start;

                    break;
                }
            case 2:
                {
                    vx /= 1.5f;

                    if (faceright) vx = MomChange(vx, 0.2f);
                    else vx = MomChange(vx, -0.2f);


                    gameObject.GetComponent<SpriteRenderer>().sprite = sprite_dash_start;

                    break;
                }
            case 3:
                {
                    if (SlimeCool > 0 || energy <= 0) attackstun = 23;
                    attackLength = attackstun;

                    gameObject.GetComponent<SpriteRenderer>().sprite = sprite_upper_start;

                    break;
                }
            case 4:
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = sprite_power1;

                    break;
                }
            case 5:
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = sprite_nair_start;

                    landingLag = 8;

                    if (SlimeCool > 0 || energy <= 0)
                    {
                        attackstun = 25;
                        landingLag = 6;
                    }

                    attackLength = attackstun;

                    break;
                }
            case 6:
                {
                    if (rightCon > 0) faceright = true;
                    else faceright = false;

                    if (vy < 0) vy *= 0.4f;

                    if (faceright && vx < .07f) vx = 0f;
                    if (!faceright && vx > -.07f) vx = 0f;

                    if (faceright) vx = MomChange(vx, .07f);
                    else vx = MomChange(vx, -.07f);

                    if (SlimeCool > 0 || energy <= 0) attackstun = 32;
                    attackLength = attackstun;

                    gameObject.GetComponent<SpriteRenderer>().sprite = sprite_fair_start2;

                    landingLag = 10;

                    break;
                }
            case 7:
                {
                    if (vy < 0.024f) vy = 0.024f;
                    vy += .009f;
                    canDair = false;

                    gameObject.GetComponent<SpriteRenderer>().sprite = sprite_dair_start;

                    landingLag = 6;

                    break;
                }
            case 8:
                {
                    prejump = -1;

                    vy /= 3;
                    vy = MomChange(vy, .65f);
                    canUair = false;

                    gameObject.GetComponent<SpriteRenderer>().sprite = sprite_uair_start;

                    landingLag = 5;

                    break;
                }
        }

        triggerAttack = false;

        Facing();
    }

    private void SpeciaTrigger()
    {
        dash = 0;

        if (upCon > 0)
        {
            UpSpec();
            return;
        }
        if (downCon > 0 && CanMegaBomb && SlimeCool <= 0)
        {
            DownSpec();
            return;
        }
        if (SlimeCool <= 0 && (rightCon > 0 || leftCon > 0))
        {
            SideSpec();
            return;
        }

        NSpec();
    }

    private void AttackControl()
    {
        GeneralAttackControl();

        //side spec
        if (attackType == -2)
        {
            if (framein == 1) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_throw;
            if (framein == 5 || BowChargeTime >= 5) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_throw2;
            if (framein == 17) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_stand;
            Jump();
            if (jumpCon > 0 && jumpframes > 0)
            {
                vy += gravity / 2f;
                jumpframes--;
            }
            else
            {
                jumpframes = 0;
            }

            if (framein == 10)
            {
                float Power = 0;
                if (BowChargeTime > 6) Power = 0.55f + ((1 - Mathf.Pow(0.9f, BowChargeTime - 6)) * .58f);
                else Power = 0.55f;
                BowChargeTime = -1;
                SlimeCool = 190;
                GameObject NewBomb;
                NewBomb = Instantiate(BombOrg, transform.position, transform.rotation);
                NewBomb.GetComponent<BowBombScript>().ChangeVol(Power * Mathf.Cos(BowAngle / 180 * Mathf.PI), Power * Mathf.Sin(BowAngle / 180 * Mathf.PI), playernumber, true);
            }

            if (specialCon > 0 && BowChargeTime <= 30 && BowChargeTime >= 0 && bowBuff)
            {
                BowChargeTime++;
                attackstun++;
            }
            else
            {
                bowBuff = false;
            }
        }

        //neutral slime
        if (attackType == -3)
        {
            Debug.Log("framein: " + framein);

            if (framein == 1) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_throw;
            if (framein == 5) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_stand;
            Jump();
            if (jumpCon > 0 && jumpframes > 0)
            {
                vy += gravity / 2f;
                jumpframes--;
            }
            else
            {
                jumpframes = 0;
            }

            if (specialCon == 0) bowBuff = false;

            //because Duel TNT is basketball
            if (rightCon == 2) faceright = true;
            if (leftCon == 2) faceright = false;

            if (faceright) transform.rotation = Quaternion.Euler(0, 180, 0);
            else transform.rotation = Quaternion.Euler(0, 0, 0);

            if (framein == 5)
            {
                if (faceright)
                {
                    faceright = true;
                    BowAngle = 50;
                }
                else
                {
                    faceright = false;
                    BowAngle = 130;
                }

                float Power = 0;
                if (BowChargeTime > 6) Power = 0.21f + ((1 - Mathf.Pow(0.96f, BowChargeTime - 6)) * 1.11f);
                else Power = 0.21f;

                SlimeCool = 60;
                GameObject NewBomb;
                NewBomb = Instantiate(BombOrg, transform.position, transform.rotation);
                NewBomb.GetComponent<BowBombScript>().ChangeVol(Power * Mathf.Cos(BowAngle / 180 * Mathf.PI), Power * Mathf.Sin(BowAngle / 180 * Mathf.PI), playernumber, false);
                NewBomb.GetComponent<SpriteRenderer>().sprite = SlimeBoom;

                /*if (doubleBowShot)
                {
                    if (BowChargeTime > 3) BowChargeTime += 4;
                    else BowChargeTime = 6;

                    if (BowChargeTime > 3) Power = 0.24f + ((1 - Mathf.Pow(0.96f, BowChargeTime - 3)) * 1.11f);
                    else Power = 0.24f;

                    GameObject NewBomb2;
                    NewBomb2 = Instantiate(BombOrg, transform.position, transform.rotation);
                    NewBomb2.GetComponent<BowBombScript>().ChangeVol(Power * 1.05f * Mathf.Cos(BowAngle / 180 * Mathf.PI), Power * 1.05f * Mathf.Sin(BowAngle / 180 * Mathf.PI), playernumber, false);
                    NewBomb2.GetComponent<SpriteRenderer>().sprite = SlimeBoom;
                }*/

                BowChargeTime = -1;
            }

            if (specialCon > 0 && BowChargeTime <= 58 && BowChargeTime >= 0 && bowBuff)
            {
                BowChargeTime++;
                attackstun++;
            }
        }

        if (attackType == -4)
        {
            if (framein == 3)
            {
                MyMegaBomb = Instantiate(MegaBombOrg, new Vector3(transform.position.x, transform.position.y - 0.4f, transform.position.z), transform.rotation);
                CanMegaBomb = false;
                SlimeCool = 100;
            }
        }

        if (attackType == -1)
        {
            //vy = MomChange(vy, -0.12f);
            //vx /= 1.2f;
            if (framein > 3) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_selfd_end;
        }

        if (grounded && attackType >= 5)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = sprite_prejump;
            attackType = -99;
            if (attackstun > landingLag) genstun = landingLag;
            else genstun = attackstun;
            genstun += 4;
            attackstun = 0;
        }

        /*if (!grounded && (attackType == 0 || attackType == 2 || attackType == 3))
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = sprite_stand;
            attackType = -99;
            attackstun = 4;
        }*/

        if (attackType == 0)
        {
            if (framein == 2) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_stack2;
            if (framein == 4) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_stack3;
            if (framein == 6) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_stack4;
            if (framein == 8) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_stack5;
            if (framein == 10) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_stack6;
            if (framein == 12) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_stack7;
            if (framein == 14) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_stack1;
            if (framein == 16) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_stack2;
            if (framein == 18) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_stack3;
            if (framein == 20) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_stack4;
            if (framein == 22) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_stack5;
            if (framein == 24) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_stack6;
            if (framein == 26) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_stack7;
            if (framein == 28) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_stack1;
            if (framein == 31) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_stack2;
            if (framein == 34) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_stack3;
            if (framein == 36) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_stack4;
            if (framein == 41) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_stack3;
            if (framein == 44) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_stack2;
            if (framein == 46) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_stack1;

            if (framein == 26) Booms[BoomCount + 1].Create(x, y - 1.5f, 5.0f, 0.45f, 3.2f, 1, 0, 0, 1, 1);
        }
        if (attackType == 1)
        {
            if (framein == 3) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_jab;
            if (framein == 7) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_jab_start;
            if (framein == 10) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_stand;
        }
        if (attackType == 2)
        {
            //Dash ~ 1.5 units in and goes ~ 2.2 units for the hitbox
            //if (framein == 1) Debug.Log("x = " + x);
            //if (framein == 10) Debug.Log("x = " + x);
            //if (framein == 18) Debug.Log("x = " + x);
            if (framein == 10)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = sprite_dash;

                if (faceright) vx = MomChange(vx, 0.43f);
                else vx = MomChange(vx, -0.43f);

                if (faceright) Booms[BoomCount + 1].Create(x - 3.5f, y - 0.6f, 4.4f, 0.37f, 2, 1, 0, playernumber, 1, 1);
                else Booms[BoomCount + 1].Create(x + 3.5f, y - 0.6f, 4.4f, 0.37f, 2, 1, 0, playernumber, 1, 1);
            }
            if (framein == 18) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_dash_start;
        }
        if (attackType == 3)
        {
            if (framein == 6) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_upper;
            if (framein == 9) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_upper_start;
            if (framein == 14) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_stand;
        }
        if (attackType == 4)
        {
            if (framein >= 10 && framein <= 11)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = sprite_power2;
            }
            if (framein >= 18 && framein <= 21)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = sprite_power5;
            }
            if (framein == 12) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_power1;
            if (framein == 14) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_power3;
            if (framein == 16) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_power4;
            if (framein == 22) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_power4;
            if (framein == 24) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_power6;

            if (framein == 16)
            {
                GameObject NewBomb;
                NewBomb = Instantiate(PowBombOrg, transform.position, transform.rotation);
                if (faceright) NewBomb.GetComponent<PowShot>().ChangeVol(0.5f, 0.5f, playernumber);
                else NewBomb.GetComponent<PowShot>().ChangeVol(-0.5f, 0.5f, playernumber);
            }
        }

        //nair
        if (attackType == 5)
        {
            if (framein == 16) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_nair_start;
            if (framein == 21) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_stand;
            if (framein == 8) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_nair;
        }

        //fair
        if (attackType == 6)
        {
            //if (framein == 11) vy /= 1.4f;
            //if (framein == 21) vy /= 1.4f;
            //if (framein >= 11 && framein <= 21) vy /= 1.1f;

            if (framein == 5) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_fair_start;

            if (framein == 11) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_fair;

            if (framein == 17) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_fair_start;
            if (framein == 20) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_fair_start2;
            if (framein == 23) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_stand;
            //if (framein == 19) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_fair_start;

            //if (framein == 21) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_fair;
            //if (framein == 24) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_fair_start;
           // if (framein == 28) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_fair_start2;
            if (framein == 30) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_stand;
        }

        //dair
        if (attackType == 7)
        {
            if (framein == 16) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_dair_start;
            if (framein == 28) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_stand;
            if (framein == 7) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_dair;

            if (framein == 10) Booms[BoomCount + 1].Create(x, y - 1.1f, 4.3f, 0.35f, 2.7f, 1, 0, 0, 1, 1);
        }

        //uair
        if (attackType == 8)
        {
            if (framein == 2) Booms[BoomCount + 1].Create(x, y - 1.8f, 4.6f, 0.58f, 2.3f, 1, 0, playernumber, 1, 1.1f);

            if (framein == 18) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_uair_start;
            if (framein == 30) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_stand;
            if (framein == 4) gameObject.GetComponent<SpriteRenderer>().sprite = sprite_uair;
        }
    }

    private void SideSpec()
    {
        attackstun = 21;
        gameObject.GetComponent<SpriteRenderer>().sprite = sprite_jab_start;
        attackLength = attackstun;
        attackType = -2;
        BowChargeTime = 0;
        energy -= 2.2f;
        bowBuff = true;

        //BowChargeTime = 0;
        if (rightCon > 0)
        {
            faceright = true;
            BowAngle = 20;
        }
        else
        {
            faceright = false;
            BowAngle = 160;
        }

    }

    private void UpSpec()
    {
        prejump = -1;
        if (vy < 0)
        {
            vy /= 2.1f;
            vy = MomChange(vy, 0.22f);
        }
        vx /= 1.7f;

        speclag = 18;
        GameObject NewBomb;
        energy -= 3.3f;
        NewBomb = Instantiate(NSpecBombOrg, new Vector3(transform.position.x, transform.position.y - 0.95f, transform.position.z), transform.rotation);
        NewBomb.GetComponent<NSpecBomb>().SetImm(playernumber);
    }

    private void NSpec()
    {
        if (SlimeCool > 0)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = sprite_selfd;
            //vy = MomChange(vy, -0.12f);
            //vx /= 1.2f;
            speclag = 7;
            energy -= 1.4f;
            attackType = -1;
            attackstun = 10;
            attackLength = attackstun;
            //vy = MomChange(vy, 0.22f);
            Hits[HitCount + 1].Create(x - (ObWd / 2), x + (ObWd / 2), y - (ObHt / 2), y + (ObHt / 2), playernumber, 0.1f, 90, 17, 2.5f, 13, 1, 1);
            Booms[BoomCount + 1].Create(transform.position.x, transform.position.y - 0.7f, 3.4f, 0.34f, 1, 1, 0, playernumber, 1f, 1);
        }
        else
        {
            attackstun = 6;
            gameObject.GetComponent<SpriteRenderer>().sprite = sprite_jab_start;
            attackLength = attackstun;
            attackType = -3;
            BowChargeTime = 0;
            energy -= 1.9f;
            bowBuff = true;
            doubleBowShot = true;
        }
    }

    private void DownSpec()
    {
        attackstun = 2;
        attackType = -4;
        attackLength = 5;
        crouch = false;
        energy -= 3.5f;
    }

    private void SlimeMove(Vector3 Going, float Ydif, float Xdif, int Ratio)
    {
        Vector3 current = Slime.transform.position;
        Going.y += Ydif;
        Going.x += Xdif;
        Vector3 dif = Going - current;
        Slime.transform.position = current + (dif / Ratio);
    }

    private void SlimeMech()
    {
        framein = attackLength - attackstun;

        if (energy > 0)
        {
            if (attackstun <= 0) SlimeCanBoom = true;
            Slime.GetComponent<SpriteRenderer>().color = Color.white;
        }
        else
        {
            SlimeCanBoom = false;
            Slime.GetComponent<SpriteRenderer>().color = Color.grey;
        }

        if (SlimeCool <= 0)
        {
            if (BowChargeTime > 0)
            {
                if (faceright) Slime.GetComponent<SlimeScript>().SlimeMove(transform.position, 1.3f, 1.7f, 4);
                else Slime.GetComponent<SlimeScript>().SlimeMove(transform.position, 1.3f, -1.7f, 4);

                //Vector3 change = Bow.transform.position;
                //change.y += Mathf.Sin(BowAngle / 180 * Mathf.PI) * 2f;
                //change.x += Mathf.Cos(BowAngle / 180 * Mathf.PI) * 2f;
                //SlimeMove(change, 0, 0, 3);
            }
            else
            {
                if (SlimeAttack > 0 && SlimeFrame > 0 && SlimeFrame < 30)
                {
                    SlimeFrame++;
                }
                else
                {
                    SlimeAttack = 0;
                }

                if (SlimeAttack == 0 && attackstun <= 0 || (attackType < 0 && attackType >= -3))
                {
                    if (faceright) SlimeMove(transform.position, 1.7f, 1.7f, 5);
                    else SlimeMove(transform.position, 1.7f, -1.7f, 5);

                    Slime.GetComponent<SpriteRenderer>().sprite = SlimeNorm;
                }
                else
                {
                    if (attackType == -4)
                    {
                        SlimeMove(transform.position, 0f, -.7f, 3);
                    }
                    if (attackType == 0)
                    {
                        if (framein >= 0 && framein <= 13)
                        {
                            if (faceright) SlimeMove(transform.position, 1.5f, -1.8f, 9);
                            else SlimeMove(transform.position, 1.5f, 1.8f, 9);
                        }
                        if (framein >= 14 && framein <= 26)
                        {
                            if (faceright) SlimeMove(transform.position, 1.5f, 1.8f, 9);
                            else SlimeMove(transform.position, 1.5f, -1.8f, 9);
                        }
                        if (framein >= 27 && framein <= 39)
                        {
                            if (faceright) SlimeMove(transform.position, 1.5f, -1.8f, 9);
                            else SlimeMove(transform.position, 1.5f, 1.8f, 9);
                        }
                        if (framein >= 40)
                        {
                            if (faceright) SlimeMove(transform.position, 1.5f, 1.8f, 9);
                            else SlimeMove(transform.position, 1.5f, -1.8f, 9);
                        }
                    }
                    if (attackType == 1 || SlimeAttack == 1)
                    {
                        if (SlimeFrame == 0) SlimeAttack = attackType;
                        if (SlimeFrame == 0) SlimeFrame = 1;

                        if (SlimeFrame <= 22)
                        {
                            if (faceright) Slime.GetComponent<SlimeScript>().SlimeMove(transform.position, -0.9f, 3.4f, 8);
                            else Slime.GetComponent<SlimeScript>().SlimeMove(transform.position, -0.9f, -3.4f, 8);
                        }
                        else
                        {
                            if (faceright) Slime.GetComponent<SlimeScript>().SlimeMove(transform.position, 1.3f, 1.7f, 5);
                            else Slime.GetComponent<SlimeScript>().SlimeMove(transform.position, 1.3f, -1.7f, 5);
                        }

                        if (SlimeFrame == 19 && SlimeCanBoom)
                        {
                            if (faceright) Booms[BoomCount + 1].Create(Slime.transform.position.x, Slime.transform.position.y, 2.9f, 0.38f, 2.1f, 1, 0, playernumber, 1, 1);
                            else Booms[BoomCount + 1].Create(Slime.transform.position.x, Slime.transform.position.y, 2.9f, 0.38f, 2.1f, 1, 0, playernumber, 1, 1);
                        }

                        if (SlimeFrame == 17) Slime.GetComponent<SpriteRenderer>().sprite = SlimeBoom;
                        if (SlimeFrame == 22) Slime.GetComponent<SpriteRenderer>().sprite = SlimeNorm;
                    }
                    if (attackType == 2)
                    {
                        if (faceright) SlimeMove(transform.position, 1.3f, 1.7f, 5);
                        else SlimeMove(transform.position, 1.3f, -1.7f, 5);
                    }
                    if (attackType == 3 || SlimeAttack == 3)
                    {
                        if (SlimeFrame == 0) SlimeAttack = attackType;
                        if (SlimeFrame == 0) SlimeFrame = 1;

                        if (SlimeFrame <= 21)
                        {
                            if (faceright) Slime.GetComponent<SlimeScript>().SlimeMove(transform.position, 3.5f, 2.4f, 13);
                            else Slime.GetComponent<SlimeScript>().SlimeMove(transform.position, 3.5f, -2.4f, 13);
                        }
                        else
                        {
                            if (faceright) Slime.GetComponent<SlimeScript>().SlimeMove(transform.position, 1.3f, 1.7f, 5);
                            else Slime.GetComponent<SlimeScript>().SlimeMove(transform.position, 1.3f, -1.7f, 5);
                        }

                        if (SlimeFrame == 19 && SlimeCanBoom)
                        {
                            if (faceright) Booms[BoomCount + 1].Create(Slime.transform.position.x, Slime.transform.position.y, 3.5f, 0.4f, 2.4f, 1, 0, playernumber, 1, 1);
                            else Booms[BoomCount + 1].Create(Slime.transform.position.x, Slime.transform.position.y, 3.5f, 0.4f, 2.4f, 1, 0, playernumber, 1, 1);
                        }

                        if (SlimeFrame == 17) Slime.GetComponent<SpriteRenderer>().sprite = SlimeBoom;
                        if (SlimeFrame == 21) Slime.GetComponent<SpriteRenderer>().sprite = SlimeNorm;
                    }
                    if (attackType == 5 || SlimeAttack == 5)
                    {
                        if (SlimeFrame == 0) SlimeAttack = attackType;
                        if (SlimeFrame == 0) SlimeFrame = 1;

                        if (SlimeFrame <= 14)
                        {
                            if (faceright) Slime.GetComponent<SlimeScript>().SlimeMove(transform.position, -4.8f, 3.6f, 3);
                            else Slime.GetComponent<SlimeScript>().SlimeMove(transform.position, -4.8f, -3.6f, 3);
                        }
                        else
                        {
                            if (faceright) Slime.GetComponent<SlimeScript>().SlimeMove(transform.position, 1.5f, 1.9f, 5);
                            else Slime.GetComponent<SlimeScript>().SlimeMove(transform.position, 1.5f, -1.9f, 5);
                        }

                        if (SlimeFrame == 9 && SlimeCanBoom)
                        {
                            if (faceright) Slime.GetComponent<SlimeScript>().SlimeMove(transform.position, -4.8f, 3.6f, 4);
                            else Slime.GetComponent<SlimeScript>().SlimeMove(transform.position, -4.8f, -3.6f, 4);

                            Booms[BoomCount + 1].Create(Slime.transform.position.x, Slime.transform.position.y, 3.6f, 0.35f, 1.3f, 1, 0, playernumber, 1, 1);
                        }

                        if (SlimeFrame == 7) Slime.GetComponent<SpriteRenderer>().sprite = SlimeBoom;
                        if (SlimeFrame == 15) Slime.GetComponent<SpriteRenderer>().sprite = SlimeNorm;
                    }

                    if (attackType == 6 || SlimeAttack == 6)
                    {
                        if (SlimeFrame == 0) SlimeAttack = attackType;
                        if (SlimeFrame == 0) SlimeFrame = 1;

                        if (SlimeFrame <= 11)
                        {
                            if (faceright) Slime.GetComponent<SlimeScript>().SlimeMove(transform.position, -3.2f, 6.5f, 3);
                            else Slime.GetComponent<SlimeScript>().SlimeMove(transform.position, -3.2f, -6.5f, 3);
                        }
                        else
                        {
                            if (faceright) Slime.GetComponent<SlimeScript>().SlimeMove(transform.position, 1.5f, 1.9f, 5);
                            else Slime.GetComponent<SlimeScript>().SlimeMove(transform.position, 1.5f, -1.9f, 5);
                        }

                        if (SlimeFrame == 9 && SlimeCanBoom)
                        {
                            if (faceright) Slime.GetComponent<SlimeScript>().SlimeMove(transform.position, -3.2f, 6.5f, 4);
                            else Slime.GetComponent<SlimeScript>().SlimeMove(transform.position, -3.2f, -6.5f, 4);

                            if (faceright) Booms[BoomCount + 1].Create(Slime.transform.position.x, Slime.transform.position.y, 4.6f, 0.51f, 1.9f, 1, 0, playernumber, 1.2f, .9f);
                            else Booms[BoomCount + 1].Create(Slime.transform.position.x, Slime.transform.position.y, 4.6f, 0.51f, 1.9f, 1, 0, playernumber, 1.2f, .9f);
                        }

                        if (SlimeFrame == 8) Slime.GetComponent<SpriteRenderer>().sprite = SlimeBoom;
                        if (SlimeFrame == 12) Slime.GetComponent<SpriteRenderer>().sprite = SlimeNorm;
                    }

                    if (attackType == 7)
                    {
                        if (faceright) SlimeMove(transform.position, 1.5f, 1.9f, 5);
                        else SlimeMove(transform.position, 1.5f, -1.9f, 5);
                    }

                    if (attackType == 8)
                    {
                        if (faceright) SlimeMove(transform.position, 1.8f, -2.3f, 5);
                        else SlimeMove(transform.position, 1.8f, 2.3f, 5);
                    }

                    if (attackType == 4)
                    {
                        if (faceright) SlimeMove(transform.position, 1.5f, -1.9f, 4);
                        else SlimeMove(transform.position, 1.5f, 1.9f, 4);
                    }
                }
            }
        }
        else
        {
            SlimeCool--;

            if (SlimeCool < 9)
            {
                if (faceright) SlimeMove(transform.position, 1.3f, 1.7f, 5);
                else SlimeMove(transform.position, 1.3f, -1.7f, 5);
            }
            else
            {
                SlimeMove(transform.position, 60, 0, 1);
            }
        }
    }

    private void ManageMegaBomb()
    {
        if (MyMegaBomb == null)
        {
            CanMegaBomb = true;
            return;
        }

        SlimeCool = 125;

        float distance = Mathf.Pow(Mathf.Pow(x - MyMegaBomb.transform.position.x, 2) + Mathf.Pow(y - MyMegaBomb.transform.position.y, 2), 0.5f);

        if (distance > 3.7f) MyMegaBomb.GetComponent<MegaBomb>().trigger();
    }

    private void Reset()
    {
        GeneralReset();

        SlimeCool = 0;
        BowChargeTime = -2;

        CanMegaBomb = true;
    }
}
