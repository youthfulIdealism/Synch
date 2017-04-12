using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sync.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.GameSpace
{
    /**
     * 
     * abstract representation of world space to ensure Sync can
     * run any simulations following the given pattern.
     * 
     * */
    public abstract class WorldBase
    {
        protected List<AgentBase> agents;

        public WorldBase()
        {
            agents = new List<AgentBase>();
        }

        /**
         * Retrieves the agent at a given point in current state
         * */
        public abstract AgentBase getAgentAtPoint(Point point);

        /**
         * Retrieves a list of agents connected to a given point in the current state
         * */
        public abstract List<AgentBase> getConnectionsAtPoint(Point point);

        /**
         * Retrieves a list of agents connected to a given agent in the current state
         * */
        public abstract List<AgentBase> getConnectionsForAgent(AgentBase agent);
        
        /**
         * Driver that calls the update and synch methods.
         * */
        public virtual void update(GameTime time)
        {
            foreach (AgentBase agent in agents)
            {
                agent.update(time);
            }
            foreach (AgentBase agent in agents)
            {
                agent.synch();
            }
        }

        /**
         * Draws the agents
         * */
        public virtual void draw(SpriteBatch batch, GameTime time, Vector2 offset, float scale)
        {
            foreach (AgentBase agent in agents)
            {
                agent.renderer.draw(batch, time, offset, scale);
            }
        }
    }
}
