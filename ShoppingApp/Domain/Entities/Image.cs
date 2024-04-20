namespace Domain.Entities
{
	public class Image
	{
        public string Id { get; set; }
		public string ImagePath { get; set; }
        public virtual Product Product { get; set; }
        public string ProductId { get; set; }
    }
}
