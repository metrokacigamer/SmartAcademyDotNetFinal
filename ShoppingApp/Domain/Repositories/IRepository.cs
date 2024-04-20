namespace Domain.Repositories
{
	public interface IRepository<T> where T : class
	{
		Task<IEnumerable<T>> GetAllAsync();
		Task<T> GetByIdAsync(string id);
		void Create(T entity);
		void Update(T entity);
		void Delete(T entity);
	}
}