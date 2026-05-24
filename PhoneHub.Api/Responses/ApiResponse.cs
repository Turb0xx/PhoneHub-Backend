using PhoneHub.Core.CustomEntities;

namespace PhoneHub.Api.Responses
{
    public class ApiResponse<T>
    {
        public T Data { get; set; }
        public Pagination? Pagination { get; set; }
        public Message[]? Messages { get; set; }

        public ApiResponse(T data)
        {
            Data = data;
        }
    }
}
