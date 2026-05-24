using System.Net;
using System.Text.Json.Serialization;

namespace PhoneHub.Core.CustomEntities
{
    public class ResponseData
    {
        public PagedList<object> Pagination { get; set; } = null!;
        public Message[] Messages { get; set; } = null!;
        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; }
    }
}
