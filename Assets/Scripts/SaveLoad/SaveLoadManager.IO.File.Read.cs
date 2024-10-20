using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BlackAm
{
    public partial class SaveLoadManager
    {
        public (bool ,T) GetDataFileInDirectory<T>(string p_FilePath)
        {
            if (File.Exists(p_FilePath))
            {
                var resultData = DecodeData.DeserializeObject<T>(p_FilePath);
                return (true, resultData);
            }
            else
            {
                return (false, default);
            }
        }
        
        
    }

}
