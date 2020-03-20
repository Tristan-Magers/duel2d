using UnityEngine;
using UnityEngine.UI;

public class CharacterControl : PhysicsObj
{
    public static int player1Controller = -1, player2Controller = -1;

    public int platFallCounter;

    public int character = 1;

    public bool AI;

    public static int controlType;

    public int lives = 3;
    public int startFrames, landingLag;

    string up;
    string down;
    string right;
    string left;
    string jump;
    string attack;
    string special;

    public int upCon, downCon, leftCon, rightCon, specialCon, jumpCon, attackCon;

    public bool triggerAttack, triggerSpecial;

    private Vector3 PosSet;

    public int test;
    public int jumpframes;

    readonly private int launchrel = 14;

    float launch;

    public float gaccdef = 0.12f;
    public float gmaxdef = 0.3f;
    public float airaccdef = 0.02f;
    public float airmaxdef = 0.24f;

    float gacc, gmax, airacc, airmax;

    public int djump = 0;

    public float startjumpx = 0;

    public int airframes = 0;

    public bool buffattack = true, buffjump = true, buffspecial = true, canDair = true, canUair = true;

    public GameObject BarOrg;
    public GameObject ArrowOrg;

    RectTransform BarBackTrans, PBarTrans, LivesTrans;

    public GameObject Bow;
    public GameObject PBar;
    public GameObject BarBack;
    public GameObject Lives;
    public GameObject Arrow;

    public int playernumber;

    public bool faceright = false;

    public Sprite Arrow1;
    public Sprite Arrow2;

    public int attackType = -1;
    public int attackLength, framein;

    public int dash = 0;
    public int prejump =-1;

    public int speclag;

    public bool bowBuff, doubleBowShot;

    public AudioSource hit_audio;
    public AudioClip hit_sound;
    public AudioSource jump_audio;
    public AudioClip jump_sound;

    // Start is called before the first frame update

    public void StartCharacter()
    {
        hit_audio.clip = hit_sound;
        jump_audio.clip = jump_sound;

        if (playernumber == 1) Player1();
        if (playernumber == 2) Player2();

        launch = gravity * launchrel;

        gacc = gaccdef;
        gmax = gmaxdef;
        airacc = airaccdef;
        airmax = airmaxdef;

        jumpframes = 0;

        ObHt = 2.5f;
        ObWd = 2;

        //Bow = Instantiate(BarOrg, transform.position, transform.rotation);
        Arrow = Instantiate(ArrowOrg, transform.position, transform.rotation);

        if (playernumber == 1) Arrow.GetComponent<SpriteRenderer>().sprite = Arrow1;
        if (playernumber == 2) Arrow.GetComponent<SpriteRenderer>().sprite = Arrow2;

        GeneralReset();

        if (immstun.Length == 0) immstun = new int[11];
    }

