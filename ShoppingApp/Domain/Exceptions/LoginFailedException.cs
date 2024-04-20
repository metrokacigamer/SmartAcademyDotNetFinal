
namespace Domain.Exceptions
{
	public class LoginFailedException: Exception
	{
        public LoginFailedException(string message = "Failed to log in"): base(message)
        {
            
        }
    }
}
