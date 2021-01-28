using System;
using System.Collections.Generic;
using MessagingService.Infrastructure;
using MessagingService.Model;

namespace MessagingService.Action
{
    public class BlockUserAction : IBlockUserAction
    {
        private readonly List<IBlockUser> _userBlockers;
        private readonly List<IBlockUserAsync> _asyncUserBlockers;

        public BlockUserAction(List<IBlockUser> userBlockers, List<IBlockUserAsync> asyncUserBlockers)
        {
            _userBlockers = userBlockers;
            _asyncUserBlockers = asyncUserBlockers;
        }
        public ProcessResult BlockUser(UserBlockingContext context)
        {
            ProcessResult processResult = new ProcessResult { IsSuccessful = true };
            try
            {
                _userBlockers.ForEach(ub => ub.BlockUser(context));
                _asyncUserBlockers.ForEach(async (asyncub) =>
                {
                    await asyncub.BlockUser(context);
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