    // Update is called once per frame
    public void UpdateCharacter()
    {
        if (immstun.Length == 0) immstun = new int[11];

        if (Pause) return;

        LivesTrack[playernumber - 1] = lives;

        InputControl();

        PlatFallManage();

        triggerAttack = false;
        triggerSpecial = false;
        hitscale = 0;

        Vector3 p = transform.position;

        PosSet = p;

        y = p.y;
        x = p.x;

        PosSet.z = -playernumber/60f;
        if (attackstun > 0) PosSet.z -= .1f;
        if (hitstun > 0) PosSet.z += .1f;

        if (specialCon == 0) buffspecial = true;
        if (attackCon == 0) buffattack = true;
        if (jumpCon == 0) buffjump = true;

        if (attackstun <= 0 && hitstun <= 0 && startFrames <= 0)
        {
            if (vy < .1)
            {
                XCharMove();
                YCharMove();
            }
            else
            {
                YCharMove();
                XCharMove();
            }

            if (grounded && (downCon == 2 || (downCon > 0 && (attackstun == 0 || hitstun == 0))))
            {
                Debug.Log("Crouch");
                crouch = true;
            }
            else
            {
                if (grounded && downCon > 0 && crouch)
                {
                    crouch = true;
                }
                else
                {
                    if (grounded && downCon == 0 && crouch)
                    {
                        if (crouch) genstun = 6;
                        crouch = false;
                    }
                    else
                    {
                        crouch = false;
                    }
                }
            }

            if (genstun <= 0 && prejump <= 0) Attacking();

            if ((specialCon == 2 || (specialCon > 0 && buffspecial && (speclag == 0 || hitstun == 0 || attackstun == 0))) && speclag <= 0 && energy > 0)
            {
                buffspecial = false;
                Special();
            }

            Facing();
        }
        else
        {
            crouch = false;

            if (hitstun > 0)
            {
                prejump = -1;
                attackType = -99;
                attackstun = -2;
            }

            if (rightCon > 0) vx = MomChange(vx, 0.032f);
            if (leftCon > 0) vx = MomChange(vx, -0.032f);

            if (freezeframe <= 0) Move();
        }

        ReduceLags();

        if (hitstun <= 0) hitby = -1;

        if (y > 18)
        {
            CamAveAdd(x, 18);
        }
        else
        {
            if (y < -12)
            {
                CamAveAdd(x, -12);
            }
            else
            {
                CamAveAdd(x, y);
            }
        }


        if (x + ObWd/2 > (ArWd * 3 / 2) + (11 + (ArWd * 3 / 2))) x = (ArWd * 3 / 2) + (11 + (ArWd * 3 / 2)) - ObWd/2;
        if (x - ObWd/2 < (ArWd * 3 / 2) - (11 + (ArWd * 3 / 2))) x = (ArWd * 3 / 2) - (11 + (ArWd * 3 / 2)) + ObWd/2;

        if (y < -30 || y > 28)
        {
            LoseLife();
        }

        PosSet.x = x;
        PosSet.y = y;
        transform.position = PosSet;

        //Debug remove block
        if(Input.GetKeyDown("k"))
        {
            int xplace = (int)Mathf.Floor((x) / 3);
            int yplace = (int)Mathf.Floor((-y) / 3);
            yplace += 1;
            Destroy(field[xplace,yplace]);
            Debug.Log(field[xplace, yplace]);
        }

        Arrow.transform.position = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
    }

    private void LateUpdate()
    {
        if (Pause)
        {
            EnergyBar();
            return;
        }

        Vector3 p = transform.position;

        PosSet = p;

        float y = p.y;
        float x = p.x;

        if (attackType == 4 && attackstun>0) hitscale += 0.8f;
        if (crouch) hitscale -= 0.8f;
        if (energy < 0) hitscale += 1;

        if (invulFrames <= 0)
        {
            if (HitBoxHit(playernumber))
            {
                jump_audio.Stop();
                hit_audio.Play(0);
            }
        }
        else invulFrames--;

        BombHit(playernumber);

        EnergyBar();
    }

    private void YCharMove()
    {
        bool startgrounded = grounded;

        if (vy > launch) jumpframes = 0;

        //hold jump
        if (jumpCon > 0 && jumpframes > 0)
        {
            vy += gravity/2f;
            jumpframes--;
        }
        else
        {
            //vy -= gravity;
            jumpframes = 0;
        }

        float prevy = vy;

        YMove();

        if (grounded)
        {
            canDair = true;
            canUair = true;

            if (!startgrounded)
            {
                float jumpdistance = startjumpx - x;
                Debug.Log("snap with frames:" + airframes + " and distance:" + jumpdistance);

                if (Mathf.Abs(prevy) > .1f)
                {
                    genstun = 4;
                    attackType = -10;
                }
            }

            grounded = true;
            // double jump trigger, currently unused
            djump = 1;
        }
        else
        {
            grounded = false;
            airframes++;
        }

        //Start jump
        Jump();

        //return jump height
        //if (y > 0 && vy >= -gravity && vy < 0) Debug.Log(y + " and " + vy);

        if (System.Math.Abs(vy) <= 0.01) vy = 0;
    }

