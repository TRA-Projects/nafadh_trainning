namespace Bidding_Models.DTOs
{
    public class SendMessageDto
    {
        public int SenderProfileId { get; set; }
        public int ReceiverProfileId { get; set; }
        public string Message { get; set; }
    }

}