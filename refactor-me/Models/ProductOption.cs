namespace refactor_me.Models
{
    public partial class ProductOption
    {
        public System.Guid Id { get; set; }
        public System.Guid ProductId { private get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
