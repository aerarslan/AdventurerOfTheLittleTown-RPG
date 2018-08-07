using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Item
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string NamePlural { get; set; }
        public int Price { get; set; }
        public int Weight { get; set; }

        public Item(int id, string name, string namePlural, int price, int weight)
        {
            ID = id;
            Name = name;
            NamePlural = namePlural;
            Price = price;
            Weight = weight;
        }
    }
}
