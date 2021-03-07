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
        public MessageGroupSaveContext(string processorUsername, string groupName, string updatedUsername, MessageGroupUpdateType updateType)
        {
            ProcessorUsername = processorUsername;
            TransactionType = TransactionType.Update;
            UpdateContext = new MessageGroupUpdateContext { GroupName = groupName, Username = updatedUsername, UpdateType = updateType };
        }
        public string MessageGroupId { get; set; }
    }

    public class MessageGroupCreationContext : MessageGroupProcessContext
    {
        public string AdminUsername { get; set; }
    }

    public class MessageGroupUpdateContext : MessageGroupProcessContext
    {
        public string Username { get; set; }
        public MessageGroupUpdateType UpdateType { get; set; }
    }

    public enum MessageGroupUpdateType
    {
        AdditionToGroup,
        EliminationFromGroup
    }

    public class MessageGroupProcessContext
    {
        public string GroupName { get; set; }
    }
}