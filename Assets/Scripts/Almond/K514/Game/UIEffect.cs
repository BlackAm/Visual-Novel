#if !SERVER_DRIVE
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using k514;


namespace UI2020
{
    public class UIEffect : AbstractUI
    {
        public List<VFXUnit> _UiEffects;

        public override void OnSpawning()
        {
            
        }

        public void OnSceneTerminated()
        {
            _UiEffects.Clear();
        }

        public void Initialize(){
            _UiEffects = new List<VFXUnit>();
            _Transform = gameObject.transform;
        }

        //조건 : _UiEffectsystem/Renderer/SortingLayer는 DepthLower부터 설정
        public (bool, VFXUnit) SpawnUIEffect(int p_Index, Vector3 p_Pos , uint p_PreDelay = 0){
            var data = UIParticleEffectData.GetInstanceUnSafe[p_Index];
            
            var (isValid, spawned) = VfxSpawnManager.GetInstance.CastVfx<VFXUnit>(data.VfxEffect, data.Position + p_Pos,
                ResourceLifeCycleType.Scene, ObjectDeployTool.ObjectDeploySurfaceDeployType.None, p_PreDelay, false);

            //이펙트의 레이어는 무조건 UI로 설정함
            spawned.gameObject.TurnLayerTo(GameManager.GameLayerType.UI, true);

            if (isValid)
            {
                spawned.SetAttach(_Transform);
                spawned.SetPlay(p_PreDelay);
                _UiEffects.Add(spawned);
            }

            return (isValid, spawned);
        }

        ///지정된 UI이펙트 회수
        public void RetrieveUIEffect(VFXUnit p_VFX){
            if(_UiEffects.Contains(p_VFX)){
                if(p_VFX.IsValid()) p_VFX.RetrieveObject();
                _UiEffects.Remove(p_VFX);
            }
        }

        ///루프가 아닌 이펙트는 자동 리스트 제거 처리함
        public void RemoveListValue(VFXUnit p_VFX) {
            if(_UiEffects.Contains(p_VFX)) _UiEffects.Remove(p_VFX);
        }

        ///UI이펙트 모두 회수
        public void RetrieveAllUIEffect(){
            foreach (var effect in _UiEffects)
            {
                if(effect.IsValid()) effect.RetrieveObject();
            }

            _UiEffects.Clear();
        }
    }
}
#endif