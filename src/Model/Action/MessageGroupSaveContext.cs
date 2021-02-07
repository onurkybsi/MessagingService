namespace MessagingService.Model
{
    public class MessageGroupSaveContext
    {
        public SaveType SaveType { get; set; }
        public MessageGroupCreationContext? CreationContext { get; set; }
        public MessageGroupUpdateContext? UpdateContext { get; set; }
    }

    public class MessageGroupCreationContext
    {
        public string GroupName { get; set; }
        public string AdminUsername { get; set; }
    }

    public class MessageGroupUpdateContext
    {
        public string GroupName { get; set; }
        public string AddedUsername { get; set; }
    }
}