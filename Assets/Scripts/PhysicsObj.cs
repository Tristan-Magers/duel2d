using UnityEngine;
using System.Collections;

public class PhysicsObj : ArenaObj
{
    public float x, y, vx, vy, ObHt, ObWd, w = 1;
    public static float gravityorg = .035f;
    public float gravity = .035f;
    public static float friction = .85f;
    public bool grounded = false;
    public bool collision = false;
    public bool conPlat = false;
    public bool crouch = false;
    public bool platfall = false;

    public int invulFrames = 0;

    public float energy = 1000;

    public int freezeframe, attackstun, hitstun, hitby, genstun;
    public int[] immstun = new int[11];
    public float hitscale;

    int wallbounce = 0;

    void Start()
    {

    }

    void Update()
    {
        if ( Pause) return;

        Move();
    }

    public void BombHit(int imm)
    {
        for (int i = 0; i <=  BoomCount; i++)
        {
            if(Booms[i].imm != imm)
            {
                float rdist =  Booms[i].r + ((ObHt/2+ObWd/2)/2);
                float dist = Mathf.Pow(Mathf.Pow( Booms[i].y - y, 2) + Mathf.Pow( Booms[i].x - x, 2), 0.5f);
                if (dist <= rdist)
                {
                    float angle = Mathf.Atan2(y -  Booms[i].y, x -  Booms[i].x);
                    float ychange =  Booms[i].uprat * Mathf.Sin(angle) *  Booms[i].kb * (1 - ((dist * dist) / (rdist * rdist)));
                    float xchange =  Booms[i].drat * Mathf.Cos(angle) *  Booms[i].kb * (1 - ((dist * dist) / (rdist * rdist)));
                    Debug.Log("xchange:" + xchange + " ychange:" + ychange + " angle:" + angle);
                    vy = MomChange(vy, ychange * 1.3f / w);
                    vx = MomChange(vx, xchange * .8f / w);
                    if (invulFrames <= 0 && Booms[i].stun > 0) hitstun = (int) ((Mathf.Abs(( Booms[i].stun/2f) * (1 - ((dist * dist) / (rdist * rdist)))) + ( Booms[i].stun / 2f)) * Mathf.Pow(2, hitscale));

                    if (invulFrames <= 0 && Booms[i].t == 2) energy -= 1.3f * Mathf.Pow(1.5f, hitscale);
                }
            }          
        }
    }

