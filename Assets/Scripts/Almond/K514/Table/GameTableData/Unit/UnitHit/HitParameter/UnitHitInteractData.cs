#if !SERVER_DRIVE
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    public class UnitHitInteractData : GameTable<UnitHitInteractData, int, UnitHitInteractData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public Dictionary<UnitTool.UnitSkinType, int> HitSkinFxMap { get; private set; }
            public Unit.UnitAttachPoint FxAttachPoint { get; private set; }
            public Vector3 FallbackSeedUV { get; private set; }
            
            /// <summary>
            /// 카메라 흔들림 인덱스
            /// </summary>
            public int CameraShakeIndex;
            
            /// <summary>
            /// 카메라 흔들림 선딜레이
            /// </summary>
            public uint CameraShakePreDelay;

            public UnitHitFxData.TableRecord this[UnitTool.UnitSkinType p_Type] =>
                UnitHitFxData.GetInstanceUnSafe[HitSkinFxMap[p_Type]];
            
            public override async UniTask OnRecordAdded()
            {
                await base.OnRecordAdded();

                var enumerator = SystemTool.GetEnumEnumerator<UnitTool.UnitSkinType>(SystemTool.GetEnumeratorType.GetAll);
                if (ReferenceEquals(null, HitSkinFxMap))
                {
                    HitSkinFxMap = new Dictionary<UnitTool.UnitSkinType, int>();
                    foreach (var skinType in enumerator)
                    {
                        HitSkinFxMap.Add(skinType, default);
                    }
                }
                else
                {
                    foreach (var skinType in enumerator)
                    {
                        if (!HitSkinFxMap.ContainsKey(skinType))
                        {
                            HitSkinFxMap.Add(skinType, default);
                        }
                    }
                }

                if (FallbackSeedUV == default)
                {
                    FallbackSeedUV = Vector3.forward;
                }
            }
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        protected override string GetDefaultTableFileName()
        {
            return "UnitHitInteractTable";
        }
    }
}
#endif
