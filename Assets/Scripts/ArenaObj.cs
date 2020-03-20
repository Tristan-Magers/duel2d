using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaObj : MonoBehaviour
{
    public static float CamX, CamY, AveX, AveY, XMin, YMin, XMax, YMax;
    public static int CamObjects = 3;

    public static RectTransform CanvasTrans;
    public static GameObject Canvas;

    public static float[] Energies = new float[2];
    public static int[] LivesTrack = new int[2];

    public static bool Pause;

    public static GameObject PlatObj;

    public static int ArHt = 3;
    public static int ArWd = 14;
    public static int Map = 0;

    //ArWd x ArHt
    public static GameObject[,] field = new GameObject[14, 3];
    public static int[,] changeHealth = new int[14, 3];

    public static Boom[] Booms = new Boom[100];
    public static HitBox[] Hits = new HitBox[100];
    public static HitBox[,,,] HitData = new HitBox[10, 11, 120, 5];
    public static int[,,] AttackData = new int[10, 11, 5];
    public static bool[,,,] IsHitBox = new bool[10, 11, 120, 5];

    public struct Bar
    {
        public RectTransform BarBackTrans;
        public RectTransform PBarTrans;
        public RectTransform LivesTrans;
        public GameObject BarBack;
        public GameObject PBar;
        public GameObject Lives;
    }

    public static Bar[] EnergyBars = new Bar[10];

    public static Plat[] Plats = new Plat[50];

    public static int PlatCount = -1;
    public static int BoomCount = -1;
    public static int HitCount = -1;
    public static bool ResetTrigger = false;
    public static bool ResetCycle = false;

    public struct Boom
    {

        public float x, y, r, kb, br, uprat, drat;
        public int t, stun, imm;

        public void Create(float xpos, float ypos, float range, float knock, float breaker, int type, int hitstun, int immune, float upratio, float downratio)
        {
            x = xpos;
            y = ypos;
            r = range;
            kb = knock;
            br = breaker;
            t = type;
            stun = hitstun;
            imm = immune;
            uprat = upratio;
            drat = downratio;

            BoomCount++;
        }
    }

    public struct Plat
    {
        int timer;
        public int type;
        public float x, y, length, height;
        public GameObject plat;
        public bool active;

        public void Create(GameObject PlatOrg, float xpos, float ypos, float platlength, float platheight, int lasttimer, int plattype)
        {
            PlatCount++;

            x = xpos;
            y = ypos;
            length = platlength;
            height = platheight;
            type = plattype;

            timer = lasttimer;

            active = true;

            Vector3 angle = new Vector3(0, 0, 0);
            plat = Instantiate(PlatOrg, new Vector3(x, y, -.12f), Quaternion.Euler(angle));
            plat.transform.localScale = new Vector3(length, height, 1);
            plat.GetComponent<RespawnPlat>().SetVals(PlatCount, timer);
        }

        public void SetTimer(int lasttimer)
        {
            plat.GetComponent<RespawnPlat>().ChangeLast(lasttimer);
        }

        public void ChangePos(float vx, float vy)
        {
            x += vx;
            y += vy;

            plat.GetComponent<RespawnPlat>().vx = vx;
            plat.GetComponent<RespawnPlat>().vy = vy;
        }
    }

    public struct HitBox
    {
        public float x1, x2, y1, y2, pow, angle, dam;
        public int imm, stun, immf, freeze, type;

        public void Create(float X1, float X2, float Y1, float Y2, int immune, float power, float hitangle, int hitstun, float damage, int immframes, int freezeframes, int hittype)
        {
            x1 = X1;
            x2 = X2;
            y1 = Y1;
            y2 = Y2;
            imm = immune;
            pow = power;
            angle = hitangle;
            stun = hitstun;
            dam = damage;
            immf = immframes;
            freeze = freezeframes;
            type = hittype;

            HitCount++;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CamAveAdd(float x, float y)
    {
        CamObjects++;
        if (x < XMin) XMin = x;
        if (y < YMin) YMin = y;

        if (x > XMax) XMax = x;
        if (y > YMax) YMax = y;

        AveX += x;
        AveY += y;
    }
}