    public bool HitBoxHit(int imm)
    {
        bool isHit = false;

        for (int i = 0; i <=  HitCount; i++)
        {
            if (Hits[i].type >= 99)
            {
                if (Hits[i].type == 99)
                {
                    vy = MomChange(vy, Hits[i].pow * Mathf.Sin(Hits[i].angle / 180 * Mathf.PI) / w);
                    vx = MomChange(vx, Hits[i].pow * Mathf.Cos(Hits[i].angle / 180 * Mathf.PI) / w);
                }
            }
            else
            {
                int immcheck;
                if (1 <= Hits[i].imm && 9 >= Hits[i].imm) immcheck = Hits[i].imm;
                else immcheck = 0;

                if (imm != Hits[i].imm && ((hitby != Hits[i].imm && Hits[i].imm > 0) || immstun[immcheck] <= 0) && BoxCol(Hits[i].x1, Hits[i].x2, Hits[i].y1, Hits[i].y2, x - (ObWd / 2), x + (ObWd / 2), y - (ObHt / 2), y + (ObHt / 2)))
                {
                    if (Hits[i].type == 1)
                    {
                        isHit = true;

                        vy = MomChange(vy / 2, Hits[i].pow * Mathf.Pow(1.3f, hitscale) * Mathf.Sin(Hits[i].angle / 180 * Mathf.PI) / w);
                        vx = MomChange(vx / 2, Hits[i].pow * Mathf.Pow(1.3f, hitscale) * Mathf.Cos(Hits[i].angle / 180 * Mathf.PI) / w);

                        hitstun = (int)Mathf.Ceil(Hits[i].stun * Mathf.Pow(1.5f, hitscale));
                        freezeframe = (int)Mathf.Ceil(Hits[i].freeze * Mathf.Pow(2, hitscale));
                        energy -= Hits[i].dam * Mathf.Pow(1.5f, hitscale);

                        hitby = Hits[i].imm;
                        immstun[immcheck] = Hits[i].immf;
                        attackstun = -1;

                        AudioSource audioData;
                        audioData = GetComponent<AudioSource>();
                        //audioData.pitch = (1 / Mathf.Pow(Global.Booms[i].r, 0.4f)) + 0.5f;
                        audioData.Play(0);
                    }
                    if (Hits[i].type == 2 && vx >= 0)
                    {
                        Debug.Log("Hit 2 at vx:" + vx);

                        isHit = true;

                        vy = MomChange(vy / 2, Hits[i].pow * Mathf.Pow(1.3f, hitscale) * Mathf.Sin(Hits[i].angle / 180 * Mathf.PI) / w);
                        vx = MomChange(vx / 2, Hits[i].pow * Mathf.Pow(1.3f, hitscale) * Mathf.Cos(Hits[i].angle / 180 * Mathf.PI) / w);

                        hitstun = (int)Mathf.Ceil(Hits[i].stun * Mathf.Pow(1.7f, hitscale));
                        freezeframe = (int)Mathf.Ceil(Hits[i].freeze * Mathf.Pow(2, hitscale));
                        energy -= Hits[i].dam * Mathf.Pow(1.5f, hitscale);

                        hitby = Hits[i].imm;
                        immstun[immcheck] = Hits[i].immf;
                        attackstun = -1;

                        AudioSource audioData;
                        audioData = GetComponent<AudioSource>();
                        //audioData.pitch = (1 / Mathf.Pow(Global.Booms[i].r, 0.4f)) + 0.5f;
                        audioData.Play(0);
                    }
                    if (Hits[i].type == 3 && vx <= 0)
                    {
                        Debug.Log("Hit 3 at vx:" + vx);

                        isHit = true;

                        vy = MomChange(vy / 2, Hits[i].pow * Mathf.Pow(1.3f, hitscale) * Mathf.Sin(Hits[i].angle / 180 * Mathf.PI) / w);
                        vx = MomChange(vx / 2, Hits[i].pow * Mathf.Pow(1.3f, hitscale) * Mathf.Cos(Hits[i].angle / 180 * Mathf.PI) / w);

                        hitstun = (int)Mathf.Ceil(Hits[i].stun * Mathf.Pow(2, hitscale));
                        freezeframe = (int)Mathf.Ceil(Hits[i].freeze * Mathf.Pow(2, hitscale));
                        energy -= Hits[i].dam * Mathf.Pow(1.5f, hitscale);

                        hitby = Hits[i].imm;
                        immstun[immcheck] = Hits[i].immf;
                        attackstun = -1;

                        AudioSource audioData;
                        audioData = GetComponent<AudioSource>();
                        //audioData.pitch = (1 / Mathf.Pow(Global.Booms[i].r, 0.4f)) + 0.5f;
                        audioData.Play(0);
                    }
                }
            }

            if (isHit) break;
        }

        return isHit;
    }

    public static bool BoxCol(float x1, float x2, float y1, float y2, float x3, float x4, float y3, float y4)
    {
        if (y1 >= y3 && y1 <= y4 && x1 <= x4 && x2 >= x3) return true;
        if (y2 >= y3 && y2 <= y4 && x1 <= x4 && x2 >= x3) return true;
        if (x1 >= x3 && x1 <= x4 && y1 <= y4 && y2 >= y3) return true;
        if (x2 >= x3 && x2 <= x4 && y1 <= y4 && y2 >= y3) return true;
        
        return false;
    }

    public static float MomChange(float start, float change)
    {
        change = (Mathf.Sign(start) * start * start) + (Mathf.Sign(change) * change * change);
        start = Mathf.Pow(Mathf.Abs(change), 0.5f) * Mathf.Sign(change);
        return start;
    }

    public void Move()
    {

        if (vy < .1)
        {
            XMove(true);
            YMove();
        }
        else
        {
            YMove();
            XMove(true);
        }

    }

