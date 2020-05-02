using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Configurations
{
    [Serializable]
    public class PlayerStats
    {
        public int AllowedBombs = 1;
        public int ActiveBombs;
        public int BombSize;
        public int Lifes = 1; 
        public bool IsDead = false;
    }
}
