using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CameraScript : ArenaObj
{
    public static GameObject Cam;

    public GameObject menu, pause;

    public GameObject BarBack;
    public GameObject PBar;
    public GameObject LivesIcon;

    public Sprite Life1;
    public Sprite Life2;
    public Sprite Life3;

    float FoV;
    float smooth = 10;

    bool slowmo = false;

    void Awake()
    {
        Cam = gameObject;
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 60;
    }

    // Start is called before the first frame update
    void Start()
    {
        Canvas = GameObject.Find("Canvas");
        CanvasTrans = Canvas.GetComponent<RectTransform>();
        CanvasTrans.anchoredPosition = Vector3.zero;

        for (int i = 0; i <  Energies.Length; i++)
        {
            Debug.Log("Make UI " + i);

            EnergyBars[i].BarBack = Instantiate(BarBack, transform.position, transform.rotation);
            EnergyBars[i].BarBack.transform.SetParent(Canvas.transform);
            EnergyBars[i].PBar = Instantiate(PBar, transform.position, transform.rotation);
            EnergyBars[i].PBar.transform.SetParent(Canvas.transform);
            EnergyBars[i].Lives = Instantiate(LivesIcon, transform.position, transform.rotation);
            EnergyBars[i].Lives.transform.SetParent(Canvas.transform);

            EnergyBars[i].BarBackTrans = EnergyBars[i].BarBack.GetComponent<RectTransform>();
            EnergyBars[i].PBarTrans = EnergyBars[i].PBar.GetComponent<RectTransform>();
            EnergyBars[i].LivesTrans = EnergyBars[i].Lives.GetComponent<RectTransform>();
        }

        CamX = (ArWd * 3 / 2);
        CamY = 7;
    }

    // Update is called once per frame
    void Update()
    {

        if (Pause)
        {
            if (Input.GetKey("0"))
            {
                Debug.Log("exit");
                Application.Quit();
            }

            if (Input.GetKey("6"))
            {
                 Pause = false;
                SceneManager.LoadScene(sceneName: "Intro");
            }

            if (Input.GetKeyDown("9"))
            {
                if (slowmo)
                {
                    slowmo = false;
                    Application.targetFrameRate = 60;
                }
                else
                {                   
                    slowmo = true;
                    Application.targetFrameRate = 10;
                }
                 Pause = false;
            }

            float UIScale = Screen.height / 400f;

            UIScale *= .7f;

            menu.GetComponent<RectTransform>().localScale = new Vector3(UIScale * 3f, UIScale * 4f, 100);

            menu.GetComponent<Image>().enabled = true;
            pause.GetComponent<Image>().enabled = false;

            return;
        }
        else
        {
            for (int i = 0; i < Energies.Length; i++)
            {
                if (LivesTrack[i] == 1) EnergyBars[i].Lives.GetComponent<Image>().sprite = Life1;
                if (LivesTrack[i] == 2) EnergyBars[i].Lives.GetComponent<Image>().sprite = Life2;
                if (LivesTrack[i] == 3) EnergyBars[i].Lives.GetComponent<Image>().sprite = Life3;
            }

                pause.GetComponent<RectTransform>().localPosition = new Vector3(Screen.width / 2f - 34, 20 - Screen.height / 2f, 1);
            menu.GetComponent<Image>().enabled = false;
            pause.GetComponent<Image>().enabled = true;
        }

        AveX /= CamObjects;
        AveY /= CamObjects;

        AveY *= 1.8f;

        if (AveY > 10) AveY = 10;

        float changex = (CamX - AveX) / smooth;
        float changey = (CamY - AveY) / smooth;

        if (Mathf.Abs(changex) < 0.01f) changex = 0;
        if (Mathf.Abs(changey) < 0.01f) changey = 0;

        if (Mathf.Abs(changex) > 0.6f) changex = .6f * Mathf.Sign(changex);
        if (Mathf.Abs(changey) > 0.6f) changey = .6f * Mathf.Sign(changey);

        CamX -= changex;
        CamY -= changey;

        transform.position = new Vector3(CamX, CamY + 1.5f, -30);

        float difx = 0, dify = 0;

        if (Mathf.Abs(XMax - AveX) > Mathf.Abs(XMax - AveX)) difx = Mathf.Abs(XMax - AveX);
        else difx = Mathf.Abs(XMin - AveX);

        if (Mathf.Abs(YMax - AveY) > Mathf.Abs(YMax - AveY)) dify = Mathf.Abs(YMax - AveY);
        else dify = Mathf.Abs(YMin - AveY);

        float dif = Mathf.Pow(Mathf.Pow(difx, 2) + Mathf.Pow(dify, 2), 0.5f);

        FoV += ((32 + dif * 1.1f) - FoV) / smooth;

        this.GetComponent<Camera>().fieldOfView = FoV;  

        CamObjects = 1;

        YMax = -1000;
        XMax = -1000;

        YMin = 1000;
        XMin = 1000;

        AveX = ( ArWd * 3 / 2) * CamObjects;
        AveY = 3f * CamObjects;

        if (ResetCycle)
        {
            CamX = ( ArWd * 3 / 2);
            CamY = 0;
            FoV = 65;
        }
    }
}
