using System;
using UnityEngine;

namespace k514
{
    /// <summary>
    /// 유닛 간의 관계를 기술하는 인터페이스
    /// </summary>
    public interface IInstanceNode<R>
    {
        /// <summary>
        /// 노드 유닛
        /// </summary>
        Unit Node { get; }
        
        /// <summary>
        /// 관계 타입
        /// </summary>
        R RelateType { get; }
        
        /// <summary>
        /// 종속 유닛을 세트하도록 구현
        /// </summary>
        void SetNode(R p_MasterNodeRelateType, Unit p_Node);
    }

    /// <summary>
    /// 유닛 간의 관계를 기술하는 기저 클래스
    /// </summary>
    public abstract class UnitNode<R> : IInstanceNode<R>
    {
        #region <Fields>
        
        /// <summary>
        /// 관계 유닛, 노드
        /// </summary>
        public Unit Node { get; protected set; }
        
        /// <summary>
        /// 관계 타입
        /// </summary>
        public R RelateType { get; protected set; }

        /// <summary>
        /// 특정 타입의 노드의 등록만 허용시키는 플래그 마스크
        /// </summary>
        protected R RestrictRelationFlagMask;

        /// <summary>
        /// 해당 노드와 통신중인 유닛
        /// </summary>
        protected PrefabInstance Handler;
        
        /// <summary>
        /// 노드 유닛 전이시 처리할 이벤트
        /// </summary>
        protected Action<Unit, Unit> OnNodeTransition;
        
        #endregion

        #region <Constructor>

        public UnitNode(PrefabInstance p_Handler, Action<Unit, Unit> p_OnNodeTransitionEvent)
        {
            Handler = p_Handler;
            OnNodeTransition = p_OnNodeTransitionEvent;
        }

        #endregion

        #region <Operator>

        public static implicit operator Unit(UnitNode<R> p_Node)
        {
            return p_Node.Node;
        }

        public static implicit operator Transform(UnitNode<R> p_Node)
        {
            return p_Node.Node._Transform;
        }
        
        public static implicit operator GameObject(UnitNode<R> p_Node)
        {
            return p_Node.Node.gameObject;
        }
        
        public static implicit operator Vector3(UnitNode<R> p_Node)
        {
            return p_Node.Node._Transform.position;
        }

        public static implicit operator bool(UnitNode<R> p_Node)
        {
            return p_Node.CheckNode();
        }
        
        #endregion
        
        #region <Methods>

        public abstract void SetNode(R p_TryRelateType, Unit p_Node);
        public abstract void AddRestrictFlag(R p_FlagMask);
        public abstract void RemoveRestrictFlag(R p_FlagMask);
        
        public bool IsNodeEquals(Unit p_Unit)
        {
            return ReferenceEquals(Node, p_Unit);
        }
        
        public bool CheckNode()
        {
            if (Node.IsValid())
            {
                return true;
            }
            else
            {
                ClearNode();

                return false;
            }
        }

        public bool CheckNode(Unit.UnitStateType p_FilterMask)
        {
            if (Node.IsInteractValid(p_FilterMask))
            {
                return true;
            }
            else
            {
                ClearNode();

                return false;
            }
        }

        public abstract bool CheckNode(R p_Type);
        public abstract bool CheckNode(R p_Type, Unit.UnitStateType p_FilterMask);
        public abstract void ClearNode();
        public abstract void ClearNode(R p_Type);

        public void ResetNode()
        {
            RestrictRelationFlagMask = default;
            ClearNode();
        }

        #endregion
    }

    public class MasterNode : UnitNode<PrefabInstanceTool.MasterNodeRelateType>
    {
        #region <Constructor>

        public MasterNode(PrefabInstance p_Handler, Action<Unit, Unit> p_OnNodeTransitionEvent) : base(p_Handler, p_OnNodeTransitionEvent)
        {
        }

        #endregion

        #region <Methods>

        public override void SetNode(PrefabInstanceTool.MasterNodeRelateType p_TryRelateType, Unit p_Node)
        {
            var prev = Node;
            
            if (p_TryRelateType != PrefabInstanceTool.MasterNodeRelateType.None
                && !ReferenceEquals(Handler, p_Node) 
                && p_Node.IsValid())
            {
                if (RestrictRelationFlagMask == PrefabInstanceTool.MasterNodeRelateType.None
                    || RestrictRelationFlagMask.HasAnyFlagExceptNone(p_TryRelateType))
                {
                    Node = p_Node;
                    RelateType = p_TryRelateType;
                }
            }
            else
            {
                if (RestrictRelationFlagMask == PrefabInstanceTool.MasterNodeRelateType.None
                    || !Node.IsValid())
                {
                    Node = null;
                    RelateType = RestrictRelationFlagMask;
                }    
            }

            if (!ReferenceEquals(prev, Node))
            {
                OnNodeTransition(prev, Node);
            }
        }