    private void XCharMove()
    {
        if (grounded)
        {
            if((rightCon > 0|| leftCon > 0) && Mathf.Abs(vx)<=gmax) vx *= Mathf.Pow(friction,0.5f);
            else vx *= friction;

            if (genstun <= 0 && (specialCon == 0) && rightCon > 0 && Mathf.Abs(vx) <= gmax*1.1)
            {
                dash++;
                //vx += gacc;
                //vx += gacc * (1-(vx/ gmax));
                float temp = (Mathf.Sign(vx) * vx * vx) + (gacc * gacc);
                vx = Mathf.Pow(Mathf.Abs(temp), .5f) * Mathf.Sign(temp);
                if (vx > gmax) vx = gmax;
                if (downCon > 0 && vx > gmax/2 && vx < gmax) vx = gmax/2;
            }
            if (genstun <= 0 && (specialCon == 0) && leftCon > 0 && Mathf.Abs(vx) <= gmax*1.1)
            {
                dash++;
                //vx -= gacc;
                //vx -= gacc * (1-(-vx/gmax));
                float temp = (Mathf.Sign(vx) * vx * vx) - (gacc * gacc);
                vx = Mathf.Pow(Mathf.Abs(temp),.5f) * Mathf.Sign(temp);
                if (vx < -gmax) vx = -gmax;
                if (downCon > 0 && vx < -gmax/2 && vx > -gmax) vx = -gmax/2;
            }
            if (downCon > 0)
            {
                vx *= .85f;
                dash = 0;
            }
            if (leftCon == 0 && rightCon == 0) dash--;
            if (dash > 7) dash = 7;
            if (dash < 0) dash = 0;
        }
        else
        {
            dash = 0;
            if (leftCon == 0 && rightCon == 0) vx *= .98f;
            if (rightCon > 0 && vx < airmax)
            {
                vx += airacc;
                if (vx > airmax) vx = airmax;
            }
            if (leftCon > 0 && vx > -airmax)
            {
                vx -= airacc;
                if (vx < -airmax) vx = -airmax;
            }
            if (System.Math.Abs(vx) > airmax)
            {
                if (vx > 0)
                {
                    if (rightCon > 0)
                    {
                        vx *= .98f;
                    }
                    else
                    {
                        vx *= .99f;
                    }
                }
                else
                {
                    if (rightCon > 0)
                    {
                        vx *= .98f;
                    }
                    else
                    {
                        vx *= .99f;
                    }
                }
            }
        }

        collision = false;

        XMove(false);

        if (collision) dash = 0;
    }

    public void Jump()
    {
        if (!crouch && (genstun <= 0 && jumpCon == 2) || (jumpCon > 0 && buffjump && (hitstun == 0 || attackstun == 0 || genstun == 0)))
        {
            buffjump = false;
            if (grounded)
            {
                if (prejump < 0)
                {
                    jump_audio.Play(0);
                    prejump = 3;
                }
            }
            else
            {
                if (djump >= 1 && vy < launch * 0.9f)
                {
                    jump_audio.Play(0);
                    djump--;
                    attackstun = 20;
                    if (invulFrames < 20) invulFrames = 20;
                    attackType = -10;
                    triggerAttack = true;
                    if (vy < 0) vy = 0;
                    vy = MomChange(vy, launch * 0.93f);
                    vx *= 0.5f;
                }
            }
        }

        if (prejump == 0)
        {
            Debug.Log("Jump");
            vy = launch;
            jumpframes = launchrel;
            startjumpx = x;
            grounded = false;
            airframes = 0;
        }
    }

