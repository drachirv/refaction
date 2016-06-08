namespace refactor_me.Models
{
    using System.Collections.Generic;
    
    public partial class Product
    {
        public Product()
        {
            this.ProductOptions = new HashSet<ProductOption>();
        }
    
        public System.Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal DeliveryPrice { get; set; }
    
        public virtual ICollection<ProductOption> ProductOptions { internal get; set; }
    }
}
