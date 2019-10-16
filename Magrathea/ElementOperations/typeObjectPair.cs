using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magrathea.ElementOperations
{
    public class typeObjectPair
    {
        public String Cast { get; set; }
        public Object Value { get; set; }

        public typeObjectPair(String cast, Object value)
        {
            this.Cast = cast;
            this.Value = value;
        }

        public typeObjectPair()
        {
        }       

        public override string ToString()
        {
            String toReturn = "";
            toReturn = Cast + ":" + Value.ToString();
            return toReturn;
        }

    }
}
