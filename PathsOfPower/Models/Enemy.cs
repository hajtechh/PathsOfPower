using PathsOfPower.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathsOfPower.Models
{
    public class Enemy : ICharacter
    {
        public string Name { get; set; }
        public int MaxHealthPoints { get; set; } 
        public int CurrentHealthPoints { get; set; }
        public int Power { get; set; }

        public void PerformAttack(ICharacter target)
        {
            target.CurrentHealthPoints -= Power;
        }
    }
}
