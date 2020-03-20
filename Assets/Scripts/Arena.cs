using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class Arena : ArenaObj
{
    public TextAsset HitBoxData;

    public GameObject HitboxVisOrg;
    public GameObject BoomOrg;
    GameObject[] BoomObjs = new GameObject[10];
    public int BoomUseNum = 0;

    public GameObject PlatOrg;
    public GameObject TileOrg;

    public Sprite BoomEx;
    public Sprite BoomZap;

    public Material MatZap;

    public int Map;

    public bool HitboxVisToggle = false;

    public static class States
    {
        public static Sprite sprite_0;
        public static Sprite sprite_1;
        public static Sprite sprite_2;
        public static Sprite sprite_3;
        public static Sprite sprite_4;
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i <= 9; i++)
        {
            BoomObjs[i] = Instantiate(BoomOrg, new Vector3(0, 1000, 0), transform.rotation);           
        }

        Debug.Log(Input.GetJoystickNames());

        CreateHitData();

        PlatObj = PlatOrg;

        MakeMap();
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("3"))
        {
            if (Pause) Pause = false;
            else Pause = true;
        }

        if (Pause)
        {
            if (Input.GetKeyDown("5"))
            {
                Map++;
                ResetTrigger = true;
                Pause = false;
            }
            if (Input.GetKeyDown("4"))
            {
                ResetTrigger = true;
                Pause = false;
            }

            if (Input.GetKeyDown("7"))
            {
                Screen.fullScreen = !Screen.fullScreen;
            }

            if (Input.GetKeyDown("8"))
            {
                Pause = false;
                if (HitboxVisToggle) HitboxVisToggle = false;
                else HitboxVisToggle = true;
            }

            return;
        }

        

        Map = Map;
    }

    private void LateUpdate()
    {
        if (Pause) return;

        for (int i = 0; i <= BoomCount; i++)
        {
            if (BoomUseNum != -1)
            {
                BoomObjs[BoomUseNum] = Instantiate(BoomOrg, new Vector3(Booms[i].x, Booms[i].y, -.2f), transform.rotation);
                BoomObjs[BoomUseNum].transform.localScale = new Vector3(Booms[i].r * 2, Booms[i].r * 2, Booms[i].r * 2);

                BoomObjs[BoomUseNum].GetComponent<BoomScript>().StartBoom();

                //if (Booms[i].t == 1) BoomObjs[BoomUseNum].GetComponent<SpriteRenderer>().sprite = BoomEx;
                if (Booms[i].t == 2) BoomObjs[BoomUseNum].GetComponent<MeshRenderer>().material = MatZap;
                //if (Booms[i].t == 2 && Booms[i].imm <= 0) BoomObjs[BoomUseNum].GetComponent<SpriteRenderer>().color = Color.red;
            }

            AudioSource audioData;
            audioData = GetComponent<AudioSource>();
            audioData.pitch = (1 / Mathf.Pow(Booms[i].r ,  0)) + 0.5f;
            audioData.Play(0);

            //Booms[i].x;
            //Booms[i].y;
            for (int y = 0; y < ArHt; y++)
            {
                for (int x = 0; x < ArWd; x++)
                {
                    float xpos = (x * 3) + 1.5f;
                    float ypos = (y * -3) - 1.5f;
                    float dist = Mathf.Pow(Mathf.Pow(xpos - Booms[i].x, 2) + Mathf.Pow(ypos - Booms[i].y, 2), 0.5f);
                    if (dist <= 1.5f)
                    {
                        changeHealth[x, y] += (int)Booms[i].br;
                    }
                    else
                    {
                        if (dist - 1.5f <= Booms[i].r)
                        {
                            changeHealth[x, y] += (int)Mathf.Round(Booms[i].br - (Booms[i].br * ((dist - 1.5f) / Booms[i].r)));
                        }
                    }
                }
            }
        }

        CheckActiveBooms();

        for (int i = PlatCount; i > -1; i--)
        {
            if (Plats[i].active == false) PlatCount--;
            else break;
        }

        BoomCount = -1;

        if (HitboxVisToggle)
        {
            if (HitCount >= 0) Debug.Log("show hits: " + HitCount);

            for (int i = 0; i <= HitCount; i++)
            {
                GameObject NewHitVis;
                float xave = (Hits[i].x1 + Hits[i].x2) / 2;
                float yave = (Hits[i].y1 + Hits[i].y2) / 2;
                float xwid = Mathf.Abs(Hits[i].x1 - Hits[i].x2);
                float ywid = Mathf.Abs(Hits[i].y1 - Hits[i].y2);
                NewHitVis = Instantiate(HitboxVisOrg, new Vector3(xave, yave, -.05f), transform.rotation);
                NewHitVis.transform.localScale = new Vector3(xwid, ywid, 1);
            }
        }

        HitCount = -1;

        if (ResetCycle)
        {
            Reset();
            ResetCycle = false;
        }

        if (ResetTrigger == true)
        {
            ResetTrigger = false;
            ResetCycle = true;
        }

        if (ResetCycle) Reset();
    }

    public void Reset()
    {
        Pause = false;

        HitCount = -1;
        BoomCount = -1;

        for (int y = 0; y < ArHt; y++)
        {
            for (int x = 0; x < ArWd; x++)
            {
                Destroy(field[x, y]);
            }
        }

        MakeMap();
    }

    private void CreateHitData()
    {
        List<string> textArray;

        int character = 1;
        int move = 0;

        int action = 0;

        textArray = HitBoxData.text.Split('\n').ToList();

        //data organized into character-move-frame

        for (int i=0; textArray.Count > i; i++)
        {
            Debug.Log("Read line" + i + " : " + textArray[i]);
            
            if (action > 0)
            {
                Debug.Log("Action event");

                switch (action)
                {
                    case 1:
                        {
                            character = int.Parse(textArray[i]);

                            Debug.Log("Set character to: " + character);

                            break;
                        }
                    case 2:
                        {
                            float X1, X2, Y1, Y2, power, hitangle, damage;
                            int immune, hitstun, immframes, freezeframes, startframe, endframe, hittype;

                            List<string> Values = textArray[i].Split(',').ToList();

                            X1 = float.Parse(Values[0]);
                            Y1 = float.Parse(Values[1]);
                            X2 = float.Parse(Values[2]);
                            Y2 = float.Parse(Values[3]);
                            immune = int.Parse(Values[4]);
                            power = float.Parse(Values[5]);
                            hitangle = float.Parse(Values[6]);
                            hitstun = int.Parse(Values[7]);
                            damage = float.Parse(Values[8]);
                            immframes = int.Parse(Values[9]);
                            freezeframes = int.Parse(Values[10]);
                            hittype = int.Parse(Values[11]);
                            startframe = int.Parse(Values[12]);
                            endframe = int.Parse(Values[13]);

                            int hitboxnumber = 0;

                            for (int h = startframe; h <= endframe; h++)
                            {
                                while (IsHitBox[character, move, h, hitboxnumber] == true)
                                {
                                    if (IsHitBox[character, move, h, hitboxnumber] == true) hitboxnumber++;
                                }
    
                                HitData[character, move, h, hitboxnumber].Create(X1, Y1, X2, Y2, immune, power, hitangle, hitstun, damage, immframes, freezeframes, hittype);
                                IsHitBox[character, move, h, hitboxnumber] = true;
                                Debug.Log("Added move:" + move + " on frame:" + h);
                            }

                            break;
                        }
                }

                action = 0;
            }
            else
            {
                textArray[i] = Regex.Replace(textArray[i], @"[^A-Za-z0-9]+", "");

                switch (textArray[i])
                {
                    case "Character":
                        {
                            Debug.Log("Trigger Set Character");
                            action = 1;
                            break;
                        }
                    case "Neutral":
                        {
                            move = 0;
                            action = 3;
                            break;
                        }
                    case "Stab":
                        {
                            move = 1;
                            action = 3;
                            break;
                        }
                    case "Dash":
                        {
                            move = 2;
                            action = 3;
                            break;
                        }
                    case "Upper":
                        {
                            move = 3;
                            action = 3;
                            break;
                        }
                    case "Power":
                        {
                            move = 4;
                            action = 3;
                            break;
                        }
                    case "Nair":
                        {
                            move = 5;
                            action = 3;
                            break;
                        }
                    case "Fair":
                        {
                            move = 6;
                            action = 3;
                            break;
                        }
                    case "Dair":
                        {
                            move = 7;
                            action = 3;
                            break;
                        }
                    case "Uair":
                        {
                            move = 8;
                            action = 3;
                            break;
                        }
                    case "AND":
                        {
                            action = 2;
                            break;
                        }
                    default:
                        {
                            //Debug.Log("No case");
                            break;
                        }
                }

                if (action == 3)
                {
                    action = 2;
                    i++;

                    textArray[i] = Regex.Replace(textArray[i], @"[^A-Za-z0-9]+", "");

                    List<string> Values = textArray[i].Split(',').ToList();

                    int attackstunset = int.Parse(Values[0]);

                    Debug.Log("Set attackstun for move " + move + " to " + attackstunset);

                    AttackData[character, move, 0] = attackstunset;
                }
            }            
        }

        //HitData[1,0,3].Create(0.2f, 2.25f, -0.3f, 0.3f, -1, 0.4f, 55, 20, 4f, 4, 3);
        //IsHitBox[1, 0, 3] = true;
    }

    private void MakeMap()
    {
        if (Map > 4) Map = 0;

        if (Map == 0)
        {
            ArHt = 3;
            ArWd = 14;

            MakeField();

            Destroy(field[0, 0]);
            Destroy(field[1, 0]);
            Destroy(field[2, 0]);
            Destroy(field[3, 0]);
            Destroy(field[3, 1]);
            Destroy(field[3, 2]);

            Destroy(field[13, 0]);
            Destroy(field[12, 0]);
            Destroy(field[11, 0]);
            Destroy(field[10, 0]);
            Destroy(field[10, 1]);
            Destroy(field[10, 2]);

            field[6, 1].GetComponent<TileBehave>().type = 2;
            field[7, 1].GetComponent<TileBehave>().type = 2;
            field[6, 2].GetComponent<TileBehave>().type = 2;
            field[7, 2].GetComponent<TileBehave>().type = 2;
            field[5, 2].GetComponent<TileBehave>().type = 2;
            field[8, 2].GetComponent<TileBehave>().type = 2;
        }

        if (Map == 1)
        {
            ArHt = 3;
            ArWd = 14;

            MakeField();

            Destroy(field[6, 0]);
            Destroy(field[5, 0]);
            Destroy(field[4, 0]);
            Destroy(field[3, 0]);
            Destroy(field[3, 1]);
            Destroy(field[3, 2]);

            Destroy(field[7, 0]);
            Destroy(field[8, 0]);
            Destroy(field[9, 0]);
            Destroy(field[10, 0]);
            Destroy(field[10, 1]);
            Destroy(field[10, 2]);

            field[4, 2].GetComponent<TileBehave>().type = 2;
            field[6, 2].GetComponent<TileBehave>().type = 2;
            field[7, 2].GetComponent<TileBehave>().type = 2;
            field[9, 2].GetComponent<TileBehave>().type = 2;
        }

        if (Map == 2)
        {
            ArHt = 3;
            ArWd = 10;

            MakeField();

            field[0, 0].GetComponent<TileBehave>().type = 1;
            field[1, 0].GetComponent<TileBehave>().type = 1;

            field[4, 0].GetComponent<TileBehave>().type = 1;
            field[5, 0].GetComponent<TileBehave>().type = 1;

            field[8, 0].GetComponent<TileBehave>().type = 1;
            field[9, 0].GetComponent<TileBehave>().type = 1;
        }

        if (Map == 3)
        {
            ArHt = 3;
            ArWd = 11;

            MakeField();

            changeHealth[0, 1] = 10;
            changeHealth[0, 2] = 10;
            changeHealth[1, 2] = 10;
            changeHealth[2, 2] = 10;
            changeHealth[3, 2] = 10;
            changeHealth[7, 2] = 10;
            changeHealth[8, 2] = 10;
            changeHealth[9, 2] = 10;
            changeHealth[10, 1] = 10;
            changeHealth[10, 2] = 10;

            field[4, 2].GetComponent<TileBehave>().type = 2;
            field[5, 2].GetComponent<TileBehave>().type = 2;
            field[6, 2].GetComponent<TileBehave>().type = 2;
            field[4, 1].GetComponent<TileBehave>().type = 2;
            field[5, 1].GetComponent<TileBehave>().type = 2;
            field[6, 1].GetComponent<TileBehave>().type = 2;
            field[4, 0].GetComponent<TileBehave>().type = 2;
            field[5, 0].GetComponent<TileBehave>().type = 2;
            field[6, 0].GetComponent<TileBehave>().type = 2;
        }

        if (Map == 4)
        {
            ArHt = 3;
            ArWd = 12;

            MakeField();

            changeHealth[1, 0] = 10;
            changeHealth[2, 0] = 10;
            changeHealth[3, 0] = 10;
            changeHealth[4, 0] = 10;
            changeHealth[5, 0] = 10;
            changeHealth[6, 0] = 10;
            changeHealth[7, 0] = 10;
            changeHealth[8, 0] = 10;
            changeHealth[9, 0] = 10;
            changeHealth[10, 0] = 10;

            field[2, 2].GetComponent<TileBehave>().type = 2;
            field[3, 2].GetComponent<TileBehave>().type = 2;

            field[8, 2].GetComponent<TileBehave>().type = 2;
            field[9, 2].GetComponent<TileBehave>().type = 2;

            field[5, 2].GetComponent<TileBehave>().type = 1;
            field[6, 2].GetComponent<TileBehave>().type = 1;

            field[5, 2].GetComponent<TileBehave>().state = 1;
            field[6, 2].GetComponent<TileBehave>().state = 1;
        }

        Plats[PlatCount + 1].Create(PlatObj, (ArWd * 3 / 2) - (50 + 11 + (ArWd * 3 / 2)), 0, 100, 300, -1, 0);
        Plats[PlatCount + 1].Create(PlatObj, (ArWd * 3 / 2) + (50 + 11 + (ArWd * 3 / 2)), 0, 100, 300, -1, 0);
    }

    public void MakeField()
    {
        field = new GameObject[ArWd, ArHt];
        changeHealth = new int[ArWd, ArHt];

        for (int y = 0; y < ArHt; y++)
        {
            for (int x = 0; x < ArWd; x++)
            {
                //GameObject tile = new GameObject("Tile" + x + "," + y);
                GameObject tile = Instantiate(TileOrg, transform.position, transform.rotation);
                tile.transform.position = new Vector3(x * 3f + 1.5f, -y * 3f - 1.5f, 0.0f);
                MonoBehaviour tilescript = tile.GetComponent<TileBehave>();
                tilescript.GetComponent<TileBehave>().x = x;
                tilescript.GetComponent<TileBehave>().y = y;
                tilescript.GetComponent<TileBehave>().xpos = x * 3f + 1.5f;
                tilescript.GetComponent<TileBehave>().ypos = -y * 3f - 1.5f;
                tilescript.GetComponent<TileBehave>().type = 0;
                field[x, y] = tile;
                changeHealth[x, y] = 0;
            }
        }
    }

    void CheckActiveBooms()
    {
        BoomUseNum = -1;

        for (int i = 0; i <= 9; i++)
        {
            if (!BoomObjs[i].GetComponent<BoomScript>().active) BoomUseNum = i;
        }
    }
}
