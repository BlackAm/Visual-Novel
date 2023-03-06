// #if !SERVER_DRIVE
// using System.Collections.Generic;
// using Cysharp.Threading.Tasks;
// using UnityEngine;
// using EPOOutline;
// using k514;
//
// /// <summary>
// /// Outline 전용 스크립트
// /// </summary>
// public class ShaderEffectTool : AsyncSingleton<ShaderEffectTool>
// {
//     #region <Enum>
//     public enum OutLineStyle
//     {
//         Only,   //항상
//         Hide,   //숨긴곳만
//     }
//     #endregion
//
//     #region <Const>
//     private List<Outlinable> _Outlinables;
//     private Dictionary<Unit, Outlinable> _UseOutLine;
//     private GameObject _OutLineGo;
//     
//     #endregion
//
//     #region <Callback>
//     
//     protected override async UniTask OnCreated()
//     {
//         await UniTask.SwitchToMainThread();
//         
//         _Outlinables = new List<Outlinable>();
//         _UseOutLine = new Dictionary<Unit, Outlinable>();
//         _OutLineGo = new GameObject("OutLineManager");
//         _OutLineGo.transform.parent = SystemBoot.GetInstance._Transform;
//
//         for (int i = 0; i < 20; i++)
//         {
//             var outline = _OutLineGo.AddComponent<Outlinable>();
//             outline.enabled = false;
//
//             _Outlinables.Add(outline);
//         }
//         AddOutlineComponent();
//     }
//     public override async UniTask OnInitiate()
//     {
//         await UniTask.CompletedTask;
//     }
//
//     #endregion
//
//     #region <Method>
//     /// <summary>
//     /// 아웃라인 사용
//     /// </summary>
//     /// <param name="unit"></param>
//     /// <param name="color"></param>
//     public void AddOutLine(Unit unit, Color color, OutLineStyle outLineStyle, float size = 0.7f)
//     {
//         if (unit.IsValid() && !_UseOutLine.ContainsKey(unit))
//         {
//             Outlinable outline = null;
//
//             for(int i = 0; i < _Outlinables.Count; i++)
//             {
//                 if(!_Outlinables[i].enabled)
//                 {
//                     outline = _Outlinables[i];
//                     break;
//                 }
//             }
//
//             if (outline == null)
//             {
//                 outline = _OutLineGo.AddComponent<Outlinable>();
//                 _Outlinables.Add(outline);
//             }
//             outline.enabled = true;
//
//             outline.OutlineLayer = unit.gameObject.layer;
//             outline.OutlineParameters.Color = color;
//             outline.OutlineParameters.DilateShift = size;
//             SetOutlineStyle(outline, color, outLineStyle);
//
//             outline.AddAllChildRenderersToRenderingList(RenderersAddingMode.All, unit._RenderObject._DefaultModelRendererControlPreset.RendererGroup.ToArray());
//             _UseOutLine.Add(unit, outline);    
//         }
//     }
//     /// <summary>
//     /// 아웃라인 사용안함
//     /// </summary>
//     public void DelOutLine(Unit unit)
//     {
//         if (_UseOutLine.ContainsKey(unit))
//         {
//             var outline = _UseOutLine[unit];
//             outline.ClearOutLine();
//             outline.enabled = false;
//
//             _UseOutLine.Remove(unit);
//         }
//     }
//
//     /// <summary>
//     /// 아웃라인의 스타일을 변경
//     /// </summary>
//     void SetOutlineStyle(Outlinable outline, Color color, OutLineStyle outLineStyle)
//     {
//         if(outLineStyle == OutLineStyle.Only)
//         {
//             outline.RenderStyle = RenderStyle.Single;
//         }
//         else if(outLineStyle == OutLineStyle.Hide)
//         {
//             outline.RenderStyle = RenderStyle.FrontBack;
//             outline.FrontParameters.Enabled = false;
//             outline.BackParameters.Color = color;
//         }
//
//     }
//
//     public void AddOutlineComponent()
//     {
//         var MainCamera = CameraManager.GetInstanceUnSafe.MainCamera;
//         var outLiner = MainCamera.GetComponent<Outliner>();
//         if (outLiner == null)
//         {
//             outLiner = MainCamera.gameObject.AddComponent<Outliner>();
//         }
//         outLiner.DilateIterations = 1;
//         //outLiner.DilateShift = 0.7f;
//         outLiner.DilateShift = 1.4f;
//         outLiner.OutlineLayerMask = GameManager.UnitSet_LayerMask;
//     }
//     #endregion
// }
//
// #endif