using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pivot_code : MonoBehaviour
{
    float spin;
    bool working;
    public List<Task> tasks = new List<Task>();
    private Task currentTask;

    private float magnitude;
    private bool workingOnTask;
    private Transform self;
    private string axis;
    private int polarity, direction;
    private Core core;
    private float angle;

    List<Cube> cubes;

    void Start ()
    {
        angle = 90;
        spin = 0;
        self = GameObject.Find("Pivot").transform;
        working = false;
        core = transform.parent.gameObject.GetComponent<Core>();
        workingOnTask = false;
        magnitude = core.getRotationMag();
    }

    public void turn(string axis, int direction)
    {
        core.clockwise = (direction == 1) ? true : false;
        switch (axis)
        {
            case "up": core.up = true; break;
            case "down": core.down = true; break;
            case "left": core.left = true; break;
            case "right": core.right = true; break;
            case "front": core.front = true; break;
            case "back": core.back = true; break;
        }

        core.setAction(1);
    }

    public void turn(string axis, int polarity, int direction)
    {
        this.axis = axis;
        this.polarity = polarity;
        this.direction = direction;

        if (!working)
        { 
            cubes = core.getCubes(axis, polarity);
            foreach (Cube cube in cubes)
                cube.transform.parent = self;
            working = true;
        }

        activate();
    }

    public void doTasks()
    {
        //Debug.Log("Tasks: " + tasks.Count);
        if (!hasTasks()) return;

        if (!workingOnTask)
        {
            workingOnTask = true;
            currentTask = tasks[0];
        }

        if (currentTask.isRelevant())
            currentTask.execute();

        tasks.Remove(currentTask);
       // Debug.Log(actionNumber++);
    }

    private void activate()
    {
        if (spin < angle)
        {
            Vector3 rot = new Vector3();
            magnitude = core.getRotationMag();
            float movement = magnitude * direction;

            switch (axis)
            {
                case "x":
                    rot = new Vector3(movement, 0, 0);
                    break;
                case "y":
                    rot = new Vector3(0, movement, 0);
                    break;
                case "z":
                    rot = new Vector3(0, 0, movement);
                    break;
            }
            transform.eulerAngles += rot;
            spin += magnitude;
        }
        else if(spin >= angle)
        {
            foreach (Cube cube in cubes)
            {
                cube.selfCorrect();
                cube.transform.parent = transform.parent;
            }

            transform.eulerAngles = new Vector3(0, 0, 0);

            spin = 0;
            working = false;
            core.resetState();
        }
    }

    public bool hasTasks()
    {
        return tasks.Count > 0;
    }

    public void addTask(Task t)
    {
        tasks.Add(t);
    }

    public bool isDoingTask()
    {
        return workingOnTask;
    }

    public void finishedTask()
    {
        workingOnTask = false;
        core.setAction(0);
    }
}
