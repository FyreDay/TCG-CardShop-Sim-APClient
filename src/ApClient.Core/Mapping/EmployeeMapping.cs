using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApClient.mapping
{
    public class EmployeeMapping
    {

        private const long ITEMS_WORKER_OFFSET = 8000;

        public static long GetItemId(int workerIndex)
        {
            if (workerIndex < 0 || workerIndex > 8)
                throw new ArgumentOutOfRangeException(nameof(workerIndex));

            return 8000L + workerIndex;
        }
        public static int getindexFromId(long itemid)
        {
            if (itemid < ITEMS_WORKER_OFFSET || itemid > ITEMS_WORKER_OFFSET + 8)
                return -1;
            return (int)(itemid - ITEMS_WORKER_OFFSET);
        }
    }
}
