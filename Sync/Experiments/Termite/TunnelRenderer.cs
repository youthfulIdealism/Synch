using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sync.Experiments.Termite
{
    /**
     * Renderer for a given tunnel. Helps visualize pheremone spread.
     * */
    public class TunnelRenderer : RendererBase
    {
        public static Texture2D pixel;
        protected Tunnel tunnel;

        public TunnelRenderer(Tunnel tunnel)
        {
            this.tunnel = tunnel;
            if(pixel == null)
            {
                pixel = new Texture2D(Sync.instance.GraphicsDevice, 1, 1);
                pixel.SetData(new Color[] { Color.White });
            }
            
        }

        public override void draw(SpriteBatch batch, GameTime time, Vector2 offset, float scale)
        {
            //pheremones
            //Calculate color, given pheremones. No pheremones = brown. Prescence pheremone = white. Work pheremone = green. Dig pheremone = blue.
            Color color = Color.Brown;
            color = Color.Lerp(color, Color.White, Math.Min(tunnel.pheremones[Tunnel.PHEREMONE_PRESCENCE], .3f));
            color = Color.Lerp(color, Color.Green, tunnel.pheremones[Tunnel.PHEREMONE_WORK] * .5f);
            color = Color.Lerp(color, Color.Blue, tunnel.pheremones[Tunnel.PHEREMONE_DIG] * .5f);

            batch.Draw(pixel, RendererBase.getStandardRenderRect(tunnel, offset, scale), color);
            
        }
    }
}
