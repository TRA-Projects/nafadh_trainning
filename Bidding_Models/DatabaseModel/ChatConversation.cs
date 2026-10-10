using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel
{
    public class ChatConversation
    {
        public long Id { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastMessageCreationTime { get; set; }
        public int Party1ProfileId { get; set; }
        public int Party2ProfileId { get; set; }
        public bool IsClosed { get; set; }
        public bool HasNewMessageForParty1 { get; set; }
        public bool HasNewMessageForParty2 { get; set; }
        public Profile Party1Profile { get; set; }
        public Profile Party2Profile { get; set; }
        public string LastMessage { get; set; } = string.Empty;
        public ICollection<ChatConversationMessage> Messages { get; set; }

    }

    public class ChatConversationMessage
    {
        public long Id { get; set; }
        public DateTime CreationTime { get; set; }
        public long ChatConversationId { get; set; }

        public int SenderProfileId { get; set; }
        public int ReceiverProfileId { get; set; }
        public string MessageText { get; set; }
        public bool IsSeen { get; set; }
        public Profile SenderProfile { get; set; }
        public Profile ReceiverProfile { get; set; }

        public DateTime? ReadTime { get; set; }
    }

}
