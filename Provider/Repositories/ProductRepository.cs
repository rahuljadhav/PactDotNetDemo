using Provider.Model;


namespace Provider.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private List<Product> State { get; set; }

        public ProductRepository()
        {
            State = new List<Product>()
            {
                new Product(1, "Electronics", "Laptop", "$ 300"),
                new Product(2, "Eduactional", "Notebook", "$ 3"),
                new Product(3, "Eduactional", "Paintbox", "$ 5")
            };
        }

        public void SetState(List<Product> state)
        {
            this.State = state;
        }

        List<Product> IProductRepository.List()
        {
            return State;
        }

        public Product Get(int id)
        {
            return State.Find(p => p.Id == id);
        }
    }
}
