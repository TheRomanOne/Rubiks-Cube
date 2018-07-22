using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Core : MonoBehaviour
{
    public bool working, up, down, left, right, front, back,
                clockwise;

    public float rotationMagintude;
    public int action;
    public bool shortTest = true;
    public bool longTest = false;

    public bool shuffle, solve;

    private int polarity = 0;
    private string axis = "";
    private int direction = 0;
    private Solution solution;
    private GameObject pivot;
    private Pivot_code pivot_c;
    private int long_test_num;
    
    public enum var_type { COLOR, AXIS };

    private void Start()
    {
        pivot = GameObject.Find("Pivot");
        pivot_c = pivot.GetComponent<Pivot_code>();
        action = 0;
        solution = GetComponent<Solution>();
        long_test_num = Random.Range(5, 15);
    }

    void Update()
    {
        if (shuffle) shuffleCube();
        else if (!working)
        {
            if (solve) solution.solveCube();
            if (shortTest) {
                shortTest = false;
                setTask(new Task("right", 1));
                setTask(new Task("front", 1));
                setTask(new Task("up", 1));
                setTask(new Task("down", 1));
                solve = true;
            }
            if(longTest)
            {
                //rotationMagintude = 90;
                shuffleCube();
                if (long_test_num-- == 0)
                {
                    solve = true;
                    longTest = false;
                    long_test_num = Random.Range(15, 30);
                }
            }
            pivot_c.doTasks();
        }
        init();

        if (clockwise)
            direction = 1;
        else direction = -1;

        switch (action)
        {
            case 0:
                if (pivot_c.isDoingTask())
                    pivot_c.finishedTask();
                break;
            case 1:
                if (!working) working = true;
                pivot_c.turn(axis, polarity, direction);
                break;
        }
    }

    public float getRotationMag()
    {
        return rotationMagintude;
    }

    private void shuffleCube()
    {
        if (working) return;

        switch (Random.Range(0, 6))
        {
            case 0: up = true; break;
            case 1: down = true; break;
            case 2: left = true; break;
            case 3: right = true; break;
            case 4: front = true; break;
            case 5: back = true; break;
        }

        clockwise = (Random.Range(0, 2) == 1) ? true : false;

        setAction(1);
    }

    public void run_long_test()
    {
        longTest = true;
    }

    private void init()
    {
        if (up) { axis = "y"; polarity = 1; }
        else if (down) { axis = "y"; polarity = -1; }
        else if (left) { axis = "z"; polarity = 1; }
        else if (right) { axis = "z"; polarity = -1; }
        else if (front) { axis = "x"; polarity = -1; }
        else if (back) { axis = "x"; polarity = 1; }
    }

    public void resetState()
    {
        up = down = left = right = front = back = false;
        working = false;
        clockwise = false;

        polarity = 0;
        axis = "";
        direction = 0;

        setAction(0);
    }

    public void setAction(int a)
    {
        action = a;
    }

    public void setTask(Task tsk)
    {
        pivot_c.addTask(tsk);
    }

    public List<Cube> getCubes(string context, var_type type)
    {
        List<Cube> ret = null;

        switch(type)
        {
            case var_type.AXIS:
                ret = getCubesByAxis(context);
                break;
            case var_type.COLOR:
                ret = getCubesByColor(context);
                break;
        }

        return ret;
    }

    public Vector3 getColorSide(string color)
    {
        return getCubes(1, color)[0].transform.position;
    }

    private List<Cube> getCubesByAxis(string axis)
    { 
        List<Cube> cubes = new List<Cube>();

        for (int index = 0; index < transform.childCount; index++)
        {
            Transform child = transform.GetChild(index);
            if (polarity != 0)
            {
                switch(axis)
                {
                    case "up":
                        cubes = getCubes("y", 1);
                        break;
                    case "down":
                        cubes = getCubes("y", -1);
                        break;
                    case "left":
                        cubes = getCubes("x", -1);
                        break;
                    case "right":
                        cubes = getCubes("x", 1);
                        break;
                }
            }
        }

        return cubes;

    }

    public List<Cube> getCubes(string axis, int polarity)
    {
        List<Cube> cubes = new List<Cube>();

        for (int index = 0; index < transform.childCount; index++)
        {
            Transform child = transform.GetChild(index);
            if (polarity != 0)
            {
                if (child.tag != "Pivot")
                    switch (axis)
                    {
                        case "x":
                            if ((polarity > 0 && child.position.x > 0.5) ||
                                (polarity < 0 && child.position.x < -0.5))
                                cubes.Add(child.GetComponent<Cube>());
                            break;
                        case "y":
                            if ((polarity > 0 && child.position.y > 0.5) ||
                                (polarity < 0 && child.position.y < -0.5))
                                cubes.Add(child.GetComponent<Cube>());
                            break;
                        case "z":
                            if ((polarity > 0 && child.position.z > 0.5) ||
                                (polarity < 0 && child.position.z < -0.5))
                                cubes.Add(child.GetComponent<Cube>());
                            break;
                    }
            }
        }
        return cubes;
    }

    public Vector3 getCubPosition(string color1, string color2, string color3)
    {
        Vector3 pos = new Vector3();

        for (int index = 0; index < transform.childCount; index++)
        {
            Transform cube = transform.GetChild(index);
            Cube cube_c = cube.GetComponent<Cube>();

            if (cube_c != null)
                if (cube_c.hasColor(color1) && cube_c.hasColor(color2) && cube_c.hasColor(color3))
                    pos = cube.position.normalized;
        }

        return pos;
    }

    public List<Cube> getCubes(string color1, string color2, string color3)
    {
        List<Cube> cubes = new List<Cube>();

        for (int index = 0; index < transform.childCount; index++)
        {
            Transform cube = transform.GetChild(index);
            Cube cube_c = cube.GetComponent<Cube>();

            if (cube_c != null)
                if (cube_c.hasColor(color1) && cube_c.hasColor(color2) && cube_c.hasColor(color3))
                    cubes.Add(cube_c);
        }

        return cubes;
    }

    public List<Cube> getCubes(string color1, string color2)
    {
        List<Cube> cubes = new List<Cube>();

        for (int index = 0; index < transform.childCount; index++)
        {
            Cube cube_c = transform.GetChild(index).GetComponent<Cube>();

            if (cube_c != null)
                if (cube_c.hasColor(color1) && cube_c.hasColor(color2))
                    cubes.Add(cube_c);
        }

        return cubes;
    }

    public List<Cube> getCubes(int size, string color)
    {
        List<Cube> cubes = new List<Cube>();

        for (int index = 0; index < transform.childCount; index++)
        {
            Cube cube_c = transform.GetChild(index).GetComponent<Cube>();

            if (cube_c != null)
                if (cube_c.hasColor(color) && cube_c.getSize() == size)
                    cubes.Add(cube_c);
        }

        return cubes;
    }

    private List<Cube> getCubesByColor(string color)
    {
        List<Cube> cubes = new List<Cube>();

        for (int index = 0; index < transform.childCount; index++)
        {
            Cube cube_c = transform.GetChild(index).GetComponent<Cube>();

            if (cube_c != null)
                if (cube_c.hasColor(color))
                    cubes.Add(cube_c);
        }

        return cubes;
    }

    public List<Cube> getCubesBySize(int n)
    {
        List<Cube> cubes = new List<Cube>();

        foreach(Transform child in transform)
        {
            Cube cube = child.GetComponent<Cube>();

            if (cube && cube.getSize() == n)
                cubes.Add(cube.GetComponent<Cube>());
            
        }

        return cubes;
    }

    public Cube getTop()
    {
        return getPolars(true);
    }

    public Cube getBottom()
    {
        return getPolars(false);
    }

    private Cube getPolars(bool isTop)
    {
        Cube ret = null;

        for (int index = 0; index < transform.childCount; index++)
        {
            Transform cube = transform.GetChild(index);
            Cube cube_c = cube.GetComponent<Cube>();

            if(cube_c != null && cube_c.getSize() == 1)
                if ((isTop && cube.position.y > 0.5) ||
                    (!isTop && cube.position.y < -0.5))
                    {
                        ret = cube_c;
                        break;
                    }
        }
        
        return ret;
    }

    public string vec3ToStr(Vector3 v_init)
    {
        string ret = "";
        Vector3 v = v_init.normalized;

        if (Vector3.Dot(v, new Vector3(0,1,0)) > 0.7)
            ret = "up";
        else if (Vector3.Dot(v, new Vector3(0, -1, 0)) > 0.7)
            ret = "down";
        else if (Vector3.Dot(v, new Vector3(1, 0, 0)) > 0.7)
            ret = "back";
        else if (Vector3.Dot(v, new Vector3(-1, 0, 0)) > 0.7)
            ret = "front";
        else if (Vector3.Dot(v, new Vector3(0, 0, 1)) > 0.7)
            ret = "left";
        else if (Vector3.Dot(v, new Vector3(0, 0, -1)) > 0.7)
            ret = "right";

        return ret;
    }

    public List<Cube> getCubes(int number)
    {
        List<Cube> cubes = new List<Cube>();

        foreach(Transform t in transform)
        {
            Cube c = t.GetComponent<Cube>();
            if (c && c.getSize() == number)
                cubes.Add(c);
        }

        return cubes;
    }

    public Cube getCornerCubeByCredentials(string side1, string side2, string side3)
    {
        List<Cube> cubes;

        if (side3.Equals(string.Empty))
            cubes = getCubes(2);
        else cubes = getCubes(3);

        foreach (Cube cube in cubes)
        {
            List<string> sides = cube.getAllSides();

            if (sides.Contains(side1) && sides.Contains(side2))
                if (side3.Equals(string.Empty))
                    return cube;
                else if (sides.Contains(side3))
                    return cube;
        }

        return null;
    }

    public Cube getCornerCubeByCredentials(List<string> sides)
    {
        if (sides.Count < 3)
            sides.Add(string.Empty);

        return getCornerCubeByCredentials(sides[0], sides[1], sides[2]);
    }

    public string side_90_deg(string side, int direction)
    {
        switch (side)
        {
            case "right":
                if (direction == 1)
                    return "back";
                else return "front";
            case "back":
                if (direction == 1)
                    return "left";
                else return "right";
            case "left":
                if (direction == 1)
                    return "front";
                else return "back";
            default: // front
                if (direction == 1)
                    return "right";
                else return "left";
        }
    }

    public Solution GetSolution()
    {
        return solution;
    }
}

public class Task
{
    public string side;
    public int direction;
    private Pivot_code pivot_c;
    private Core core;
    private bool relevant;

    public Task(string side, int direction)
    {
        relevant = true;
        this.side = side;

        if(direction == 0)
            this.direction = (Random.Range(0, 2) == 1)? 1 : -1;
        else this.direction = direction;

        pivot_c = GameObject.Find("Pivot").GetComponent<Pivot_code>();
        core = GameObject.Find("CubeCenter").GetComponent<Core>();
    }

    public void execute()
    {
        pivot_c.turn(side, direction);
        relevant = false;
    }

    public bool isRelevant()
    {
        return relevant;
    }

    public int getDirection()
    {
        return direction;
    }

    public string getSide()
    {
        return side;
    }
}