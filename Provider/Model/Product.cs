using System;
namespace Provider.Model
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Price { get; set; }

        public Product(int id, string name, string type, string price)
        {
            this.Id = id;
            this.Name = name;
            this.Type = type;
            this.Price = price;
        }
    }
}
