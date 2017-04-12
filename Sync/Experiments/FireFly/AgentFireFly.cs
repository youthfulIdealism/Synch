using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Sync.Agents;
using Sync.GameSpace;

namespace Sync.Experiments.FireFly
{
    /**
     * 
     * Represents a firefly with charge properties similar to those
     * described in Synch.
     * 
     * */
    public class AgentFireFly : AgentBase
    {
        public static Random random;
        public WorldBase world { get; set; }
        public SyncheableData<float> charge { get; private set; }

        public AgentFireFly(bool isRandom, WorldBase world, Point position)
        {
            if(isRandom)
            {
                if(random == null)
                {
                    random = new Random();
                }
                charge = new SyncheableData<float>((float)random.NextDouble());
            }else
            {
                charge = new SyncheableData<float>(0);
            }
            this.world = world;
            this.point = position;
            this.widthAndHeight = new Point(1, 1);
        }

        public override void synch()
        {
            charge.synch();
        }

        public override void update(GameTime time)
        {
            List<AgentBase> connections = world.getConnectionsForAgent(this);

            float increaseAmount = 0;

            foreach(AgentFireFly agent in connections)
            {
                //if a visible firefly is flashing, increase this agent's charge in order to synchronize with that firefly.
                if(agent.charge.current >= 1)
                {
                    increaseAmount += ((1 - charge.current) * .02f + .05f);
                }
            }

            //slowly charge.
            increaseAmount += ((1 - charge.current) * .0001f + .005f);
            charge.update(charge.current + increaseAmount);

            //reset on flash.
            if (charge.current > 1)
            {
                charge.update(0);
            }
        }
    }
}
