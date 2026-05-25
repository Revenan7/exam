using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootShop
{
    class DatabaseContext
    {
        private static DatabaseModels.Entities Entities;

        public static DatabaseModels.Entities GetContext()
        {
            if (Entities == null)
            {
                Entities = new DatabaseModels.Entities();
            }
            return Entities;
        }
    }
}
