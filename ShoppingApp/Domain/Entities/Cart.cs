namespace Domain.Entities
{
	public class Cart
	{
        public string Id { get; set; }
        public virtual AppUser? User { get; set; }
        public string? UserId{ get; set; }
        public virtual IEnumerable<Item>? Items { get; set; }
    }
}
