using System;
using System.Collections.Generic;
using System.Text;
using Big3.Hitbase.SharedResources;

namespace Big3.Hitbase.DataBaseEngine
{
    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(int id)
        {
            ID = id;   
        }

        public int ID { get; set; }

        public override string Message
        {
            get
            {
                return string.Format(StringTable.ItemNotFoundException, ID);
            }
        }
    }
}
