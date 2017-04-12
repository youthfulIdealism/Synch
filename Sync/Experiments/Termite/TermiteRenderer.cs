using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sync.Experiments.Termite
{
    public class TermiteRenderer : RendererBase
    {
        public static Texture2D pixel;
        protected Termite termite;

        public TermiteRenderer(Termite termite)
        {
            this.termite = termite;
            if (pixel == null)
            {
                pixel = new Texture2D(Sync.instance.GraphicsDevice, 1, 1);
                pixel.SetData(new Color[] { Color.White });
            }

        }

        public override void draw(SpriteBatch batch, GameTime time, Vector2 offset, float scale)
        {
            Color color = Color.Yellow;
            if(termite.state == Termite.State.HAULING)
            {
                color = Color.DarkOrange;
            }else if (termite.state == Termite.State.BUILDING)
            {
                color = Color.Red;
            }

            batch.Draw(pixel, RendererBase.getStandardRenderRect(termite, offset, scale), color);
        }
    }
}
