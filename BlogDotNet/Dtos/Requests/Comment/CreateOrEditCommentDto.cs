using Newtonsoft.Json;

namespace BlogDotNet.Models.ViewModels.Requests.Comment
{
    public class CreateOrEditCommentDto
    {
        // [JsonProperty(PropertyName = "content")]
        public string Content { get; set; }
    }
}