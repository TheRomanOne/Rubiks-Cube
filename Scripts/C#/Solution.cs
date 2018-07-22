using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Solution : MonoBehaviour
{
    private Core core;
    private bool working;
    private int action;
    
    private Pivot_code pivot_c;

    private Stage1 s1;
    private Stage2 s2;
    private Stage3 s3;

    public void init()
    {
        working = true;
        core = GetComponent<Core>();
        action = 1;

        pivot_c = GameObject.Find("Pivot").GetComponent<Pivot_code>();

        s1 = new Stage1(pivot_c, core);
        s2 = new Stage2(pivot_c, core);
        s3 = new Stage3(pivot_c, core);
    }

    public void solveCube()
    {
        if(!working)
            init();

        switch(action)
        {
            case 1:
                if (s1.run())
                {
                    action = 2;
                    goto case 2;
                }
                break;
            case 2:
                if (s2.run())
                {
                    action = 3;
                    goto case 3;
                }
                break;
            case 3:
                //core.rotationMagintude = 1.5f;
                if (s3.run())
                {
                    action = 4;
                    goto case 4;
                }
                break;
            case 4:
                Debug.Log("Done");
                //core.run_long_test();
                working = false;
                Application.Quit();
                break;
        }
    }

    public void setAction(int a)
    {
        action = a;
    }
}