    private void Attacking()
    {
        if (attackCon == 2 || (attackCon > 0 && buffattack && (prejump == 0 || attackstun == 0 || hitstun == 0 || genstun == 0)))
        {
            buffattack = false;
            if (grounded)
            {
                if (upCon > 0 && dash <= 3 && !crouch)
                {
                    Debug.Log("Upper");
                    attackType = 3;
                }
                else
                {
                    if (dash >= 5)
                    {
                        Debug.Log("Dash");
                        attackType = 2;
                    }
                    else
                    {
                        if (crouch)
                        {
                            Debug.Log("Power");
                            attackType = 4;
                        }
                        else
                        {
                            if (dash > 0 && (rightCon > 0 || leftCon > 0))
                            {
                                Debug.Log("Stab");
                                attackType = 1;
                            }
                            else
                            {
                                Debug.Log("Normal");
                                attackType = 0;
                            }
                        }                    
                    }
                }
            }
            else
            {
                if (upCon > 0 && canUair)
                {
                    Debug.Log("Uair");
                    attackType = 8;
                }
                else
                {
                    if (downCon > 0 && canDair)
                    {
                        Debug.Log("Dair");
                        attackType = 7;
                    }
                    else
                    {
                        if (rightCon > 0 || leftCon > 0)
                        {
                            Debug.Log("Fair");
                            attackType = 6;
                        }
                        else
                        {
                            Debug.Log("Nair");
                            attackType = 5;
                        }                       
                    }                       
                }               
            }

            dash = 0;

            attackstun = AttackData[character, attackType, 0];
            attackLength = attackstun;

            triggerAttack = true;
        }
    }

    private void Special()
    {
        triggerSpecial = true;
    }

    private void Player1()
    {
        if (controlType == 0)
        {
            up = "up";
            down = "down";
            right = "right";
            left = "left";
            jump = "[1]";
            attack = "[2]";
            special = "[3]";
        }

        if (controlType == 1)
        {
            up = "up";
            down = "down";
            right = "right";
            left = "left";
            jump = "p";
            attack = "o";
            special = "i";
        }
    }

    private void Player2()
    {
        if (controlType == 0)
        {
            up = "w";
            down = "s";
            right = "d";
            left = "a";
            jump = "t";
            attack = "y";
            special = "u";
        }

        if (controlType == 1)
        {
            up = "w";
            down = "s";
            right = "d";
            left = "a";
            jump = "c";
            attack = "v";
            special = "b";
        }
    }

    public void GeneralReset()
    {
        vx = 0;
        vy = 0;

        invulFrames = -2;

        ResetTimers();

        rightCon = 0;
        leftCon = 0;
        upCon = 0;
        downCon = 0;
        jumpCon = 0;
        attackCon = 0;
        specialCon = 0;

        Vector3 ResetPos = transform.position;
        ResetPos.y = ObHt/2;

        if (playernumber == 1) ResetPos.x = (ArWd * 3 / 2) + 3.8f;
        if (playernumber == 2) ResetPos.x = (ArWd * 3 / 2) - 3.8f;

        if (playernumber == 2) faceright = true;
        if (playernumber == 1) faceright = false;

        transform.position = ResetPos;

        energy = 12;
        lives = 3;
    }

