#if UNITY_EDITOR
using Cysharp.Threading.Tasks;

namespace BlackAm
{
    /// <summary>
    /// 셰이더의 경우에는 기본적으로 GraphicSetting.asset에 빌드에 포함될 셰이더가 자동으로 추가되어야 하는데, Resource폴더의
    /// 랜더러들이 사용하는 셰이더는 빌드 시에, 자동 등록되지만 번들의 렌더러들이 사용하는 셰이더 중에서 유니티 기본 셰이더 들은
    /// 빌드 시에 포함되지 않기 때문에 수동으로 등록해주어야 한다. 기본 셰이더가 아닌 셰이더들은 의존성에 의해 에셋번들에 포함시키면
    /// 같이 로드되므로 문제가 없다.
    ///
    /// 셰이더 의존성을 만드려면, 우선 ShaderCrawler로 빌드에 포함될 셰이더를 등록시킨 다음에
    /// 에셋 번들을 만들면 된다.
    /// 
    /// </summary>
    public class ShaderCrawler : EditorModeOnlyGameData<ShaderCrawler, string, ShaderCrawler.ShaderRecord>
    {
        public class ShaderRecord : EditorModeOnlyTableRecord
        {
            public bool IsDefaultTable { get; private set; }

            public override async UniTask SetRecord(string p_Key, object[] p_RecordField)
            {
                await base.SetRecord(p_Key, p_RecordField);
                
                IsDefaultTable = (bool)p_RecordField[0];
            }
        }

        /// <summary>
        /// 해당 테이블에 들어갈 기본 값을 테이블 컬렉션에 레코드 인스턴스로 추가하는 메서드
        /// </summary>
        protected override async UniTask GenerateDefaultRecordSet()
        {
            await base.GenerateDefaultRecordSet();
            
            // 'Always Included Shader'에 기본적으로 포함되어 있는 셰이더들을 테이블 레코드에 기술한다.
            var projectDefaultShaderGroup = SystemMaintenance.DefaultShaderNameList;
            foreach (var shaderName in projectDefaultShaderGroup)
            {
                await AddRecord(shaderName, true);
            }
        }

        protected override string GetDefaultTableFileName()
        {
            return "AssembledShaderTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}
#endif