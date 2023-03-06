using System;
using System.Collections.Generic;
using UnityEngine;

namespace k514
{
    public class UnitIKControl
    {
        #region <Consts>
        
        private const float ArcherFootAngle = 270;

        private const float KnightFootAngle = 500;

        private const float MagicianFootAngle = 270;

        #endregion
        
        #region <Fields>

        private float FootAngle;

        private LamiereUnit _MasterNode;
        
        private Transform _RootTransform;
        
        private Dictionary<UnitBoneTool.LamiereUnitIKBone, UnitBoneWrapper> _UnitBoneWrapperCollection;
        
        private List<Transform> _RemaindTransformSet;

        #endregion

        #region <Constructors>

        public UnitIKControl(LamiereUnit p_LamiereUnit)
        {
            _MasterNode = p_LamiereUnit;
            _RootTransform = _MasterNode._Transform;
            _UnitBoneWrapperCollection = new Dictionary<UnitBoneTool.LamiereUnitIKBone, UnitBoneWrapper>();
            _RemaindTransformSet = new List<Transform>();
            _RootTransform.GetComponentsInChildren(_RemaindTransformSet);

            var boneEnumerator = UnitBoneTool.GetInstance._LamiereUnitIKBoneEnumerator;
            foreach (var renderer in _RemaindTransformSet)
            {
                var boneTransform = renderer.transform;
                var boneTransformName = boneTransform.name;
                foreach (var unitBone in boneEnumerator)
                {
                    var symbol = $"{unitBone.ToString().Replace("_", " ")}";
                    if (boneTransformName.Contains(symbol))
                    {
                        if (!_UnitBoneWrapperCollection.TryGetValue(unitBone, out var targetBoneWrapper))
                        {
                            _UnitBoneWrapperCollection.Add(unitBone, targetBoneWrapper = new UnitBoneWrapper(unitBone, _MasterNode, boneTransform));
                        }
                    }
                }
            }
            SetFootAngle(_MasterNode.Vocation);
        }

        #endregion

        #region <Callbacks>
        
        public void UpdateCharacterBones()
        {
            if (_MasterNode.Vocation == Vocation.MONSTER) return;
            if(_UnitBoneWrapperCollection[UnitBoneTool.LamiereUnitIKBone.Pelvis].ReturnPosition() != Vector3.zero) _UnitBoneWrapperCollection[UnitBoneTool.LamiereUnitIKBone.Pelvis].ResetPosition();
            if (_MasterNode == null || !_MasterNode._ActableObject.IsIdleState()) return;
            
            RaycastHit left_foot_hit;
            RaycastHit right_foot_hit;

            //<todo BlackAm> : 발을 붙일 수 있는 최대 길이 까지 거리를 제한한다.
            if (Physics.Raycast(_UnitBoneWrapperCollection[UnitBoneTool.LamiereUnitIKBone.L_IK_Focus].ReturnPosition(), Vector3.down, out left_foot_hit, Mathf.Infinity) &&
                Physics.Raycast(_UnitBoneWrapperCollection[UnitBoneTool.LamiereUnitIKBone.R_IK_Focus].ReturnPosition(), Vector3.down, out right_foot_hit, Mathf.Infinity))
            {
#if !SERVER_DRIVE
                Debug.DrawLine(_UnitBoneWrapperCollection[UnitBoneTool.LamiereUnitIKBone.L_IK_Focus].ReturnPosition(), left_foot_hit.point, Color.green);
                Debug.DrawLine(_UnitBoneWrapperCollection[UnitBoneTool.LamiereUnitIKBone.R_IK_Focus].ReturnPosition(), right_foot_hit.point, Color.yellow);
                Debug.DrawLine(right_foot_hit.point ,left_foot_hit.point, Color.red);
#endif
                var l = Vector3.Distance(right_foot_hit.point, left_foot_hit.point);
                var h = right_foot_hit.point.y - left_foot_hit.point.y;
                //<todo BlackAm> : 캐릭터에 따라 다른 Angle 값을 사용한다.
                var angle = Mathf.Asin(h / l) * FootAngle / Mathf.PI;
                
                if (h > 0)
                {
                    _UnitBoneWrapperCollection[UnitBoneTool.LamiereUnitIKBone.Pelvis].LowerBody(h);
                    _UnitBoneWrapperCollection[UnitBoneTool.LamiereUnitIKBone.R_Thigh].RaiseBone(-angle);
                    _UnitBoneWrapperCollection[UnitBoneTool.LamiereUnitIKBone.RThighTwist].RaiseBone(-angle);
                    _UnitBoneWrapperCollection[UnitBoneTool.LamiereUnitIKBone.R_Calf].RaiseBone(angle);
                }
                else if (h < 0)
                {
                    _UnitBoneWrapperCollection[UnitBoneTool.LamiereUnitIKBone.Pelvis].LowerBody(-h);
                    _UnitBoneWrapperCollection[UnitBoneTool.LamiereUnitIKBone.L_Thigh].RaiseBone(angle);
                    _UnitBoneWrapperCollection[UnitBoneTool.LamiereUnitIKBone.LThighTwist].RaiseBone(angle);
                    _UnitBoneWrapperCollection[UnitBoneTool.LamiereUnitIKBone.L_Calf].RaiseBone(-angle);
                }
            }
        }

        #endregion

        #region <Methods>

        public void SetFootAngle(Vocation p_Vocation)
        {
            switch (p_Vocation)
            {
                case Vocation.ARCHER:
                    FootAngle = ArcherFootAngle;
                    break;
                case Vocation.KNIGHT:
                    FootAngle = KnightFootAngle;
                    break;
                case Vocation.MAGICIAN:
                    FootAngle = MagicianFootAngle;
                    break;
            }
        }

        #endregion
    }

    public class UnitBoneTool : Singleton<UnitBoneTool>
    {
        #region <Enums>

        public enum LamiereUnitIKBone
        {
            Pelvis,
            L_IK_Focus,
            R_IK_Focus,
            L_Thigh,
            R_Thigh,
            LThighTwist,
            RThighTwist,
            L_Calf,
            R_Calf,

        }

        public LamiereUnitIKBone[] _LamiereUnitIKBoneEnumerator;

        #endregion

        #region <Callbacks>

        protected override void OnCreated()
        {
            _LamiereUnitIKBoneEnumerator = SystemTool.GetEnumEnumerator<LamiereUnitIKBone>(SystemTool.GetEnumeratorType.GetAll);
        }

        public override void OnInitiate()
        {
        }

        #endregion
    }

    public class UnitBoneWrapper
    {
        private UnitBoneTool.LamiereUnitIKBone ThisBoneType;
        private Unit MasterNode;
        public Transform Wrapper;

        public UnitBoneWrapper(UnitBoneTool.LamiereUnitIKBone p_BoneType, Unit p_MasterNode, Transform p_Transform)
        {
            ThisBoneType = p_BoneType;
            MasterNode = p_MasterNode;
            Wrapper = p_Transform;

        }

        #region <Methods>

        public Vector3 ReturnPosition()
        {
            return Wrapper.position;
        }

        public void RaiseBone(float p_Angle)
        {
            Wrapper.Rotate(new Vector3(0,0,p_Angle));
        }

        public void LowerBody(float p_Height)
        {
            var downDir = Wrapper.right;
            Wrapper.position += downDir * p_Height;
        }

        public void ResetPosition()
        {
            Wrapper.localPosition = new Vector3(0, 0, 0);
        }

        #endregion
    }
}