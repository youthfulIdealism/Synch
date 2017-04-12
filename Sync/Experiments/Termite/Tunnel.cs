using Sync.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Sync.GameSpace;

namespace Sync.Experiments.Termite
{
    public class Tunnel : AgentBase
    {
        public WorldBase world;
        public float[] pheremones { get; private set; }

        public const int PHEREMONE_PRESCENCE = 0;
        const float PHEREMONE_PRESCENCE_deterioration = .9f;
        const float PHEREMONE_PRESCENCE_spread = .01f;

        public const int PHEREMONE_WORK = 1;
        const float PHEREMONE_WORK_deterioration = .97f;
        const float PHEREMONE_WORK_spread = .12f;

        public const int PHEREMONE_DIG = 2;
        const float PHEREMONE_DIG_deterioration = .97f;
        const float PHEREMONE_DIG_spread = .12f;


        public Tunnel(WorldBase world, Point currentLoc)
        {
            this.world = world;
            this.point = currentLoc;
            this.widthAndHeight = new Point(1, 1);
            pheremones = new float[3];
        }

        public override void synch()
        {
            for(int i = 0; i < pheremones.Length; i++)
            {
                if(pheremones[i] <= .05f)
                {
                    pheremones[i] = 0;
                }
            }
        }

        public override void update(GameTime time)
        {
            pheremones[PHEREMONE_PRESCENCE] *= PHEREMONE_PRESCENCE_deterioration;
            pheremones[PHEREMONE_DIG] *= PHEREMONE_DIG_deterioration;
            pheremones[PHEREMONE_WORK] *= PHEREMONE_WORK_deterioration;

            List<AgentBase> actionables = world.getConnectionsForAgent(this);
            List<AgentBase> tunnels = actionables.FindAll(x => x is Tunnel && !x.getPoint().Equals(this.point));
            //spread pheremones to nearby tunnels
            foreach(Tunnel t in tunnels)
            {

                //t.pheremones[PHEREMONE_PRESCENCE] += (pheremones[PHEREMONE_PRESCENCE] - t.pheremones[PHEREMONE_PRESCENCE]) * PHEREMONE_PRESCENCE_spread;
                //t.pheremones[PHEREMONE_DIG] += (pheremones[PHEREMONE_DIG] - t.pheremones[PHEREMONE_DIG]) * PHEREMONE_DIG_spread;
                //t.pheremones[PHEREMONE_WORK] += (pheremones[PHEREMONE_WORK] - t.pheremones[PHEREMONE_WORK]) * PHEREMONE_WORK_spread;
                //spread pheremones (ensuring that we don't accidentally reduce pheremone concentrations)
                t.pheremones[PHEREMONE_PRESCENCE] += Math.Max(pheremones[PHEREMONE_PRESCENCE] - t.pheremones[PHEREMONE_PRESCENCE], 0) * PHEREMONE_PRESCENCE_spread;
                t.pheremones[PHEREMONE_DIG] += Math.Max(pheremones[PHEREMONE_DIG] - t.pheremones[PHEREMONE_DIG], 0) * PHEREMONE_DIG_spread;
                t.pheremones[PHEREMONE_WORK] += Math.Max(pheremones[PHEREMONE_WORK] - t.pheremones[PHEREMONE_WORK], 0) * PHEREMONE_WORK_spread;
            }
        }

        
    }
}
