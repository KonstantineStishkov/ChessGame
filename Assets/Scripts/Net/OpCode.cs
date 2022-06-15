using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Net
{
    public enum OpCode
    {
        KEEP_ALIVE = 1,
        WELCOME = 2,
        START_GAME = 3,
        MAKE_MOVE = 4,
        REMATCH = 5,
    }
}
