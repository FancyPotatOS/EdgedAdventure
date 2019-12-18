using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdgedAdventure
{
    public class SaveEntity
    {

        public readonly Entity entity;
        public readonly int[] destination;

        public SaveEntity(Entity e, int[] des)
        {
            entity = e;
            destination = des;
        }

    }
}
