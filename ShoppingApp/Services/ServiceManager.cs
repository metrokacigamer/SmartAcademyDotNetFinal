using Service.Abstactions;
using Domain.Repositories;

namespace Services
{
	public class ServiceManager: IServiceManager
	{
		public ServiceManager(IRepositoryManager repositoryManager) { }
	}
}
