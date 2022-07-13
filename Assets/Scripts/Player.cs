using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public string Name { get; set; }
    public int Experience { get; set; }


    public int Level
    {
        get
        {
            if (Experience < 11)
                return 0;
            if (Experience < 101)
                return 1;
            if (Experience < 501)
                return 2;
            if (Experience < 1001)
                return 3;
            if (Experience < 3001)
                return 4;

            return 5;
        }
    }
}
