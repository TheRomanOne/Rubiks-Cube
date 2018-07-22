using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1
{
    private Pivot_code pivot_c;
    private Core core;
    private Cube top_cube;

    private string top_cube_color;
    private int cross_action;
    private bool valid;
    private List<Cube> cubes;
    private List<Cube> sideColors;
    private Vector3 normal_self;
    private Vector3 normal_other;
    private Cube cube_c, cubeToDo;


    public Stage1(Pivot_code p, Core c)
    {
        pivot_c = p;
        core = c;

        top_cube = core.getTop().GetComponent<Cube>();
        top_cube_color = top_cube.GetComponent<Cube>().getColors()[0];

        cubes = core.getCubes(2, top_cube_color);
        sideColors = core.getCubesBySize(1);

        cubeToDo = null;
    }

    public bool run()
    {
        if (pivot_c.hasTasks())
            return false;

        normal_self = new Vector3();
        normal_other = new Vector3();
        cross_action = 0;
        valid = true;

        //Decide which action to take

        foreach (Cube cube in cubes)
        {
            cube_c = cubeToDo? cubeToDo:cube;
            normal_self = cube_c.getColorRealPosition(top_cube_color);

            List<string> rest_of_colors = cube_c.getColors();
            rest_of_colors.Remove(top_cube_color);
            normal_other = cube_c.getColorRealPosition(rest_of_colors[0]);
            Vector3 other_position = core.getColorSide(rest_of_colors[0]);
            float other_dot = Vector3.Dot(other_position.normalized, normal_other.normalized);

            bool cont = true;

            foreach (Cube norm in sideColors)
            {
                Vector3 normal = norm.getColorRealPosition(top_cube_color);

                float dot = Vector3.Dot(normal_self.normalized, normal.normalized);

                if (norm.hasColor(top_cube_color) && (dot < 0.8 || other_dot < 0.8))
                {
                    valid = false;

                    if(!cubeToDo)
                        cubeToDo = cube_c;

                    if (dot > 0.8 && other_dot < 0.8)
                    {
                        cross_action = 0;

                        string to_turn = core.vec3ToStr(normal_other);
                        core.setTask(new Task(to_turn, 1));
                        core.setTask(new Task(to_turn, 1));
                        core.setTask(new Task("down", 0));

                        cont = false; break;
                    }
                    else if (normal_self.y > 0.5)
                    {
                        cross_action = 0;

                        string side = core.vec3ToStr(normal_self);
                        string to_turn = core.side_90_deg(side, -1);
                        core.setTask(new Task(side, 1));
                        core.setTask(new Task(to_turn, 1));

                        cont = false; break;
                    }
                    else if (normal_self.y < -0.5)
                    {
                        if (Vector3.Dot(normal_self.normalized, new Vector3(0, -1, 0)) > 0.8)
                        {
                            //Cube is at the bottom
                            cross_action = 2;
                            cubeToDo = null;
                            cont = false; break;
                        }
                        else
                        {
                            //Cube at the bottom, color on the side
                            cross_action = 0;

                            string side1 = core.vec3ToStr(normal_self);
                            string side2 = core.side_90_deg(side1, -1);
                            core.setTask(new Task(side1, 1));
                            core.setTask(new Task(side2, -1));

                            cont = false; break;
                        }
                    }
                    else
                    {
                        string color_1 = "";

                        foreach (string c in cube_c.getColors())
                            if (c != top_cube_color)
                                color_1 = c;
                        Vector3 p = cube_c.getColorRealPosition(color_1);
                        Vector3 color_p = core.getColorSide(color_1);
                        float dott = Vector3.Dot(p.normalized, color_p.normalized);

                        if (dott > 0.8)
                        {
                            cube_c.bring_to(top_cube_color, "up");
                            cross_action = 2;
                        }
                        else
                        {
                            cross_action = 0;

                            cube_c.bring_to(top_cube_color, "down");
                            core.setTask(new Task("down", 0));
                        }

                        cont = false;
                        break;
                    }
                }
            }

            if (!cont)
                break;
        }

        switch (cross_action)
        {
            case 1: //Color on the side
                string color_1 = "";

                foreach (string c in cube_c.getColors())
                    if (c != top_cube_color)
                        color_1 = c;
                Vector3 p_1 = cube_c.getColorRealPosition(color_1);
                Vector3 color_p_1 = core.getColorSide(color_1);

                Vector3 mult_1 = Vector3.Scale(p_1, color_p_1).normalized;
                if (mult_1.Equals(new Vector3(1, 0, 0)) ||
                    mult_1.Equals(new Vector3(0, 1, 0)) ||
                    mult_1.Equals(new Vector3(0, 0, 1)))
                    core.setTask(new Task(core.vec3ToStr(color_p_1), 1));
                else
                {
                    Vector3 main_color_pos = cube_c.getColorRealPosition(top_cube_color);
                    mult_1 = Vector3.Scale(color_p_1, main_color_pos).normalized;
                    if (mult_1.Equals(new Vector3(1, 0, 0)) ||
                    mult_1.Equals(new Vector3(0, 1, 0)) ||
                    mult_1.Equals(new Vector3(0, 0, 1)))
                        core.setTask(new Task(core.vec3ToStr(p_1), 1));
                    else
                    {
                        core.setTask(new Task(core.vec3ToStr(p_1), 1));
                    }
                }

                break;

            case 2: //Color at the bottom
                string color_2 = "";

                foreach (string c in cube_c.getColors())
                    if (c != top_cube_color)
                        color_2 = c;
                Vector3 p_2 = cube_c.getColorRealPosition(color_2);
                Vector3 color_p_2 = core.getColorSide(color_2);

                // Check if colors point to the same side

                Vector3 mult_2 = Vector3.Scale(p_2, color_p_2).normalized;
                if (!mult_2.Equals(new Vector3(1, 0, 0)) &&
                    !mult_2.Equals(new Vector3(0, 1, 0)) &&
                    !mult_2.Equals(new Vector3(0, 0, 1)))
                    core.setTask(new Task("down", 1));
                else
                {
                    core.setTask(new Task(core.vec3ToStr(color_p_2), 1));
                    core.setTask(new Task(core.vec3ToStr(color_p_2), 1));
                    cubeToDo = null;
                }
                break;
        }
        return valid;
    }
}
