using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sync.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Experiments.Termite
{
    public class ColoredDotRenderer : RendererBase
    {
        public static Texture2D pixel;
        protected AgentBase agent;
        protected Color color;

        public ColoredDotRenderer(AgentBase agent, Color color)
        {
            this.agent = agent;
            this.color = color;
            if (pixel == null)
            {
                pixel = new Texture2D(Sync.instance.GraphicsDevice, 1, 1);
                pixel.SetData(new Color[] { Color.White });
            }

        }

        public override void draw(SpriteBatch batch, GameTime time, Vector2 offset, float scale)
        {
            batch.Draw(pixel, RendererBase.getStandardRenderRect(agent, offset, scale), color);
        }
    }
}