    public void Facing()
    {
        if (vx > 0 && rightCon > 0 && leftCon == 0)
        {
            if (!faceright) dash = 0;
            faceright = true;
        }
        if (vx < 0 && leftCon > 0 && rightCon == 0)
        {
            if (faceright) dash = 0;
            faceright = false;
        }

        if (vx > .1 && rightCon > 0)
        {
            if (!faceright) dash = 0;
            faceright = true;
        }
        if (vx < -.1 && leftCon > 0)
        {
            if (faceright) dash = 0;
            faceright = false;
        }

        if (faceright) transform.rotation = Quaternion.Euler(0, 180, 0);
        else transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    private void EnergyBar()
    {
        if (PBar == null) ConnectUI();
        if (PBar == null) return;

        Energies[playernumber-1] = energy;

        //EBarBack.transform.position = Camera.Cam.transform.position;
        //EBar.transform.position = Camera.Cam.transform.position;

        if (!Pause)
        {
            if (energy > -12 && hitstun <= 0)
            {
                if (energy > 0)
                {
                    energy += 1f / 80f;
                }
                else
                {
                    energy += 1f / 40f;
                }
                if (crouch) energy += 1f / 80f;
            }
            if (energy > 12) energy = 12;
            if (energy <= -12) energy = -12.01f;        }

        float UIScale = Screen.height / 400f;

        PBarTrans.localPosition = new Vector3((((Mathf.Abs(energy) / 6) - 2) * 50 * UIScale) + playernumber * (-320 * UIScale) + 480 * UIScale, Screen.height / 2 - 40  * UIScale, 0);
        BarBackTrans.localPosition = new Vector3(playernumber * (-320 *UIScale) + 480 * UIScale, Screen.height/2 - 40 * UIScale, 0);
        PBarTrans.localScale = new Vector3(Mathf.Abs(energy) / 6, .45f, 1);
        BarBackTrans.localScale = new Vector3(2.15f, .6f, 1);

        LivesTrans.localPosition = new Vector3(playernumber * (-320 * UIScale) + 480 * UIScale - (50 * UIScale), Screen.height / 2 - 66 * UIScale, 0);
        LivesTrans.localScale = new Vector3((13f/5f), 1f, 1);

        PBarTrans.localScale *= UIScale;
        BarBackTrans.localScale *= UIScale;
        LivesTrans.localScale *= UIScale / 3.5f;

        if (energy >= 0)
        {
            this.GetComponent<SpriteRenderer>().color = Color.white;
            if (invulFrames > 1) this.GetComponent<SpriteRenderer>().color = Color.yellow;
            PBar.GetComponent<Image>().color = new Color(0f / 255f, 235f / 255f, 0f / 255f);
        }
        else
        {
            this.GetComponent<SpriteRenderer>().color = Color.gray;
            PBar.GetComponent<Image>().color = Color.red;
        }
     
        if (energy == 12) PBar.GetComponent<Image>().color = new Color(50f / 255f, 255f / 255f, 50f / 255f);
        if (energy > -12 && hitstun > 0)
        {
            if (energy > 0) PBar.GetComponent<Image>().color = Color.yellow;
            else PBar.GetComponent<Image>().color = new Color(255f / 255f, 170f / 255f, 170f / 255f);
        }
        if (energy <= -12) PBar.GetComponent<Image>().color = new Color(159f / 255f, 0f, 0f);

        if (playernumber == 1) BarBack.GetComponent<Image>().color = new Color(0f / 255f, 0f / 255f, 60f / 255f);
        if (playernumber == 2) BarBack.GetComponent<Image>().color = new Color(60f / 255f, 0f / 255f, 0f / 255f);
    }

    private void InputControlKeyboard()
    {
        if (!Input.GetKey(up) || downCon > 0) upCon = 0;
        else if (upCon > 0) upCon = 1;
        if (!Input.GetKey(down) || upCon > 0) downCon = 0;
        else if (downCon > 0) downCon = 1;
        if (!Input.GetKey(right) || leftCon > 0) rightCon = 0;
        else if (rightCon > 0) rightCon = 1;
        if (!Input.GetKey(left) || rightCon > 0) leftCon = 0;
        else if (leftCon > 0) leftCon = 1;

        if (Input.GetKeyDown(up))
        {
            upCon = 2;
            downCon = 0;
        }

        if (Input.GetKeyDown(down))
        {
            upCon = 0;
            downCon = 2;
        }

        if (Input.GetKeyDown(right))
        {
            rightCon = 2;
            leftCon = 0;
        }

        if (Input.GetKeyDown(left))
        {
            rightCon = 0;
            leftCon = 2;
        }

        if (!Input.GetKey(special)) specialCon = 0;
        else specialCon = 1;
        if (Input.GetKeyDown(special)) specialCon = 2;

        if (!Input.GetKey(jump)) jumpCon = 0;
        else jumpCon = 1;
        if (Input.GetKeyDown(jump)) jumpCon = 2;

        if (!Input.GetKey(attack)) attackCon = 0;
        else attackCon = 1;
        if (Input.GetKeyDown(attack)) attackCon = 2;
    }

    private void InputControlJoyStick()
    {
        string jumpBut = "joystick button 1";
        string attackBut = "joystick button 0";
        string specialBut = "joystick button 2";

        Debug.Log("Vert Axis:"+ Input.GetAxis("Vertical_1")+" Hort Axis:"+ Input.GetAxis("Horizontal_1"));

        float VertInput = Input.GetAxis("Vertical_1");
        float HorzInput = Input.GetAxis("Horizontal_1");

        if (Mathf.Abs(VertInput) < .5f) VertInput = 0;
        if (Mathf.Abs(HorzInput) < .5f) HorzInput = 0;

        //VertInput *= -1;

        if (VertInput <= 0 || downCon > 0) upCon = 0;
        else if (upCon > 0) upCon = 1;
        if (VertInput >= 0 || upCon > 0) downCon = 0;
        else if (downCon > 0) downCon = 1;
        if (HorzInput <= 0 || leftCon > 0) rightCon = 0;
        else if (rightCon > 0) rightCon = 1;
        if (HorzInput >= 0 || rightCon > 0) leftCon = 0;
        else if (leftCon > 0) leftCon = 1;

        if (VertInput > 0)
        {
            upCon = 2;
            downCon = 0;
        }

        if (VertInput < 0)
        {
            upCon = 0;
            downCon = 2;
        }

        if (HorzInput > 0)
        {
            rightCon = 2;
            leftCon = 0;
        }

        if (HorzInput < 0)
        {
            rightCon = 0;
            leftCon = 2;
        }

        if (!Input.GetKey(specialBut)) specialCon = 0;
        else specialCon = 1;
        if (Input.GetKeyDown(specialBut)) specialCon = 2;

        if (!Input.GetKey(jumpBut)) jumpCon = 0;
        else jumpCon = 1;
        if (Input.GetKeyDown(jumpBut)) jumpCon = 2;

        if (!Input.GetKey(attackBut)) attackCon = 0;
        else attackCon = 1;
        if (Input.GetKeyDown(attackBut)) attackCon = 2;
    }

    private void InputControl()
    {
        if (AI)
        {
            AIControl();
        }
        else
        {
            if ((playernumber == 1 && player1Controller > -1) || (playernumber == 2 && player2Controller > -1)) InputControlJoyStick();
            else InputControlKeyboard();
        }
    }

    private void AIControl()
    {
        float[] cords = NearestBlock(); 
        float midDist = x - cords[0];

        Debug.Log("midDist = " + midDist + " to cords: " + cords[0] + "," + cords[1]);

        if (Mathf.Abs(midDist) > 2)
        {
            if (midDist > 0)
            {
                leftCon = 1;
                rightCon = 0;
            }
            else
            {
                leftCon = 0;
                rightCon = 1;
            }
        }
        else
        {
            leftCon = 0;
            rightCon = 0;
        }

        if (!grounded && vy < -0.3f && Mathf.Abs(midDist) > 2)
        {
            if (jumpCon == 0) jumpCon = 2;
            else jumpCon = 1;           
        }
        else
        {
            jumpCon = 0;
        }
    }

    private float[] NearestBlock()
    {
        float yfinal = 0, xfinal = 0;
        float[] cords = new float[2];

        float minDistance = 1000000;

        for (int ytest = 0; ytest < ArHt; ytest++)
        {
            for (int xtest = 0; xtest < ArWd; xtest++)
            {
                if (field[xtest,ytest] != null)
                {
                    float blockx = field[xtest, ytest].GetComponent<TileBehave>().xpos;
                    float blocky = field[xtest, ytest].GetComponent<TileBehave>().ypos;
                    float testDistance = Mathf.Pow(Mathf.Pow(x - blockx, 2) + Mathf.Pow(y - blocky, 2), 0.5f);                    

                    if (blocky + 1.5f <= y - (ObHt/2) && testDistance < minDistance)
                    {
                        Debug.Log("Cords: " + xtest + "," + ytest + " test distance " + testDistance + " to " + minDistance);

                        xfinal = blockx;
                        yfinal = blocky;
                        minDistance = testDistance;
                    }
                }
            }
        }

        cords[0] = xfinal;
        cords[1] = yfinal;
        return cords;
    }

    private void ReduceLags()
    {
        if (energy > -12) hitstun--;
        attackstun--;
        for (int i = 0; i < 10; i++)
        {
            immstun[i] -= 1;
        }
        speclag--;
        freezeframe--;
        genstun--;
        prejump--;
    }

    private void LoseLife()
    {
        lives--;
        if (lives <= 0) ResetTrigger = true;

        y = 12.75f;
        vy = 0;
        vx = 0;
        energy = 8;

        ResetTimers();

        invulFrames = 100;

        Plats[PlatCount + 1].Create(PlatObj, x, y - 2, 8, .5f, 180, 1);
    }

    private void ResetTimers()
    {
        attackstun = -2;
        hitstun = -2;
        genstun = -2;
        prejump = -2;
        speclag = -2;
        freezeframe = -2;
        for (int i = 0; i < 10; i++)
        {
            immstun[i] = -2;
        }
        attackType = -99;
    }

    private void PlatFallManage()
    {
        if (downCon > 0 && hitstun <= 0 && attackstun <= 0 && invulFrames <= 0) platFallCounter++;
        else platFallCounter = 0;

        if (platFallCounter >= 5) platfall = true;
        else platfall = false;
    }

    public void GeneralAttackControl()
    {
        framein = attackLength - attackstun;

        int hitboxnumber = 0;

        if (attackType >= 0 && IsHitBox[character, attackType, framein, hitboxnumber])
        {
            while (IsHitBox[character, attackType, framein, hitboxnumber] == true)
            {
                int type = HitData[character, attackType, framein, hitboxnumber].type;

                if (!faceright)
                {
                    if (type == 2) type = 4;
                    if (type == 3) type = 2;
                    if (type == 4) type = 3;
                }

                if (faceright) Hits[HitCount + 1].Create(x + HitData[character, attackType, framein, hitboxnumber].x1, x + HitData[character, attackType, framein, hitboxnumber].x2, y + HitData[character, attackType, framein, hitboxnumber].y1, y + HitData[character, attackType, framein, hitboxnumber].y2, playernumber, HitData[character, attackType, framein, hitboxnumber].pow, HitData[character, attackType, framein, hitboxnumber].angle, HitData[character, attackType, framein, hitboxnumber].stun, HitData[character, attackType, framein, hitboxnumber].dam, HitData[character, attackType, framein, hitboxnumber].immf, HitData[character, attackType, framein, hitboxnumber].freeze, type);
                else
                {
                    Hits[HitCount + 1].Create(x - HitData[character, attackType, framein, hitboxnumber].x2, x - HitData[character, attackType, framein, hitboxnumber].x1, y + HitData[character, attackType, framein, hitboxnumber].y1, y + HitData[character, attackType, framein, hitboxnumber].y2, playernumber, HitData[character, attackType, framein, hitboxnumber].pow, 180 - HitData[character, attackType, framein, hitboxnumber].angle, HitData[character, attackType, framein, hitboxnumber].stun, HitData[character, attackType, framein, hitboxnumber].dam, HitData[character, attackType, framein, hitboxnumber].immf, HitData[character, attackType, framein, hitboxnumber].freeze, type);
                }

                hitboxnumber++;
            }
        }
    }

    private void ConnectUI()
    {
        PBarTrans = EnergyBars[playernumber - 1].PBarTrans;
        BarBackTrans = EnergyBars[playernumber - 1].BarBackTrans;
        LivesTrans = EnergyBars[playernumber - 1].LivesTrans;
        PBar = EnergyBars[playernumber - 1].PBar;
        BarBack = EnergyBars[playernumber - 1].BarBack;
        Lives = EnergyBars[playernumber - 1].Lives;
            
        if(PBar == null) Debug.Log("!!!failed to connect UI for player " + playernumber);
        else Debug.Log("connected UI for player " + playernumber);
    }
}
