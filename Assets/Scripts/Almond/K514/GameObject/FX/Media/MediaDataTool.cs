using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;

namespace k514
{
    public interface IIndexableMediaTableBridge : ITableBase
    {
    }

    public interface IIndexableMediaTableRecordBridge : ITableBaseRecord
    {
        List<string> NameList { get; }
        SystemTool.DataAccessType Type { get; }
        AudioManager.AudioPreset AudioPreset { get; }
    }

    public static class MediaTool
    {
        public interface IMediaTracker<Me2> where Me2 : Object
        {
#if !SERVER_DRIVE
            MediaPreset<Me2> MediaPreset { get; }
            void SetMediaPreset(MediaPreset<Me2> p_MediaPreset);
#endif
        }
        
        public struct MediaPreset<Me2> : _IDisposable where Me2 : Object
        {
            #region <Fields>

            public bool Valid { get; private set; }
            public int Index;
            public int ListIndex;
            private AssetPreset AssetPreset;
            private Me2 MediaAsset;
            public AudioManager.AudioPreset AudioPreset;
            
            #endregion

            #region <Constructor>

            public MediaPreset(Me2 p_Media, AudioManager.AudioPreset p_AudioPreset = default)
            {
                Index = default;
                ListIndex = default;
                AssetPreset = default;
                MediaAsset = p_Media;
                AudioPreset = p_AudioPreset;
                Valid = !ReferenceEquals(null, MediaAsset);
                IsDisposed = false;
            }
            
            public MediaPreset(int p_Index, int p_ListIndex, (AssetPreset, Me2) p_MediaAssetTuple, AudioManager.AudioPreset p_AudioPreset = default)
            {
                Index = p_Index;
                ListIndex = p_ListIndex;
                AssetPreset = p_MediaAssetTuple.Item1;
                MediaAsset = p_MediaAssetTuple.Item2;
                AudioPreset = p_AudioPreset;
                Valid = AssetPreset.IsValid;
                IsDisposed = false;
            }

            #endregion

            #region <Operator>

            public static implicit operator bool(MediaPreset<Me2> p_MediaPreset)
            {
                return p_MediaPreset.Valid;
            }
            
            public static implicit operator Me2(MediaPreset<Me2> p_MediaPreset)
            {
                return p_MediaPreset.MediaAsset;
            }

            #endregion

            #region <Disposable>
        
            /// <summary>
            /// dispose 패턴 onceFlag
            /// </summary>
            public bool IsDisposed { get; private set; }

            /// <summary>
            /// dispose 플래그를 초기화 시키는 메서드
            /// </summary>
            public void Rejunvenate()
            {
                IsDisposed = false;
            }

            /// <summary>
            /// 인스턴스 파기 메서드
            /// </summary>
            public void Dispose()
            {
                if (IsDisposed)
                {
                    return;
                }
                else
                {
                    IsDisposed = true;
                    DisposeUnManaged();
                }
            }

            /// <summary>
            /// 인스턴스가 파기될 때 수행할 작업을 기술한다.
            /// </summary>
            private void DisposeUnManaged()
            {
                Valid = default;
                Index = default;
                MediaAsset = default;
                LoadAssetManager.GetInstanceUnSafe.UnloadAsset(AssetPreset);
                AssetPreset = default;
            }

            #endregion
        }
    }

