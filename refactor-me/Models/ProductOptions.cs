using System.Collections.Generic;

namespace refactor_me.Models
{
    public class ProductOptions
    {
        public IEnumerable<ProductOption> Items { get; set; }

        public ProductOptions()
        {
            Items = new List<ProductOption>();
        }

        public ProductOptions(IEnumerable<ProductOption> productOptions)
        {
            Items = productOptions;
        }
    }
}