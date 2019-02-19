namespace BlogDotNet.Dtos.Responses
{
    public class SuccessResponse : AppResponse
    {
        public SuccessResponse() : base(true)
        {
        }

        public SuccessResponse(string message) : base(true, message)
        {
        }
    }
}