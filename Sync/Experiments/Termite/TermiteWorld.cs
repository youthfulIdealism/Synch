using Sync.GameSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Sync.Agents;
using System.Threading;

namespace Sync.Experiments.Termite
{

    /**
     * World for the termite experiments. The objective is to get the termites to act
     * in concert, builing a tunnel. Future extensions may involve retrieving food, 
     * raising young, etc.
     * 
     * Termites should be able to communicate by smelling pheremones, and should be able
     * to act upon the world.
     * 
     * Since the termites need to interact with the world, this extends BlackBoardWorld.
     * 
     * If you wanna try extending this, hooking it up to a neural net would be pretty cool.
     * The pheremone values and termite priorities are both things that could do with some
     * machine learning. Reccomended evaluation function is the depth of the deepest tunnel....
     * ...or, perhaps, the number of times termites place tunnels.
     * */
    public class TermiteWorld : BlackBoardWorld
    {
        public List<AgentBase> addedAgents;
        public List<AgentBase> removedAgents;
        public Random rand;
        public List<Termite> termites;
        public Termite[,] termiteGrid;
        public Tunnel[,] terrainGrid;
        public int dimensions;

        //slows down or speeds up simjulation so that the user can see what's going on.
        const int throttleMax = 3;
        int throttle = throttleMax;

        public TermiteWorld(int dimensions)
        {
            addedAgents = new List<AgentBase>();
            removedAgents = new List<AgentBase>();

            rand = new Random();
            this.dimensions = dimensions;
            termiteGrid = new Termite[dimensions, dimensions];
            terrainGrid = new Tunnel[dimensions, dimensions];

            //make an aboveground space occupying half the map
            int groundHeight = dimensions / 2;
            for(int x = 0; x < dimensions; x++)
            {
                for(int y = 0; y < groundHeight; y++)
                {
                    Tunnel tunnel = new Tunnel(this, new Point(x, y));
                    tunnel.renderer = new TunnelRenderer(tunnel);
                    terrainGrid[tunnel.getPoint().X, tunnel.getPoint().Y] = tunnel;
                    agents.Add(tunnel);
                }
            }

            //Make a belowground niche for the termites to spawn in
            int spawnTermitesX = rand.Next(dimensions - 4) + 2;
            int spawnTermitesY = groundHeight + 2;
            for (int x = -2; x <= 2; x++)
            {
                for (int y = -2; y <= 2; y++)
                {
                    Point potentialSpawn = new Point(spawnTermitesX + x, spawnTermitesY + y);
                    if (terrainGrid[potentialSpawn.X, potentialSpawn.Y] == null)
                    {
                        Tunnel tunnel = new Tunnel(this, potentialSpawn);
                        tunnel.renderer = new TunnelRenderer(tunnel);
                        terrainGrid[tunnel.getPoint().X, tunnel.getPoint().Y] = tunnel;
                        agents.Add(tunnel);
                    }
                    
                }
            }

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Termite termite = new Termite(this, new Point(spawnTermitesX + x, spawnTermitesY + y));
                    termite.renderer = new TermiteRenderer(termite);
                    termiteGrid[termite.getPoint().X, termite.getPoint().Y] = termite;
                    agents.Add(termite);


                }
            }

        }

        /**
         * Following the interface won't work, here, because we could be asking
         * for a termite, or for a tunnel.
         * */
        public override AgentBase getAgentAtPoint(Point point)
        {
            throw new Exception("multiple agents per point");
        }


        public List<AgentBase> getAgentsAtPoint(Point point)
        {
            List<AgentBase> agents = new List<AgentBase>();
            if(valid(point))
            {
                
                if (termiteGrid[point.X, point.Y] != null) { agents.Add(termiteGrid[point.X, point.Y]); }
                if (terrainGrid[point.X, point.Y] != null) { agents.Add(terrainGrid[point.X, point.Y]); }
            }
            return agents;
        }

        public override List<AgentBase> getConnectionsAtPoint(Point point)
        {
            List<AgentBase> connections = new List<AgentBase>();

            connections.AddRange(getAgentsAtPoint(new Point(point.X, point.Y)));
            connections.AddRange(getAgentsAtPoint(new Point(point.X - 1, point.Y)));
            connections.AddRange(getAgentsAtPoint(new Point(point.X + 1, point.Y)));
            connections.AddRange(getAgentsAtPoint(new Point(point.X, point.Y - 1)));
            connections.AddRange(getAgentsAtPoint(new Point(point.X, point.Y + 1)));
            return connections;
        }

        public override List<AgentBase> getConnectionsForAgent(AgentBase agent)
        {
            return getConnectionsAtPoint(agent.getPoint());
        }

        public override void update(GameTime time)
        {
            throttle--;
            if(throttle > 0)
            {
                return;
            }
            throttle = throttleMax;

            base.update(time);

            agents.AddRange(addedAgents);
            addedAgents.Clear();

            foreach(AgentBase agent in removedAgents)
            {
                agents.Remove(agent);
            }
            removedAgents.Clear();

        }

        /**
         * The termite can pass commands to the world, held in this struct.
         * 
         * */
        public struct TermiteCommand
        {
            public TermiteCommand(object termite1, int command, int[] args) : this()
            {
                this.termite = (Termite)termite1;
                this.command = command;
                this.args = args;
            }

            public Termite termite;
            public int command;
            private int[] args;
        }

        public override bool interact(AgentBase agent, int command, int[] args)
        {
            if(agent is Termite)
            {
                Termite termite = (Termite)agent;
                if(command == Termite.CMD_MOV)
                {
                    if(termiteGrid[args[0], args[1]] == null && terrainGrid[args[0], args[1]] != null)
                    {
                        Point moveto = new Point(args[0], args[1]);
                        
                        termiteGrid[agent.getPoint().X, agent.getPoint().Y] = null;
                        termiteGrid[moveto.X, moveto.Y] = termite;
                        termite.setPoint(moveto);
                        return true;
                    }
                }else if(command == Termite.CMD_DIG)
                {
                    Point digFrom = new Point(args[0], args[1]);
                    if (valid(digFrom) && terrainGrid[args[0], args[1]] == null)
                    {
                        

                        Tunnel tunnel = new Tunnel(this, digFrom);
                        terrainGrid[digFrom.X, digFrom.Y] = tunnel;
                        addedAgents.Add(tunnel);
                        tunnel.renderer = new TunnelRenderer(tunnel);
                        return true;
                    }
                }
                else if (command == Termite.CMD_BUILD)
                {
                    if (terrainGrid[args[0], args[1]] != null && termiteGrid[args[0], args[1]] == null)
                    {
                        Point placeAt = new Point(args[0], args[1]);

                        Tunnel removed = terrainGrid[placeAt.X, placeAt.Y];
                        removedAgents.Add(removed);
                        terrainGrid[placeAt.X, placeAt.Y] = null;
                        return true;
                    }
                }
                else if (command == Termite.CMD_SCENT)
                {
                    if (terrainGrid[args[0], args[1]] != null)
                    {
                        Point placeAt = new Point(args[0], args[1]);

                        Tunnel scented = terrainGrid[placeAt.X, placeAt.Y];
                        scented.pheremones[args[2]] += 1;
                        return true;
                    }
                }

            }
            return false;
        }

        

        public bool valid(int index)
        {
            return index < dimensions && index > 0;
        }

        public bool valid(Point index)
        {
            return valid(index.X) && valid(index.Y);
        }
    }
}
