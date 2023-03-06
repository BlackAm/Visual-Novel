using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace k514
{
    public class SpriteSheetCarrier
    {
        // 유니티 컴포넌트의 경우에는 소멸자를 쓰지 말고, OnDestory를 통해 소멸자 메서드를 호출할 것
        ~SpriteSheetCarrier()
        {
            Dispose();
        }
        
        #region <Fields>

        private SpriteSheetLoadType LoadType;
        private string SpriteSheetPath;
        private AssetPreset SpriteAssetPreset;
        public Sprite[] SpriteSet { get; private set; }

        #endregion

        #region <Enums>

        private enum SpriteSheetLoadType
        {
            Unity_Resource_LoadAll,
            ResourceNameTable
        }

        #endregion
        
        #region <Constructors>

        public SpriteSheetCarrier(string p_AbsolutePath)
        {
            LoadType = SpriteSheetLoadType.Unity_Resource_LoadAll;

            SpriteSheetPath = p_AbsolutePath;
            SpriteSet = SystemTool.LoadAll<Sprite>(p_AbsolutePath);
        }

        public SpriteSheetCarrier((AssetPreset, Sprite[]) p_SpritePreset)
        {
            LoadType = SpriteSheetLoadType.ResourceNameTable;

            (SpriteAssetPreset, SpriteSet) = p_SpritePreset;
        }

        #endregion

        #region <Methods>

        public Sprite GetSprite(int p_Index)
        {
            return SpriteSet.GetElementSafe(p_Index);
        }

        public List<Sprite> SelectList(int p_Start, int p_End)
        {
            var startIndex = Mathf.Max(0, p_Start);
            var endIndex = p_End < 0 ? SpriteSet.Length - 1 :Mathf.Min(p_End, SpriteSet.Length - 1);
            var result = new List<Sprite>();
            for (int i = startIndex; i <= endIndex; i++)
            {
                result.Add(SpriteSet[i]);
            }

            return result;
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
        protected void DisposeUnManaged()
        {
            switch (LoadType)
            {
                case SpriteSheetLoadType.Unity_Resource_LoadAll:
                    SpriteSet = null;
                    break;
                case SpriteSheetLoadType.ResourceNameTable:
                    SpriteSet = null;
                    LoadAssetManager.GetInstanceUnSafe.UnloadAsset(SpriteAssetPreset);
                    SpriteAssetPreset = default;
                    break;
            }
        }

        #endregion
    }
}