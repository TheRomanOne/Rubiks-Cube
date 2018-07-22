using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    private List<string> colors = new List<string>();
    private Core core;
    public int size;

    void Start ()
    {
        foreach (Transform child in transform)
            colors.Add(child.name);
        size = colors.Count;

        core = transform.parent.transform.GetComponentInParent<Core>();
    }
	
    public int getSize()
    {
        return size;
    }

    public bool hasColor(string color)
    {
        return colors.Contains(color);
    }

    public List<string> getColors()
    {
        return new List<string>(colors);
    }

    public void selfCorrect()
    {
        Vector3 correction = new Vector3();

        if (transform.position.x > .5)
            correction  += new Vector3(1, 0, 0);
        else if (transform.position.x < -.5)
            correction += new Vector3(-1, 0, 0);

        if (transform.position.y > .5)
            correction += new Vector3(0, 1, 0);
        else if (transform.position.y < -.5)
            correction += new Vector3(0, -1, 0);

        if (transform.position.z > .5)
            correction += new Vector3(0, 0, 1);
        else if (transform.position.z < -.5)
            correction += new Vector3(0, 0, -1);

        transform.position = correction;
    }

    public string getSideFacing(string color)
    {
        if (colors.Contains(color))
            foreach (Transform child in transform)
                if (child.name.Equals(color))
                {
                    if (Vector3.Dot(child.position.normalized, new Vector3(1, 0, 0)) > 0.7)
                        return "back";
                    else if (Vector3.Dot(child.position.normalized, new Vector3(-1, 0, 0)) > 0.7)
                        return "front";
                    else if (Vector3.Dot(child.position.normalized, new Vector3(0, 1, 0)) > 0.7)
                        return "up";
                    else if (Vector3.Dot(child.position.normalized, new Vector3(0, -1, 0)) > 0.7)
                        return "down";
                    else if (Vector3.Dot(child.position.normalized, new Vector3(0, 0, 1)) > 0.7)
                        return "left";
                    else if (Vector3.Dot(child.position.normalized, new Vector3(0, 0, -1)) > 0.7)
                        return "right";
                }
        return "";
    }

    public Vector3 getColorRealPosition(string color)
    {
        if (colors.Contains(color))
            foreach (Transform child in transform)
                if (child.name.Equals(color))
                    return child.position;

        return new Vector3();
    }

    public List<ColorMonitor> get_all_color_monitors()
    {
        List<ColorMonitor> cm = new List<ColorMonitor>();

        foreach (Transform c in transform)
            cm.Add(c.GetComponent<ColorMonitor>());

        return cm;
    }

    public Task bring_down(string color)
    {
        return bring_to(color, "down");
    }

    public Task bring_up(string color)
    {
        return bring_to(color, "up");
    }

    public Task bring_to(string color, string side)
    {
        Task ret = null;

        foreach(ColorMonitor cm in get_all_color_monitors())
            if (color.Equals(cm.name))
            {
                if(side.Equals("down"))
                    ret = cm.bring_down();
                else if (side.Equals("up"))
                    ret = cm.bring_up();
            }

        return ret;
    }

    public List<string> getAllSides()
    {
        List<string> ret = new List<string>();

        foreach (Transform t in transform)
            ret.Add(getSideFacing(t.name));

        return ret;
    }

    public string getColorAtSide(string side)
    {
        foreach (Transform t in transform)
            if (getSideFacing(t.name).Equals(side))
                return t.name;

        return "";

    }

    public void alignWithColor(string refColor)
    {
        string facingSide = getSideFacing(refColor);
        string colorSide = core.vec3ToStr(core.getColorSide(refColor));

        //Check if the desired color is on left/right, else turn bottom twice
        if (!facingSide.Equals(colorSide))
        {
            int dir = 1;
            if (!core.side_90_deg(colorSide, dir).Equals(facingSide))
            {
                dir = -1;

                core.setTask(new Task("down", dir));

                if (!core.side_90_deg(colorSide, dir).Equals(facingSide))
                    core.setTask(new Task("down", dir));
            }
            else core.setTask(new Task("down", dir));
        }
    }

    public void alignCornerUp(string primaryColor, string secondaryColor, Core.var_type vType)
    {
        string originPrime = primaryColor;
        string originSecond = secondaryColor;

        if (vType == Core.var_type.AXIS)
        {
            string side1 = core.vec3ToStr(core.getColorSide(primaryColor));
            string side2 = core.vec3ToStr(core.getColorSide(secondaryColor));

            primaryColor = getColorAtSide(side1);
            secondaryColor = getColorAtSide(side2);
        }

        string mainSideFace = getSideFacing(primaryColor);
        string otherSideFace = core.vec3ToStr(core.getColorSide(secondaryColor));

        Cube atTheTop = null;
        List<Cube> cubesAtTop = core.getCubes(3);
        foreach (Cube c in cubesAtTop)
        {
            List<string> sides = c.getAllSides();
            int count = 0;

            if (sides.Contains("up"))
            {
                List<string> colorsInReady = getAllSides();

                foreach (string s in colorsInReady)
                    if (sides.Contains(s))
                        count++;

                if (count == 2)
                {
                    atTheTop = c;
                    break;
                }
            }
        }

        if(vType == Core.var_type.AXIS)
        {
            mainSideFace = core.vec3ToStr(core.getColorSide(originPrime));
            otherSideFace = core.vec3ToStr(core.getColorSide(originSecond));
        }

            int dir = 1;
        if (!core.side_90_deg(mainSideFace, dir).Equals(otherSideFace))
            dir = -1;

        core.setTask(new Task("down", dir));
        string co = atTheTop.getColorAtSide(getSideFacing(primaryColor));
        atTheTop.bring_down(co);
        core.setTask(new Task("down", -1 * dir));

        bring_up(primaryColor);
    }

    public bool doesColorAlign(string color)
    {
        return getSideFacing(color).Equals(core.vec3ToStr(core.getColorSide(color)));
    }

    public bool isInPlace()
    {
        foreach (string color in colors)
            if (!doesColorAlign(color))
                return false;

        return true;
    }
}
