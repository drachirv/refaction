using System.Collections.Generic;

namespace refactor_me.Models
{
    public class Products
    {
        public IEnumerable<Product> Items { get; set; }

        public Products()
        {
            Items = new List<Product>();
        }

        public Products(IEnumerable<Product> products)
        {
            Items = products;
        }
    }
}