    public void YMove()
    {
        y += vy;

        vy -= gravity;

        if (vy >= 2.9) vy = 2.9f;
        if (vy <= -2.9) vy = -2.9f;

        grounded = false;
        conPlat = false;

        if (onGround(ObHt, ObWd, x, y))
        {
            float platVol = 0;

            collision = true;
            grounded = true;
            
            if (!conPlat)
            {
                int yplace = (int)Mathf.Floor((-(y - (ObHt / 2))) / 3);
                y = -(yplace) * 3 + (ObHt / 2);
            }
            else
            {
                for (int i = 0; i <=  PlatCount; i++)
                {
                    if ( Plats[i].active == true && ( Plats[i].type == 0 || !platfall) && x + (ObWd / 2) >  Plats[i].x - ( Plats[i].length / 2) && x - (ObWd / 2) <  Plats[i].x + ( Plats[i].length / 2) && y - (ObHt / 2) <=  Plats[i].y + ( Plats[i].height / 2) && y - (ObHt / 2) - vy >=  Plats[i].y + ( Plats[i].height / 2))
                    {
                        if (platVol == 0) { }
                        else
                        {
                            platVol =  Plats[i].plat.GetComponent<RespawnPlat>().vy;
                        }

                        y =  Plats[i].y + ( Plats[i].height / 2) + (ObHt / 2);
                        break;
                    }
                }
            }

            if (hitstun > 0 && vy <= -.3f) vy *= -0.6f;
            else vy = 0;
        }

        if (!conPlat && ceiling(ObHt, ObWd, x, y))
        {
            collision = true;
            if (!conPlat)
            {
                int yplace = (int)Mathf.Floor((-(y - (ObHt / 2))) / 3);
                y = -(yplace) * 3 - (ObHt / 2);
            }
            else
            {
                for (int i = 0; i <=  PlatCount; i++)
                {
                    if ( Plats[i].active == true &&  Plats[i].type == 0 && x + (ObWd / 2) >  Plats[i].x - ( Plats[i].length / 2) && x - (ObWd / 2) <  Plats[i].x + ( Plats[i].length / 2) && y + (ObHt / 2) >=  Plats[i].y - ( Plats[i].height / 2) && y + (ObHt / 2) - vy <=  Plats[i].y - ( Plats[i].height / 2))
                    {
                        y =  Plats[i].y - ( Plats[i].height / 2) - (ObHt / 2) - 0.01f;
                        break;
                    }
                }
            }
            if (hitstun > 0 && vy >= .3f) vy *= -0.6f;
            else vy = 0;
        }

        if (System.Math.Abs(vy) <= 0.01) vy = 0;
    }


