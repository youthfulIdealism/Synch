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
    public class Termite : AgentBase
    {
        Random rand;
        public BlackBoardWorld world { get; set; }
        public enum State { WANDERING, DIGGING, HAULING, BUILDING};
        public State state;
        public Point lastLoc;

        public const int CMD_DIG = 0;
        public const int CMD_MOV = 1;
        public const int CMD_BUILD = 2;
        public const int CMD_SCENT = 3;

        public Termite(BlackBoardWorld world, Point currentLoc)
        {
            rand = new Random(currentLoc.X + 10 * currentLoc.Y);
            this.world = world;
            this.point = currentLoc;
            this.widthAndHeight = new Point(1, 1);
            state = State.WANDERING;
        }

        public void setPoint(Point p)
        {
            point = p;
        }

        public override void synch()
        {
            lastLoc = point;
        }

        public override void update(GameTime time)
        {
            List<AgentBase> actionables = world.getConnectionsForAgent(this);
            actionables.Shuffle(rand);
            
            switch (state)
            {
                //The wandering state will try to transition to any other state as rapidly as possible.
                case (State.WANDERING):
                    //check if there are enough tunnel segments to dig.
                    List<AgentBase> tunnelSegments = actionables.FindAll(x => x is Tunnel);
                    if (tunnelSegments.Count <= 3)
                    {
                        state = State.DIGGING;
                    }else
                    {
                        //otherwise, wander.
                        wander(tunnelSegments, 1, 4, 20);
                    }


                    

                    break;
                case (State.DIGGING):
                    Point[] walls = new Point[] { new Point(this.point.X + 1, this.point.Y), new Point(this.point.X - 1, this.point.Y), new Point(this.point.X , this.point.Y + 1), new Point(this.point.X, this.point.Y - 1) };
                    foreach(Point p in walls)
                    {
                        if(actionables.Find(x => x.getPoint().Equals(p)) == null)
                        {
                            if(world.interact(this, CMD_DIG, new int[] { p.X, p.Y}))
                            {
                                state = State.HAULING;
                                //world.interact(this, CMD_SCENT, new int[] { getPoint().X, getPoint().Y, Tunnel.PHEREMONE_WORK });
                                spreadScentsAdjacent(Tunnel.PHEREMONE_DIG);
                            }
                            break;
                        }
                    }
                    break;
                case (State.HAULING):
                    //if we're hauling material, try to get rid of the material.
                    List<AgentBase> tunnelSegments2 = actionables.FindAll(x => x is Tunnel);
                    if (tunnelSegments2.Count > 4)
                    {
                        state = State.BUILDING;
                    }
                    else
                    {
                        wander(tunnelSegments2, 1, 6, 0);
                        world.interact(this, CMD_SCENT, new int[] { getPoint().X, getPoint().Y, Tunnel.PHEREMONE_WORK });
                    }

                    break;
                case (State.BUILDING):
                    AgentBase tunnelSegmentPlace = actionables.Find(x => x is Tunnel && !x.getPoint().Equals(this.point));
                    if(tunnelSegmentPlace != null)
                    {
                        //try to build. If sucessful,
                        if(world.interact(this, CMD_BUILD, new int[] { tunnelSegmentPlace.getPoint().X, tunnelSegmentPlace.getPoint().Y }))
                        {
                            //spread the work pheremone to adjacent tiles. (the current tile won't work because when we're done building,
                            //there will no longer be a tunnel!)
                            spreadScentsAdjacent(Tunnel.PHEREMONE_WORK);
                            state = State.WANDERING;
                        }
                    }else
                    {
                        state = State.HAULING;
                    }
                    

                    break;
            }

            world.interact(this, CMD_SCENT, new int[] { getPoint().X, getPoint().Y, Tunnel.PHEREMONE_PRESCENCE });

            
        }

        private void spreadScentsAdjacent(int pheremone)
        {
            world.interact(this, CMD_SCENT, new int[] { getPoint().X - 1, getPoint().Y, pheremone });
            world.interact(this, CMD_SCENT, new int[] { getPoint().X, getPoint().Y - 1, pheremone });
            world.interact(this, CMD_SCENT, new int[] { getPoint().X + 1, getPoint().Y, pheremone });
            world.interact(this, CMD_SCENT, new int[] { getPoint().X, getPoint().Y + 1, pheremone });
        }

        /**
         * Finds a path to follow corresponding to nearby pheremones.
         * */
        public void wander(List<AgentBase> agents, int prescencePriority, int workPriority, int digPriority)
        {
            //Find all tunnels such that the tunnel isn't the tile currently occupied, and the tile isn't where the agent was last update.
            List<AgentBase> tunnels = agents.FindAll(x => x is Tunnel && !x.getPoint().Equals(this.point)/* && !x.getPoint().Equals(lastLoc)*/);
            
            if (tunnels.Count > 0)
            {
                
                if(point.Equals(lastLoc))//if the best position is wherever I just was, don't go there.
                {
                    AgentBase t = tunnels[rand.Next((int)(tunnels.Count))];
                    world.interact(this, CMD_MOV, new int[] { t.getPoint().X, t.getPoint().Y });
                }
                else
                {
                    //sort the available tunnel tiles, prioritizing via the arguments
                    tunnels.Sort(delegate (AgentBase x, AgentBase y)
                    {
                        Tunnel q = (Tunnel)y;
                        Tunnel w = (Tunnel)x;
                        float qaccum = 0;
                        float waccum = 0;
                        qaccum += q.pheremones[Tunnel.PHEREMONE_WORK] * workPriority;
                        qaccum += q.pheremones[Tunnel.PHEREMONE_PRESCENCE] * prescencePriority;
                        qaccum += q.pheremones[Tunnel.PHEREMONE_DIG] * digPriority;

                        waccum += w.pheremones[Tunnel.PHEREMONE_WORK] * workPriority;
                        waccum += w.pheremones[Tunnel.PHEREMONE_PRESCENCE] * prescencePriority;
                        waccum += w.pheremones[Tunnel.PHEREMONE_DIG] * digPriority;

                        return qaccum.CompareTo(waccum);
                    });
                    //pick the top tunnel
                    AgentBase t = tunnels[0];
                    world.interact(this, CMD_MOV, new int[] { t.getPoint().X, t.getPoint().Y });
                }

                
            }
        }


    }
}
