#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using Cysharp.Threading.Tasks;

namespace BlackAm
{
    /// <summary>
    /// 특정한 값을 테이블에 기록하는 기능을 제어하는 클래스
    /// </summary>
    public static class TableModifier
    {
        #region <Consts>

        private const string TableElementName = "Table";
        private const string RecordElementName = "Record";
        private static readonly XmlWriterSettings xmlWriterSettings;
        
        static TableModifier()
        {
            xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.NewLineOnAttributes = true;
            xmlWriterSettings.Indent = true;
        }

        #endregion
        
        #region <Method/Path>

        public static async UniTask<string> GetTableFilePathByWriteType(ITableBase p_Table, ExportDataTool.WriteType p_WriteTpe, string p_TargetRootPath)
        {
            var tableNameExceptExt = p_Table.GetTableFileName(TableTool.TableNameType.Alter, false);
            var tableExt = p_Table.GetTableFileType().GetTableExtension();
            return await ExportDataTool.TryCheckGetExportFilePath
                (
                    p_TargetRootPath, 
                    tableNameExceptExt,
                    tableExt, 
                    p_WriteTpe
                );
        }
        
        #endregion
        
        #region <Method/UpdateTable>
        
        /// <summary>
        /// 지정된 위치에 해당 테이블 클래스의 정보를 파일로 생성하는 메서드
        /// </summary>
        public static async UniTask UpdateTable<K, T>(ITableBase p_TableBase, Dictionary<K, T> p_Table, ExportDataTool.WriteType p_WriteType, string p_TargetFileRootPath)
        {
            var tableDirectory = await GetTableFilePathByWriteType(p_TableBase, p_WriteType, p_TargetFileRootPath);
            await UpdateTable(p_TableBase, p_Table, tableDirectory);
        }

        /// <summary>
        /// 지정된 위치에 해당 테이블 클래스의 정보를 파일로 생성하는 메서드
        /// </summary>
        private static async UniTask UpdateTable<K, T>(ITableBase p_TableBase, Dictionary<K, T> p_Table, string p_TableCreatePath)
        {
            var xmlWriter = XmlWriter.Create(p_TableCreatePath, xmlWriterSettings);
            xmlWriter.WriteStartDocument();
            
            // 테이블 태그 기술
            xmlWriter.WriteStartElement(TableElementName);
            
            var tableRecordGroup = p_Table;
            foreach (var tableRecordPair in tableRecordGroup)
            {
                var tryRecordKey = tableRecordPair.Key;
                var tryRecordValue = tableRecordPair.Value;
                
                // 레코드 태그 기술
                xmlWriter.WriteStartElement(RecordElementName);
                
                // 키 태그 기술
                xmlWriter.WriteStartElement(TableTool.TableKeyFieldName);
                xmlWriter.WriteString(tryRecordKey.EncodeValue(typeof(K)));
                xmlWriter.WriteEndElement();
                
                // 필드 벨류 태그 기술
                var fieldInfoSet = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                foreach (var fieldInfo in fieldInfoSet)
                {
                    /* flattenHierarchy 플래그를 달아도, 해당 필드에 접근하는 메서드가 있으면, KEY 필드가 해당 블록에 진입하기 때문에
                     조건문으로 걸러준다. */
                    /* 프로퍼티의 경우 wcf 직렬화를 위해 컴파일러가 BackingField를 생성하므로 걸러준다. */
                    if(fieldInfo.Name == TableTool.TableKeyFieldName || fieldInfo.Name.Contains(TableTool.BackingFieldRear)) continue;
                    xmlWriter.WriteStartElement(fieldInfo.Name);
                    xmlWriter.WriteString(fieldInfo.GetValue(tryRecordValue).EncodeValue(fieldInfo.FieldType));
                    xmlWriter.WriteEndElement();
                }
                
                // 프로퍼티 벨류 태그 기술
                var propertyInfoSet = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                foreach (var propertyInfo in propertyInfoSet)
                {
                    if(propertyInfo.Name == TableTool.TableKeyFieldName) continue;
                    xmlWriter.WriteStartElement(propertyInfo.Name);
                    xmlWriter.WriteString(propertyInfo.GetValue(tryRecordValue).EncodeValue(propertyInfo.PropertyType));
                    xmlWriter.WriteEndElement();
                }
                
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();

            // 테이블이 수정된 경우의 콜백을 호출해준다.
            await p_TableBase.OnUpdateTableFile();
        }

        #endregion

        #region <Method/Delete>

        /// <summary>
        /// 테이블 파일을 지우는 메서드
        /// </summary>
        public static async UniTask DeleteTable(ITableBase p_TableBase, ExportDataTool.WriteType p_WriteType, string p_TargetRootPath)
        {
            await UniTask.SwitchToThreadPool();
            
            switch (p_WriteType)
            {
                case ExportDataTool.WriteType.Overlap:
                case ExportDataTool.WriteType.BackUp:
                {
                    var tableDirectory = await GetTableFilePathByWriteType(p_TableBase, p_WriteType, p_TargetRootPath);
                    if (File.Exists(tableDirectory))
                    {
                        File.Delete(tableDirectory);
                    }
                    break;
                }
                case ExportDataTool.WriteType.CopyWithNumbering:
                {
                    var tableFormat = p_TableBase.GetTableFileType();
                    var tableNameWithOutExt = p_TableBase.GetTableFileName(TableTool.TableNameType.Alter, false);
                    var tableExt = tableFormat.GetTableExtension();

                    if (Directory.Exists(p_TargetRootPath))
                    {
                        var backUpFilePattern = ExportDataTool.GetNumberingFileRegex(tableNameWithOutExt, tableExt);
                        var fileNameSet = Directory.GetFiles(p_TargetRootPath, backUpFilePattern);
                        foreach (var fileName in fileNameSet)
                        {
                            if (File.Exists(fileName))
                            {
                                File.Delete(fileName);
                            }
                        }
                    }
                }
                    break;
            }
        }

        #endregion
    }
}
#endif