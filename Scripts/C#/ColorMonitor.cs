using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorMonitor : MonoBehaviour
{
    public Vector3 p;
    private Cube parent;
    private Core core;

    private void Start()
    {
        parent = transform.parent.GetComponent<Cube>();
        core = parent.transform.GetComponentInParent<Core>();
    }

    void Update()
    {
        p = transform.position;
    }

    public Task bring_down()
    {
        return bring_to(1);
    }

    public Task bring_up()
    {
        return bring_to(-1);
    }

    private Task bring_to(int dir)
    {
        List<ColorMonitor> others = parent.get_all_color_monitors();
        others.Remove(this);
        ColorMonitor mainColor = null;

        if (others.Count == 2)
        {
            Vector3 up = new Vector3(0, 1, 0),
                    down = new Vector3(0, -1, 0),
                    col1 = parent.getColorRealPosition(others[0].name).normalized,
                    col2 = parent.getColorRealPosition(others[1].name).normalized;
            float dgima = Vector3.Dot(col2, up);

            if (Vector3.Dot(col2, up) > 0.7 || Vector3.Dot(col2, down) > 0.7)
                mainColor = others[0];
            else mainColor = others[1];

        } else if (others.Count == 1)
            mainColor = others[0];

        Vector3 pos = new Vector3(0, dir, 0);
        Vector3 cross = Vector3.Cross(p, pos);
        int direction = 1;

        if (Mathf.Abs(cross.x) > 1.2)
            direction *= (cross.x > 0) ? -1 : 1;
        else direction *= (cross.z > 0) ? -1 : 1;

        string side = core.vec3ToStr(mainColor.transform.position);

        Task t = new Task(side, direction);
        core.setTask(t);

        return t;
    }
}