        public override void AddRestrictFlag(PrefabInstanceTool.MasterNodeRelateType p_FlagMask)
        {
            RestrictRelationFlagMask.AddFlag(p_FlagMask);
        }

        public override void RemoveRestrictFlag(PrefabInstanceTool.MasterNodeRelateType p_FlagMask)
        {
            RestrictRelationFlagMask.RemoveFlag(p_FlagMask);
        }

        public override bool CheckNode(PrefabInstanceTool.MasterNodeRelateType p_Type)
        {
            return RelateType == p_Type && CheckNode();
        }

        public override bool CheckNode(PrefabInstanceTool.MasterNodeRelateType p_Type, Unit.UnitStateType p_FilterMask)
        {
            return RelateType == p_Type && CheckNode(p_FilterMask);
        }

        public override void ClearNode()
        {
            if (RelateType != PrefabInstanceTool.MasterNodeRelateType.None)
            {
                SetNode(PrefabInstanceTool.MasterNodeRelateType.None, null);
            }
        }

        public override void ClearNode(PrefabInstanceTool.MasterNodeRelateType p_Type)
        {
            if (RelateType == p_Type)
            {
                SetNode(PrefabInstanceTool.MasterNodeRelateType.None, null);
            }
        }

        #endregion
    }

    public class FocusNode : UnitNode<PrefabInstanceTool.FocusNodeRelateType>
    {
        #region <Constructor>

        public FocusNode(PrefabInstance p_Handler, Action<Unit, Unit> p_OnNodeTransitionEvent) : base(p_Handler, p_OnNodeTransitionEvent)
        {
        }

        #endregion

        #region <Methods>

        public override void SetNode(PrefabInstanceTool.FocusNodeRelateType p_TryRelateType, Unit p_Node)
        {
            var prev = Node;
            
            if (p_TryRelateType != PrefabInstanceTool.FocusNodeRelateType.None
                && !ReferenceEquals(Handler, p_Node) 
                && p_Node.IsValid())
            {
                if (RestrictRelationFlagMask == PrefabInstanceTool.FocusNodeRelateType.None
                    || RestrictRelationFlagMask.HasAnyFlagExceptNone(p_TryRelateType))
                {
                    Node = p_Node;
                    RelateType = p_TryRelateType;
                }
            }
            else
            {
                if (RestrictRelationFlagMask == PrefabInstanceTool.FocusNodeRelateType.None
                    || !Node.IsValid())
                {
                    Node = null;
                    RelateType = RestrictRelationFlagMask;
                }    
            }

            if (!ReferenceEquals(prev, Node))
            {
                OnNodeTransition(prev, Node);
            }
        }

        public override void AddRestrictFlag(PrefabInstanceTool.FocusNodeRelateType p_FlagMask)
        {
            RestrictRelationFlagMask.AddFlag(p_FlagMask);
        }

        public override void RemoveRestrictFlag(PrefabInstanceTool.FocusNodeRelateType p_FlagMask)
        {
            RestrictRelationFlagMask.RemoveFlag(p_FlagMask);
        }
        
        public override bool CheckNode(PrefabInstanceTool.FocusNodeRelateType p_Type)
        {
            return RelateType == p_Type && CheckNode();
        }

        public override bool CheckNode(PrefabInstanceTool.FocusNodeRelateType p_Type, Unit.UnitStateType p_FilterMask)
        {
            return RelateType == p_Type && CheckNode(p_FilterMask);
        }

        public override void ClearNode()
        {
            if (RelateType != PrefabInstanceTool.FocusNodeRelateType.None)
            {
                SetNode(PrefabInstanceTool.FocusNodeRelateType.None, null);
            }
        }

        public override void ClearNode(PrefabInstanceTool.FocusNodeRelateType p_Type)
        {
            if (RelateType == p_Type)
            {
                SetNode(PrefabInstanceTool.FocusNodeRelateType.None, null);
            }
        }

        #endregion
    }
}