using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync
{
    public class SyncheableData<T>
    {
        public T next { get; protected set; }
        public T current { get; protected set; }

        public SyncheableData(T start)
        {
            next = start;
            current = start;
        }

        public void update(T val)
        {
            next = val;
        }

        public void synch()
        {
            current = next;
        }

    }
}
