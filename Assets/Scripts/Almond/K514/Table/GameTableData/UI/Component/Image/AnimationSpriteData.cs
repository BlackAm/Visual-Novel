using UnityEngine;

#if !SERVER_DRIVE
namespace k514
{
    public class AnimationSpriteData : GameTable<AnimationSpriteData, UITool.AnimationSpriteType, AnimationSpriteData.AnimationSpriteDataInstance>
    {
        public class AnimationSpriteDataInstance : GameTableRecordBase
        {
            /// <summary>
            /// 이미지 스케일
            /// </summary>
            public Vector3 Scale { private set; get; }
            
            /// <summary>
            /// 이미지 오프셋
            /// </summary>
            public Vector3 Offset { private set; get; }

            /// <summary>
            /// 스프라이트 파일 이름 포맷, 넘버링은 0부터 시작하도록 지을 것.
            /// </summary>
            public string SpriteNameFormat { private set; get; }
            
            /// <summary>
            /// 스프라이트 파일 이름 인덱스 번호 길이
            /// </summary>
            public int IndexNumberLength { private set; get; }
            
            /// <summary>
            /// 스프라이트 갯수
            /// </summary>
            public int SpriteNumber { private set; get; }
            
            /// <summary>
            /// 애니메이션 재생시간
            /// </summary>
            public float AnimationDuration { private set; get; }
            
            /// <summary>
            /// 애니메이션 반복 수, 0이면 무한
            /// </summary>
            public int LoopCount { private set; get; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "AnimationSprite";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}
#endif