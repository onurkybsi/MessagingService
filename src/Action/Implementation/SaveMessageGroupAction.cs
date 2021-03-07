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

            ExecuteMessageGroupRecorders(context, processResult);

            ExecuteAsyncMessageGroupRecorders(context, processResult);

            return processResult;
        }

        private void ExecuteMessageGroupRecorders(MessageGroupSaveContext context, ProcessResult processResult)
        {
            if (!processResult.IsSuccessful)
                return;

            _messageGroupRecorders.ForEach(msgc =>
            {
                if (!processResult.IsSuccessful)
                    return;
                try
                {
                    msgc.SaveMessageGroup(context);
                }
                catch (System.Exception ex)
                {
                    SetExceptionOccuredResult(processResult, ex);
                    return;
                }
            });
        }

        private void ExecuteAsyncMessageGroupRecorders(MessageGroupSaveContext context, ProcessResult processResult)
        {
            if (!processResult.IsSuccessful)
                return;

            _asyncMessageGroupRecorders.ForEach(async (asyncmsgc) =>
            {
                if (!processResult.IsSuccessful)
                    return;
                try
                {
                    await asyncmsgc.SaveMessageGroup(context);
                }
                catch (System.Exception ex)
                {
                    SetExceptionOccuredResult(processResult, ex);
                    return;
                }
            });
        }

        private void SetExceptionOccuredResult(ProcessResult processResult, System.Exception ex)
        {
            processResult.IsSuccessful = false;
            processResult.Message = ex.Message;
        }
    }
}