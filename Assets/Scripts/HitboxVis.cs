using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxVis : ArenaObj
{
    int last = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Pause) return;

        if (last == 0) Destroy(gameObject);
        last--;
    }
}
