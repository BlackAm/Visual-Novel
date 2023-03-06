using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    public partial class CommandSystem
    {
        #region <Consts>

        

        #endregion
        
        #region <Fields>

        /// <summary>
        /// 비동기 작업을 취소하기 위한 토큰
        /// </summary>
        private CancellationTokenSource CancellationTokenSrc;

        #endregion
        
        #region <Methods>

        private void UpdateCancellationToken()
        {
            CancellationTokenSrc?.Cancel(false);
            CancellationTokenSrc = new CancellationTokenSource();
        }

        private async void HoldingKeyEventCooldownReq(int p_KeyState)
        {
            // 홀딩 키 쿨다운을 세트한다.
            HoldingCommandBlock.Add(p_KeyState);
            
            await UniTask.Delay(CommandCooldown, cancellationToken: CancellationTokenSrc.Token);
            
            HoldingCommandBlock.Remove(p_KeyState);
        }

        private async void PerspectiveArrowInputExpireReq(ArrowType p_LastTransitedArrowTypeDelta)
        {
            LastTransitedArrowTypeDeltaCompare = p_LastTransitedArrowTypeDelta;
            
            await UniTask.Delay(CommandExpireMsec, cancellationToken: CancellationTokenSrc.Token);
            
            LastTransitedArrowTypeDeltaCompare = default;
        }

        /// <summary>
        /// 커맨드 큐에 방향 커맨드를 등록하고, 일정시간 후에 등록한 커맨드를 삭제하게 하는 메서드
        /// </summary>
        private async void ArrowCommandExpireReq(int p_KeyCode)
        {
            StackedArrowKeyCodePressed.t_Current = p_KeyCode;
            CommandExpireReq(p_KeyCode);
            
            await UniTask.Delay(CommandExpireMsec, cancellationToken: CancellationTokenSrc.Token).SuppressCancellationThrow();

            StackedArrowKeyCodePressed = default;
        }
        
        /// <summary>
        /// 커맨드 큐에 특정 커맨드를 등록하고, 일정시간 후에 등록한 커맨드를 삭제하게 하는 메서드
        /// </summary>
        private async void CommandExpireReq(int p_KeyCode)
        {
            CommandQueue.Add(p_KeyCode);
            
            if (CommandQueue.Count > CommandMaxCapacity)
            {
                CommandQueue.RemoveAt(0);
            }
            else
            {
                await UniTask.Delay(CommandExpireMsec, cancellationToken: CancellationTokenSrc.Token).SuppressCancellationThrow();

                if (CommandQueue.Count > 0)
                {
                    CommandQueue.RemoveAt(0);
                }
            }
        }

        #endregion
    }
}