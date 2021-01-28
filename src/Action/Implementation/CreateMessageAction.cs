using System;
using System.Collections.Generic;
using MessagingService.Infrastructure;
using MessagingService.Model;

namespace MessagingService.Action
{
    public class CreateMessageGroupAction : ICreateMessageGroupAction
    {
        private readonly List<ICreateMessageGroup> _messageGroupCreators;
        private readonly List<ICreateMessageGroupAsync> _asyncMessageGroupCreators;

        public CreateMessageGroupAction(List<ICreateMessageGroup> messageGroupCreators, List<ICreateMessageGroupAsync> asyncMessageGroupCreators)
        {
            _messageGroupCreators = messageGroupCreators;
            _asyncMessageGroupCreators = asyncMessageGroupCreators;
        }

        public ProcessResult CreateMessageGroup(MessageGroupCreationContext context)
        {
            ProcessResult processResult = new ProcessResult { IsSuccessful = true };
            try
            {
                _messageGroupCreators.ForEach(msgc => msgc.CreateMessageGroup(context));
                _asyncMessageGroupCreators.ForEach(async (asyncmsgc) =>
                {
                    await asyncmsgc.CreateMessageGroup(context);
                });
            }
            catch (Exception ex)
            {
                processResult.IsSuccessful = false;
                processResult.Message = ex.Message;
            }

            return processResult;
        }
    }
}