    public abstract class MediaDataRootBase<M, K, T, Me, Me2> : MultiTableProxy<M, int, K, T, Me> 
        where M :  MediaDataRootBase<M, K, T, Me, Me2>, new() 
        where K : struct 
        where T : IIndexableMediaTableBridge
        where Me : IIndexableMediaTableRecordBridge
        where Me2 : Object
    {
        #region <Fields>

        protected ResourceType _ResourceType;

        #endregion
        
        #region <Methods>

        protected override MultiTableIndexer<int, K, Me> SpawnGameDataTableCluster()
        {
            return new IntegerMultiTableIndexer<K, Me>();
        }

#if !SERVER_DRIVE
        public void PreloadMediaClip(int p_ClipIndex)
        {
            if (p_ClipIndex != default && GameDataTableCluster.GetTableData(p_ClipIndex, out var o_TargetRecord))
            {
                var NameList = o_TargetRecord.NameList;
                if (!ReferenceEquals(null, NameList))
                {
                    foreach (var soundSourceName in NameList)
                    {
                        LoadAssetManager.GetInstanceUnSafe.LoadAsset<Me2>(_ResourceType, ResourceLifeCycleType.Scene, soundSourceName);
                    }
                }
            }
        }
 
        public MediaTool.MediaPreset<Me2> GetMediaClip(int p_MediaIndex, MediaTool.MediaPreset<Me2> p_PrevMediaPreset, ResourceLifeCycleType p_RLCT = ResourceLifeCycleType.Scene)
        {
            if (p_MediaIndex != default && GameDataTableCluster.GetTableData(p_MediaIndex, out var o_TargetRecord))
            {
                var recordType = o_TargetRecord.Type;
                var NameList = o_TargetRecord.NameList;
                if (NameList.CheckGenericCollectionSafe())
                {
                    switch (recordType)
                    {
                        default:
                        case SystemTool.DataAccessType.Sequence:
                        {
                            var seqIndex = p_PrevMediaPreset.Index;
                            if (seqIndex > NameList.Count - 1)
                            {
                                seqIndex = 0;
                            }
                            var resultTuple = LoadAssetManager.GetInstanceUnSafe.LoadAsset<Me2>(_ResourceType,
                                p_RLCT, NameList[seqIndex++]);
                            return new MediaTool.MediaPreset<Me2>(p_MediaIndex, seqIndex, resultTuple, o_TargetRecord.AudioPreset);
                        }
                        case SystemTool.DataAccessType.Random:
                        {
                            var randIndex = NameList.GetRandom();
                            var resultTuple = LoadAssetManager.GetInstanceUnSafe.LoadAsset<Me2>(_ResourceType,
                                p_RLCT, NameList[randIndex]);
                            return new MediaTool.MediaPreset<Me2>(p_MediaIndex, randIndex, resultTuple, o_TargetRecord.AudioPreset);
                        }
                        case SystemTool.DataAccessType.RandomNotSame:
                        {
                            var randIndex = NameList.GetRandomExcept(p_PrevMediaPreset.Index);
                            var resultTuple = LoadAssetManager.GetInstanceUnSafe.LoadAsset<Me2>(_ResourceType,
                                p_RLCT, NameList[randIndex]);
                            return new MediaTool.MediaPreset<Me2>(p_MediaIndex, randIndex, resultTuple, o_TargetRecord.AudioPreset);
                        }
                    }
                }
                else
                {
                    return default;
                }
            }
            else
            {
                return default;
            }
        }
            
        public async UniTask<(bool, MediaTool.MediaPreset<Me2>)> GetMediaClipAsync(int p_MediaIndex, int p_PrevMediaIndex)
        {
            if (p_MediaIndex != default && GameDataTableCluster.GetTableData(p_MediaIndex, out var o_TargetRecord))
            {
                var recordType = o_TargetRecord.Type;
                var NameList = o_TargetRecord.NameList;
                if (NameList.CheckGenericCollectionSafe())
                {
                    switch (recordType)
                    {
                        default:
                        case SystemTool.DataAccessType.Sequence:
                        {
                            var seqIndex = p_PrevMediaIndex;
                            if (seqIndex > NameList.Count - 1)
                            {
                                seqIndex = 0;
                            }
                            var resultTuple = await LoadAssetManager.GetInstanceUnSafe.LoadAssetAsync<Me2>(_ResourceType,
                                ResourceLifeCycleType.Scene, NameList[seqIndex++]);
                            return (resultTuple.Item1.IsValid, new MediaTool.MediaPreset<Me2>(p_MediaIndex, seqIndex, resultTuple, o_TargetRecord.AudioPreset));
                        }
                        case SystemTool.DataAccessType.Random:
                        {
                            var randIndex = NameList.GetRandom();
                            var resultTuple = await LoadAssetManager.GetInstanceUnSafe.LoadAssetAsync<Me2>(_ResourceType,
                                ResourceLifeCycleType.Scene, NameList[randIndex]);
                            return (resultTuple.Item1.IsValid, new MediaTool.MediaPreset<Me2>(p_MediaIndex, randIndex, resultTuple, o_TargetRecord.AudioPreset));
                        }
                        case SystemTool.DataAccessType.RandomNotSame:
                        {
                            var randIndex = NameList.GetRandomExcept(p_PrevMediaIndex);
                            var resultTuple = await LoadAssetManager.GetInstanceUnSafe.LoadAssetAsync<Me2>(_ResourceType,
                                ResourceLifeCycleType.Scene, NameList[randIndex]);
                            return (resultTuple.Item1.IsValid, new MediaTool.MediaPreset<Me2>(p_MediaIndex, randIndex, resultTuple, o_TargetRecord.AudioPreset));
                        }
                    }
                }
                else
                {
                    return default;
                }
            }
            else
            {
                return default;
            }
        }
#endif
        #endregion
    }
}
