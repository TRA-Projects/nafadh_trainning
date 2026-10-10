using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
   public class ChatMessage
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime When { get; set; }
        public AppUser Sender { get; set; }
        public string  SenderId { get; set; }

        public AppUser Receiver { get; set; }
        public string ReceiverId { get; set; }

        public bool SeenBySender { get; set; }

        public bool SeenByReceiver { get; set; }

        public bool isText { get; set; }

        [NotMapped]
        public string EncryptedId { get; set; }

        [NotMapped]
        public string Document { get; set; }
        [NotMapped]
        public DocumentWrapper DocumentWrapper { get; set; }
        [NotMapped]
        public string MimeType { get; set; }

        [NotMapped]
        public int StartIndex { get; set; }


    }
}
