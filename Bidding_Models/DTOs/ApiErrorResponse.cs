namespace Bidding_Models.DTOs
{
    public class ApiErrorResponse
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public object Data { get; set; }
    }
    
}