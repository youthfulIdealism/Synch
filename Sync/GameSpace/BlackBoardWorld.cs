using Sync.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.GameSpace
{
    public abstract class BlackBoardWorld : WorldBase
    {
        public abstract bool interact(AgentBase agent, int command, int[] args);
    }
}
