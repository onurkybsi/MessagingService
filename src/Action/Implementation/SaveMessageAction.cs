using System;
using System.Collections.Generic;
using MessagingService.Infrastructure;
using MessagingService.Model;

namespace MessagingService.Action
{
    public class SaveMessageGroupAction : ISaveMessageGroupAction
    {
        private readonly List<ISaveMessageGroup> _messageGroupRecorders;
        private readonly List<ISaveMessageGroupAsync> _asyncMessageGroupRecorders;

        public SaveMessageGroupAction(List<ISaveMessageGroup> messageGroupRecorders, List<ISaveMessageGroupAsync> asyncMessageGroupRecorders)
        {
            _messageGroupRecorders = messageGroupRecorders;
            _asyncMessageGroupRecorders = asyncMessageGroupRecorders;
        }

        public ProcessResult SaveMessageGroup(MessageGroupSaveContext context)
        {
            ProcessResult processResult = new ProcessResult { IsSuccessful = true };
            try
            {
                _messageGroupRecorders?.ForEach(msgc => msgc.SaveMessageGroup(context));
                _asyncMessageGroupRecorders?.ForEach(async (asyncmsgc) =>
                {
                    await asyncmsgc.SaveMessageGroup(context);
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