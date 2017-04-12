using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sync.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync
{
    /**
     * Abstraction of renderable object. Ensures that drivers have
     * a standard interface by which to render.
     * */
    public abstract class RendererBase
    {
        public static Rectangle getStandardRenderRect(AgentBase agent, Vector2 offset, float scale)
        {
            float x = scale * (agent.getPoint().X + offset.X);
            float y = scale * (agent.getPoint().Y + offset.Y);
            float w = scale * agent.getWidthAndHeight().X;
            float h = scale * agent.getWidthAndHeight().Y;

            return new Rectangle((int)x, (int)y, (int)w, (int)h);
        }

        public abstract void draw(SpriteBatch batch, GameTime time, Vector2 offset, float scale);
    }
}
