namespace BookstoreSimulator.Contracts
{
    public class OrderRequest
    {
        public Guid BookId { get; set; }
        public int Quantaty { get; set; }
    }
}
