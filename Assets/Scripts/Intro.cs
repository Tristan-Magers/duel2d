using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro : ArenaObj
{
    public Sprite Slide1;
    public Sprite Slide2;

    int Slide;

    private void Awake()
    {
        Screen.fullScreen = false;
        //Screen.SetResolution(1080, 720, false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Soldier.controlType == 0)
        {
            Slide = 1;
        }
        if (Soldier.controlType == 1)
        {
            Slide = 2;
        }

        if (Slide == 1) gameObject.GetComponent<SpriteRenderer>().sprite = Slide1;
        if (Slide == 2) gameObject.GetComponent<SpriteRenderer>().sprite = Slide2;

        if (Input.GetKeyDown("m"))
        {
            Soldier.controlType++;
            if (Soldier.controlType > 1) Soldier.controlType = 0;
        }

        if (Input.GetKey("0"))
        {
            Debug.Log("exit");
            Application.Quit();
        }

        if (Input.GetKeyDown("n"))
        {
            ResetTrigger = true;
            SceneManager.LoadScene(sceneName: "Duel TNT");
        }
    }
}