    public void XMove(bool usefriction)
    {
        if (vx >= 2.9) vx = 2.9f;
        if (vx <= -2.9) vx = -2.9f;

        conPlat = false;

        if (hitstun <= 0) wallbounce = 0;

        if (usefriction)
        {
            if (grounded)
            {
                vx *= friction;
            }
            else
            {
                vx *= .995f;
            }
        }

        if (System.Math.Abs(vx) <= 0.01) vx = 0;

        x += vx;

        if (checkRight(ObHt, ObWd, x, y))
        {
            collision = true;
            if (!conPlat)
            {
                int xplace = (int)Mathf.Floor((x + (ObWd / 2)) / 3);
                x = (xplace) * 3 - (ObWd / 2) - 0.01f;
            }
            else
            {
                for (int i = 0; i <=  PlatCount; i++)
                {
                    if ( Plats[i].active == true &&  Plats[i].type == 0 && y + (ObHt / 2) >  Plats[i].y - ( Plats[i].height / 2) && y - (ObHt / 2) <  Plats[i].y + ( Plats[i].height / 2) && x + (ObWd / 2) >=  Plats[i].x - ( Plats[i].length / 2) && x + (ObWd / 2) - vx <=  Plats[i].x - ( Plats[i].length / 2))
                    {
                        x =  Plats[i].x - ( Plats[i].length / 2) - (ObWd / 2) - 0.01f;
                        break;
                    }
                }
            }

            if (hitstun > 0)
            {
                if (wallbounce >= 3)
                {
                    vx *= -0.8f;
                }
                else
                {
                    vx *= .8f;
                    wallbounce++;
                }
            }
            else
            {
                vx *= .8f;
            }
        }

        if (checkLeft(ObHt, ObWd, x, y))
        {
            collision = true;
            if (!conPlat)
            {
                int xplace = (int)Mathf.Floor((x - (ObWd / 2)) / 3);
                x = (xplace + 1) * 3 + (ObWd / 2) + 0.01f;
            }
            else
            {
                for (int i = 0; i <=  PlatCount; i++)
                {
                    if ( Plats[i].active == true &&  Plats[i].type == 0 && y + (ObHt / 2) >  Plats[i].y - ( Plats[i].height / 2) && y - (ObHt / 2) <  Plats[i].y + ( Plats[i].height / 2) && x - (ObWd / 2) <=  Plats[i].x + ( Plats[i].length / 2) && x - (ObWd / 2) - vx >=  Plats[i].x + ( Plats[i].length / 2))
                    {
                        x =  Plats[i].x + ( Plats[i].length / 2) + (ObWd / 2) + 0.01f;
                        break;
                    }
                }
            }

            if (hitstun > 0)
            {
                if (wallbounce >= 3)
                {
                    vx *= -0.8f;
                }
                else
                {
                    vx *= .8f;
                    wallbounce++;
                }
            }
            else
            {
                vx *= .8f;
            }               
        }
    }

    public bool onGround(float ObHt, float ObWd, float x, float y)
    {      
        if (energy < -12) return false;

        for (int i = 0; i <=  PlatCount; i++)
        {
            if ( Plats[i].active == true && ( Plats[i].type == 0 || !platfall) && x + (ObWd / 2) >  Plats[i].x - ( Plats[i].length / 2) && x - (ObWd / 2) <  Plats[i].x + ( Plats[i].length / 2) && y - (ObHt / 2) <=  Plats[i].y + ( Plats[i].height / 2) && y - (ObHt / 2) - vy >=  Plats[i].y + ( Plats[i].height / 2))
            {
                conPlat = true;
                return true;
            }
        }

        //if(vy<0 && Mathf.Floor((-(y - (ObHt / 2))) / 3) != Mathf.Floor((-(y - vy - (ObHt / 2))) / 3))
        int xplace, yplace;

            for (float i = 0; i < ObWd; i += 2.9f)
            {
                xplace = (int)Mathf.Floor((x - (ObWd / 2) + i) / 3);
                yplace = (int)Mathf.Floor((-(y - (ObHt / 2))) / 3);

                if (checkBlock(xplace, yplace))
                {
                    return true;
                }
            }

            xplace = (int)Mathf.Floor((x + (ObWd / 2)) / 3);
            yplace = (int)Mathf.Floor((-(y - (ObHt / 2))) / 3);

            if (checkBlock(xplace, yplace))
            {
                return true;
            }

        return false;
    }

    public bool ceiling(float ObHt, float ObWd, float x, float y)
    {
        if (energy < -12) return false;

        for (int i = 0; i <=  PlatCount; i++)
        {
            if ( Plats[i].active == true &&  Plats[i].type == 0 && x + (ObWd / 2) >  Plats[i].x - ( Plats[i].length / 2) && x - (ObWd / 2) <  Plats[i].x + ( Plats[i].length / 2) && y + (ObHt / 2) >=  Plats[i].y - ( Plats[i].height / 2) && y + (ObHt / 2) - vy <=  Plats[i].y - ( Plats[i].height / 2))
            {
                conPlat = true;
                return true;
            }
        }

        //if (vy > 0 && Mathf.Floor((-(y + (ObHt / 2))) / 3) != Mathf.Floor((-(y - vy + (ObHt / 2))) / 3))
        int xplace, yplace;

            for (float i = 0; i < ObWd; i += 2.9f)
            {
                xplace = (int)Mathf.Floor((x - (ObWd / 2) + i) / 3);
                yplace = (int)Mathf.Floor((-(y + (ObHt / 2))) / 3);

                if (checkBlock(xplace, yplace))
                {
                    return true;
                }
            }

            xplace = (int)Mathf.Floor((x + (ObWd / 2)) / 3);
            yplace = (int)Mathf.Floor((-(y + (ObHt / 2))) / 3);

            if (checkBlock(xplace, yplace))
            {
                return true;
            }

        return false;
    }

