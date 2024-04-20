namespace Domain.Entities
{
	public class Item
	{
		public string Id { get; set; }
		public virtual Cart Cart { get; set; }
		public string CartId { get; set; }
        public virtual Product Product { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
