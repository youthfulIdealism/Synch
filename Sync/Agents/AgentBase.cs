using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Agents
{
    public abstract class AgentBase
    {
        public RendererBase renderer { get; set; }
        protected Point point;
        protected Point widthAndHeight;

        public Point getPoint()
        {
            return point;
        }
        public Point getWidthAndHeight()
        {
            return widthAndHeight;
        }

        public abstract void update(GameTime time);
        public abstract void synch();
    }
}
