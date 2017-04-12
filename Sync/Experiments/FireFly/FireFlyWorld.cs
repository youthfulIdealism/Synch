using Sync.GameSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Sync.Agents;
using Microsoft.Xna.Framework.Graphics;

namespace Sync.Experiments.FireFly
{
    /**
     * 
     * World to drive a firefly simulation, adapted from
     * the model described in Sync by Steven Strogatz.
     * 
     * The model described in the book has each 'firefly'
     * influencinge each other firefly in the problem space.
     * My variation has each firefly influencing only its neighbors.
     * The result is not synchronous flashing as described in
     * the book, but rather eventually culminates in a 'wave'.
     * 
     * If you'd like to play with this model, my reccomendation would
     * be to extend the influence of a given firefly by modifying the
     * getConnectionsAtPoint function. It would be interesting to see
     * if flashing in synchrony resulted from wider ranges of influence.
     * 
     * It would also be reasonable to make a modification such that
     * the fireflies followed a 'small world network' pattern: connect
     * N percent of fireflies (such that N is small) to another firefly
     * somewhere on the map, and see if the synchrony is increased.
     * 
     * */
    public class FireFlyWorld : WorldBase
    {
        protected int dimensions;
        protected AgentBase[,] agentGrid;

        public FireFlyWorld(int dimensions) : base()
        {
            this.dimensions = dimensions;
            agentGrid = new AgentBase[dimensions, dimensions];

            for(int x = 0; x < dimensions; x++)
            {
                for(int y = 0; y < dimensions; y++)
                {
                    agentGrid[x, y] = new AgentFireFly(true, this, new Point(x, y));
                    agentGrid[x, y].renderer = new FireFlyRenderer((AgentFireFly)agentGrid[x, y]);
                    agents.Add(agentGrid[x, y]);
                }
            }
        }

        public override AgentBase getAgentAtPoint(Point point)
        {
            return agentGrid[point.X, point.Y];
        }

        /*
         * 
         * Each firefly is connected to its neighbor on the toroidal grid. Thus, retrieving
         * the connections involves fetching the neighbors.
         * 
         * */
        public override List<AgentBase> getConnectionsAtPoint(Point point)
        {
            List<AgentBase> connections = new List<AgentBase>();
            
            connections.Add(agentGrid[wrap(point.X + -1, dimensions), wrap(point.Y + -1, dimensions)]);
            connections.Add(agentGrid[wrap(point.X + 0, dimensions), wrap(point.Y + -1, dimensions)]);
            connections.Add(agentGrid[wrap(point.X + 1, dimensions), wrap(point.Y + -1, dimensions)]);

            connections.Add(agentGrid[wrap(point.X + -1, dimensions), wrap(point.Y + 0, dimensions)]);
            connections.Add(agentGrid[wrap(point.X + 0, dimensions), wrap(point.Y + 0, dimensions)]);
            connections.Add(agentGrid[wrap(point.X + 1, dimensions), wrap(point.Y + 0, dimensions)]);

            connections.Add(agentGrid[wrap(point.X + -1, dimensions), wrap(point.Y + 1, dimensions)]);
            connections.Add(agentGrid[wrap(point.X + 0, dimensions), wrap(point.Y + 1, dimensions)]);
            connections.Add(agentGrid[wrap(point.X + 1, dimensions), wrap(point.Y + 1, dimensions)]);

            return connections;
        }

        public override List<AgentBase> getConnectionsForAgent(AgentBase agent)
        {
            return getConnectionsAtPoint(agent.getPoint());
        }


        private int wrap(int index, int maxSize)
        {
            while (index < 0)
            {
                index += maxSize;
            }
            return index % maxSize;
        }
    }
}
