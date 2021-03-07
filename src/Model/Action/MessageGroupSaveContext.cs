namespace MessagingService.Model
{
    public class MessageGroupSaveContext
    {
        public string ProcessorUsername { get; private set; }
        public TransactionType TransactionType { get; private set; }
        public MessageGroupCreationContext CreationContext { get; private set; }
        public MessageGroupSaveContext(string processorUsername, string groupName)
        {
            ProcessorUsername = processorUsername;
            TransactionType = TransactionType.Insert;
            CreationContext = new MessageGroupCreationContext { AdminUsername = processorUsername, GroupName = groupName };
        }
        public MessageGroupUpdateContext UpdateContext { get; private set; }
        public MessageGroupSaveContext(string processorUsername, string groupId, string updatedUsername, MessageGroupUpdateType updateType)
        {
            ProcessorUsername = processorUsername;
            TransactionType = TransactionType.Update;
            MessageGroupId = groupId;
            UpdateContext = new MessageGroupUpdateContext { MessageGroupId = groupId, Username = updatedUsername, UpdateType = updateType };
        }
        public string MessageGroupId { get; set; }
    }

    public class MessageGroupCreationContext
    {
        public string GroupName { get; set; }
        public string AdminUsername { get; set; }
    }

    public class MessageGroupUpdateContext
    {
        public string MessageGroupId { get; set; }
        public string Username { get; set; }
        public MessageGroupUpdateType UpdateType { get; set; }
    }

    public enum MessageGroupUpdateType
    {
        AdditionToGroup,
        EliminationFromGroup
    }
}