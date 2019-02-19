using BlogDotNet.Models;

namespace BlogDotNet.Dtos.Responses
{
    public abstract class PagedDto : SuccessResponse
    {
        public PageMeta PageMeta { get; set; }
    }
}