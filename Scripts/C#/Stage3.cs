using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage3
{
    private Pivot_code pivot_c;
    private Core core;
    private Cube ready_cube;
    private List<Cube> cubes;
    private string refColor, otherColor;
    private string bottomColor;
    private Cube reversed;

    public Stage3(Pivot_code p, Core c)
    {
        pivot_c = p;
        core = c;

        cubes = filterCubes(core.getCubes(2));
        bottomColor = core.getBottom().GetComponent<Cube>().getColors()[0];
        reversed = null;
    }

    public bool run()
    {
        if (pivot_c.hasTasks())
            return false;

        bool valid = true;

        if (ready_cube)
        {
            valid = false;

            string secondarySide = core.vec3ToStr(core.getColorSide(otherColor));
            List<string> sides = new List<string>();

            if (reversed != null)
            {
                sides = reversed.getAllSides();
                secondarySide = "down";

                reversed = null;
            } else
                sides = ready_cube.getAllSides();

            sides.Add(secondarySide);
            Cube refCube = core.getCornerCubeByCredentials(sides);

            if (refCube != null)
                refCube.alignCornerUp(refColor, otherColor, Core.var_type.AXIS);
            else Debug.Log("ERROR: Ref cube align color up for cube " + ready_cube);

            ready_cube = null;

            core.GetSolution().setAction(2);
        } else foreach (Cube cube in cubes)
            {
                if (cube.hasColor(bottomColor))
                    continue;

                List<string> colors = cube.getColors();

                refColor = colors[0];
                otherColor = colors[1];

                if (cube.transform.position.y < -0.5)
                {
                    valid = false;

                    Vector3 colorPos = cube.getColorRealPosition(colors[0]);
                    float dot = Vector3.Dot(colorPos.normalized, new Vector3(0, -1, 0));

                    if (dot > 0.7)
                    {
                        refColor = colors[1];
                        otherColor = colors[0];
                    }

                    cube.alignWithColor(refColor);
                    ready_cube = cube;

                    break;
                }
                else if (isCubeReversed(cube))
                {
                    valid = false;
                    reversed = cube;

                    List<string> sides = new List<string>();
                    sides.Add(cube.getSideFacing(refColor));
                    sides.Add("down");

                    Cube refCube = core.getCornerCubeByCredentials(sides);

                    if (refCube != null)
                        ready_cube = refCube;
                    else Debug.Log("ERROR: couldn't find 2 sided ready cube");

                    break;
                }
            }

        return valid;
    }

    private bool isCubeReversed(Cube cube)
    {
        return cube.getSideFacing(refColor).Equals(core.vec3ToStr(core.getColorSide(otherColor))) &&
               cube.getSideFacing(otherColor).Equals(core.vec3ToStr(core.getColorSide(refColor)));
    }

    private List<Cube> filterCubes(List<Cube> cubes)
    {
        List<Cube> ret = new List<Cube>();

        string top = core.getTop().getColors()[0];
        string bottom = core.getBottom().getColors()[0];

        foreach (Cube c in cubes)
            if (!c.hasColor(top) && !c.hasColor(bottom))
                ret.Add(c);

        return ret;
    }
}
