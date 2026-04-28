using System.Net;

namespace PhoneHub.Core.Exceptions
{
    public class NotFoundException : BusinessException
    {
        public NotFoundException(string message)
            : base(message, HttpStatusCode.NotFound)
        {
        }
    }
}
