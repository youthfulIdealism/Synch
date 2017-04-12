using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sync.Experiments.FireFly
{
    /**
     * Renderer for a given firefly.
     * */
    public class FireFlyRenderer : RendererBase
    {
        public static Texture2D pixel;
        protected AgentFireFly firefly;

        public FireFlyRenderer(AgentFireFly firefly)
        {
            this.firefly = firefly;
            if(pixel == null)
            {
                pixel = new Texture2D(Sync.instance.GraphicsDevice, 1, 1);
                pixel.SetData(new Color[] { Color.White });
            }
            
        }

        public override void draw(SpriteBatch batch, GameTime time, Vector2 offset, float scale)
        {
            
            if (firefly.charge.current >= 1)
            {
                //if the firefly is flashing, draw it yellow.
                batch.Draw(pixel, RendererBase.getStandardRenderRect(firefly, offset, scale), Color.Yellow);
            }else
            {
                //if the firefly is not flashing, draw it a shade of grey. The whiter it is, the closer it is to flashing.
                batch.Draw(pixel, RendererBase.getStandardRenderRect(firefly, offset, scale), Color.White * firefly.charge.current * .5f);
            }
        }
    }
}
