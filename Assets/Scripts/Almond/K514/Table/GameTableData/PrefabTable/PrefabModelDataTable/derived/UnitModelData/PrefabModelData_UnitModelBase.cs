using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    public abstract class PrefabModelData_UnitModelBase<M, T> : PrefabModelDataIntTable<M, T> where M : PrefabModelData_UnitModelBase<M, T>, new() where T : PrefabModelData_UnitModelBase<M, T>.UnitBaseTableRecord, new()
    {
        public class UnitBaseTableRecord : PrefabModelDataRecord, UnitModelDataRecordBridge
        {
            public int AttachPointIndex { get; protected set; }
            
            /* 컨트롤러 초기화 실패시 사용하는 Fallback 필드 */
            /// <summary>
            /// 반경
            /// </summary>
            public float FallbackRadius { get; protected set; }
            
            /// <summary>
            /// 높이
            /// </summary>
            public float FallbackHeights { get; protected set; }
            
            /// <summary>
            /// 중심 옵셋
            /// </summary>
            public Vector3 FallbackCenterOffset { get; protected set; }

            /// <summary>
            /// 스킨 타입
            /// </summary>
            public UnitTool.UnitSkinType UnitSkinType { get; protected set; }

            public override async UniTask OnRecordAdded()
            {
                await base.OnRecordAdded();

                if (FallbackRadius == default)
                {
                    FallbackRadius = 1f;
                }
                
                if (FallbackHeights == default)
                {
                    FallbackHeights = 2f;
                }
                
                if (FallbackCenterOffset == default)
                {
                    FallbackCenterOffset = Vector3.up;
                }
                
                if (UnitSkinType == default)
                {
                    UnitSkinType = UnitTool.UnitSkinType.Leather;
                }
            }
        }
    }
}