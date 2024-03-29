﻿namespace Domain.Entities
{
	public class Product
	{
		public string Id { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public Enums.Category Catregory { get; set; }
        public virtual IEnumerable<Item> CartItems { get; set; }
        public virtual IEnumerable<Image> Images { get; set; }
    }
}
