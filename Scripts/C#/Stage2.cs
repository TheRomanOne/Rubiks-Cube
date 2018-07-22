using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage2
{
    private Pivot_code pivot_c;
    private Core core;
    private Cube ready_cube;
    private Cube top_cube;
    private string otherColorOfReady;

    public Stage2(Pivot_code p, Core c)
    {
        pivot_c = p;
        core = c;

        top_cube = core.getTop().GetComponent<Cube>();
    }

    public bool run()
    {
        if (pivot_c.hasTasks())
            return false;

        string top_cube_color = top_cube.GetComponent<Cube>().getColors()[0];
        bool valid = true;

        List<Cube> cubes = core.getCubes(3, top_cube_color);

        if (ready_cube) //Cube ready to be brought up
        {
            valid = false;
            ready_cube.alignCornerUp(top_cube_color, otherColorOfReady, Core.var_type.COLOR);
            ready_cube = null;
        }
        else foreach (Cube cube in cubes)
            {
                List<string> colors = cube.getColors();
                colors.Remove(top_cube_color);

                Vector3 dirMain = cube.getColorRealPosition(top_cube_color).normalized;
                Vector3 dir1 = cube.getColorRealPosition(colors[0]).normalized;
                Vector3 dir2 = cube.getColorRealPosition(colors[1]).normalized;

                float alignMain = Vector3.Dot(dirMain, core.getColorSide(top_cube_color));
                float align1 = Vector3.Dot(dir1, core.getColorSide(colors[0]));
                float align2 = Vector3.Dot(dir2, core.getColorSide(colors[1]));

                if (alignMain < 0.7 || align1 < 0.7 || align2 < 0.7)
                {
                    valid = false;

                    if (alignMain > 0.7) // Color on top but others are not aligned 
                    {
                        Task task = cube.bring_down(colors[0]);
                        string side = task.getSide();
                        string sideFace = cube.getSideFacing(colors[0]);

                        int dir = 1;
                        if (!core.side_90_deg(sideFace, dir).Equals(side))
                            dir = -1;

                        core.setTask(new Task("down", dir));
                        core.setTask(new Task(side, -1 * task.getDirection()));
                    }
                    else if (cube.transform.position.y > 0.5) // Cube on top but none of the colors is aligned
                    {
                        Task task = cube.bring_down(top_cube_color);
                        string side = task.getSide();
                        string sideFace = cube.getSideFacing(top_cube_color);

                        int dir = 1;
                        if (core.side_90_deg(sideFace, dir).Equals(side))
                            dir = -1;

                        core.setTask(new Task("down", dir));
                        core.setTask(new Task(side, -1 * task.getDirection()));
                    }
                    else if (Vector3.Dot(dirMain, new Vector3(0, -1, 0)) > 0.7)
                    {
                        Task task = cube.bring_down(colors[0]);
                        string side = task.getSide();
                        string sideFace = cube.getSideFacing(colors[0]);

                        int dir = 1;
                        if (core.side_90_deg(sideFace, dir).Equals(side))
                            dir = -1;

                        core.setTask(new Task("down", dir));
                        core.setTask(new Task(side, -1 * task.getDirection()));
                    }
                    else
                    {
                        string refColor = "";

                        if (Vector3.Dot(cube.getColorRealPosition(colors[0]).normalized, new Vector3(0, -1, 0)) > 0.7)
                            refColor = colors[1];
                        else
                            refColor = colors[0];

                        otherColorOfReady = refColor;

                        cube.alignWithColor(refColor);

                        ready_cube = cube;
                    }
                }

                if (!valid)
                    break;
            }

        return valid;
    }
}