    public bool checkRight(float ObHt, float ObWd, float x, float y)
    {
        if (energy < -12) return false;

        for (int i = 0; i <=  PlatCount; i++)
        {
            if ( Plats[i].active == true &&  Plats[i].type == 0 && y + (ObHt / 2) >  Plats[i].y - ( Plats[i].height / 2) && y - (ObHt / 2) <  Plats[i].y + ( Plats[i].height / 2) && x + (ObWd / 2) >=  Plats[i].x - ( Plats[i].length / 2) && x + (ObWd / 2) - vx <=  Plats[i].x - ( Plats[i].length / 2))
            {
                conPlat = true;
                return true;
            }
        }

        //if (vx > 0 && Mathf.Floor((x + (ObWd / 2)) / 3) != Mathf.Floor((x - vx + (ObWd / 2)) / 3))
        int xplace, yplace;

            for (float i = 0; i < ObHt - .02f; i += 2.9f)
            {
                xplace = (int)Mathf.Floor((x + (ObWd / 2)) / 3);
                yplace = (int)Mathf.Floor((-(y - (ObHt / 2) + i + 0.01f)) / 3);

                if (checkBlock(xplace, yplace))
                {
                    return true;
                }
            }

            xplace = (int)Mathf.Floor((x + (ObWd / 2)) / 3);
            yplace = (int)Mathf.Floor((-(y + (ObHt / 2) - 0.01f)) / 3);

            if (checkBlock(xplace, yplace))
            {
                return true;
            }
        
        return false;
    }

    public bool checkLeft(float ObHt, float ObWd, float x, float y)
    {
        if (energy < -12) return false;

        for (int i = 0; i <=  PlatCount; i++)
        {
            if ( Plats[i].active == true &&  Plats[i].type == 0 && y + (ObHt / 2) >  Plats[i].y - ( Plats[i].height / 2) && y - (ObHt / 2) <  Plats[i].y + ( Plats[i].height / 2) && x - (ObWd / 2) <=  Plats[i].x + ( Plats[i].length / 2) && x - (ObWd / 2) - vx >=  Plats[i].x + ( Plats[i].length / 2))
            {
                conPlat = true;
                return true;
            }
        }

        //if (vx < 0 && Mathf.Floor((x - (ObWd / 2)) / 3) != Mathf.Floor((x - vx - (ObWd / 2)) / 3))
        int xplace, yplace;

            for (float i = 0; i < ObHt - 0.02f; i += 2.9f)
            {
                xplace = (int)Mathf.Floor((x - (ObWd / 2)) / 3);
                yplace = (int)Mathf.Floor((-(y - (ObHt / 2) + i + 0.01f)) / 3);

                if (checkBlock(xplace, yplace))
                {
                    return true;
                }
            }

            xplace = (int)Mathf.Floor((x - (ObWd / 2)) / 3);
            yplace = (int)Mathf.Floor((-(y + (ObHt / 2) - 0.01f)) / 3);

            if (checkBlock(xplace, yplace))
            {
                return true;
            }
        
        return false;
    }

    public bool checkBlock(int x, int y)
    {
        if (x >= 0 && x <=  ArWd - 1 && y >= 0 && y <=  ArHt - 1)
        {
            if ( field[x, y] != null &&  field[x, y].GetComponent<TileBehave>().Collide) return true;
        }
        return false;
    }

    public void VolLimit()
    {
        if (vy > 2.9f) vy = 2.9f;
        if (vy < -2.9f) vy = -2.9f;

        if (vx > 2.9f) vx = 2.9f;
        if (vx < -2.9f) vx = -2.9f;
    }
}