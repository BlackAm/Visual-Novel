using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BlackAm
{
    public partial class SaveLoadManager : AsyncSingleton<SaveLoadManager>
    {
        #region <Callbacks>

        protected override async UniTask OnCreated()
        {
            await UniTask.CompletedTask;
        }

        public override async UniTask OnInitiate()
        {
            await UniTask.CompletedTask;

            loadType = LoadType.None;
        }

        protected override void DisposeUnManaged()
        {
            base.DisposeUnManaged();
            
            // 게임 종료 시 세이브
            SaveReadDialogueFile();
        }

        #endregion

        #region <Class>

        [Serializable]
        public class DataSave
        {
            #region <Method>
            
            public void SerializeObject(string p_TargetFileFullPath)
            {
                var trySerialize = SerializeObject();
                if (!ReferenceEquals(null, trySerialize))
                {
                    var rootPath = p_TargetFileFullPath.GetUpperPath();
                    if (!Directory.Exists(rootPath))
                    {
                        Directory.CreateDirectory(rootPath);
                    }
                    using (var outFile = new FileStream(p_TargetFileFullPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        outFile.Write(trySerialize, 0, trySerialize.Length);
                    }
                }
            }

            public byte[] SerializeObject()
            {
                if (ReferenceEquals(null, this))
                {
                    return null;
                }
                else
                {
                    var bf = new BinaryFormatter();
                    using (var ms = new MemoryStream())
                    {
                        bf.Serialize(ms, this);
                        return ms.ToArray();
                    }
                }
            }
            
            #endregion
        }

        [Serializable]
        public class SaveData : DataSave
        {
            #region <Fields>

            public string SaveDataTime;

            public int DialogueKey;

            public DialogueGameManager.DialogueEndingFlag DialogueEndingFlag;

            public DialogueEventData DialogueEvent;

            #endregion

            #region <Constructors>

            public SaveData(int p_DialogueKey, DialogueGameManager.DialogueEndingFlag p_DialogueEndingFlag, DialogueEventData p_DialogueEvent)
            {
                SaveDataTime = DateTime.Now.ToString("yy/MM/dd HH:mm:ss").Replace("-", "/");
                DialogueKey = p_DialogueKey;
                DialogueEndingFlag = p_DialogueEndingFlag;
                DialogueEvent = p_DialogueEvent;
            }

            #endregion

            #region <Struct>

            [Serializable]
            public struct DialogueEventData
            {
                #region <Fields>
                public int BGM;

                public ImageSaveData BackGroundImageSave;

                public ImageSaveData EventCGSave;
                
                public bool FadeActivated;

                public Dictionary<Character, int> Liking;

                public Dictionary<Character, CharacterImageSaveData> CharacterImageSave;
                
                #endregion
                
                #region <Constructors>
                
                public DialogueEventData(int p_BGM, bool p_FadeActivated, Dictionary<Character, int> p_Liking,
                    Dictionary<Character, CharacterImageSaveData> p_CharacterImageSave, ImageSaveData p_BackGroundImageSave, ImageSaveData p_EventCGSave)
                {
                    BGM = p_BGM;
                    Liking = p_Liking;
                    FadeActivated = p_FadeActivated;
                    CharacterImageSave = p_CharacterImageSave;
                    BackGroundImageSave = p_BackGroundImageSave;
                    EventCGSave = p_EventCGSave;
                }

                public DialogueEventData(int p_BGM, bool p_FadeActivated, Dictionary<Character, int> p_Liking)
                    : this(p_BGM, p_FadeActivated, p_Liking, new Dictionary<Character, CharacterImageSaveData>(), new ImageSaveData(), new ImageSaveData())
                {
                }


                #endregion
            }

            
            
            #endregion
        }

        [Serializable]
        public class CharacterImageSaveData
        {
            #region <Fields>

            public SerializedVector3 Position;

            public int ImageKey;

            public float Scale;

            public (float, float) FadeTuple;

            #endregion

            #region <Constructors>

            public CharacterImageSaveData(SerializedVector3 p_Position, int p_Key, float p_Scale, (float, float) p_FadeTuple)
            {
                Position = p_Position;
                ImageKey = p_Key;
                Scale = p_Scale;
                FadeTuple = p_FadeTuple;
            }

            #endregion
        }

        [Serializable]
        public class ImageSaveData
        {
            #region <Fields>

            public SerializedVector2 Position;

            public int ImageKey;

            public float Scale;

            #endregion

            #region <Constructors>

            public ImageSaveData()
            {
                Position = new SerializedVector2();
                ImageKey = 0;
                Scale = 1;
            }

            public ImageSaveData(ImageSaveData p_ImageSaveData)
            {
                Position = p_ImageSaveData.Position;
                ImageKey = p_ImageSaveData.ImageKey;
                Scale = p_ImageSaveData.Scale;
            }

            public ImageSaveData(SerializedVector2 p_Position, int p_Key, float p_Scale)
            {
                Position = p_Position;
                ImageKey = p_Key;
                Scale = p_Scale;
            }

            #endregion
        }

        [Serializable]
        public class ReadDialogueKey : DataSave
        {
            #region <Fields>

            public Dictionary<int, bool> ReadDialogueInfo;

            #endregion
            
            #region <Constructors>

            public ReadDialogueKey()
            {
                ReadDialogueInfo = new Dictionary<int, bool>();
            }
            
            #endregion
        }

        [Serializable]
        public class GallerySaveData : DataSave
        {
            #region <Fields>

            public Dictionary<GalleryManager.GalleryType, Dictionary<int, GalleryUnLockData>> GallerySaveInfo;

            #endregion

            #region <Constructors>

            public GallerySaveData()
            {
                GallerySaveInfo = new Dictionary<GalleryManager.GalleryType, Dictionary<int, GalleryUnLockData>>();
            }

            #endregion
        }
        
        public class GalleryUnLockData
        {
            public Dictionary<int, bool> GalleryUnLockInfo;

            public GalleryUnLockData(int p_ImageKey)
            {
                GalleryUnLockInfo = new Dictionary<int, bool>();
                GalleryUnLockInfo.Add(p_ImageKey, false);
            }

            public GalleryUnLockData(List<int> p_ImageKeyList)
            {
                GalleryUnLockInfo = new Dictionary<int, bool>();

                foreach (var imageKey in p_ImageKeyList)
                {
                    GalleryUnLockInfo.Add(imageKey, false);
                }
            }
        }

        #endregion

        #region <Struct>

        [Serializable]
            public struct SerializedVector3
            {
                public float x;
                public float y;
                public float z;

                public SerializedVector3(float p_X, float p_Y, float p_Z)
                {
                    x = p_X;
                    y = p_Y;
                    z = p_Z;
                }

                public SerializedVector3(Vector3 p_Vector)
                {
                    x = p_Vector.x;
                    y = p_Vector.y;
                    z = p_Vector.z;
                }

                public override bool Equals(object obj)
                {
                    if ((obj is SerializedVector3) == false)
                    {
                        return false;
                    }

                    var s = (SerializedVector3)obj;
                    return x == s.x && y == s.y && z == s.z;
                }

                public Vector3 ToVector3()
                {
                    return new Vector3(x, y, z);
                }

                public static bool operator == (SerializedVector3 a, SerializedVector3 b)
                {
                    return a.x == b.x && a.y == b.y && a.z == b.z;
                }

                public static bool operator != (SerializedVector3 a, SerializedVector3 b)
                {
                    return a.x != b.x || a.y != b.y || a.z != b.z;
                }

                public static implicit operator Vector3(SerializedVector3 p_SVector3)
                {
                    return new Vector3(p_SVector3.x, p_SVector3.y, p_SVector3.z);
                }

                public static implicit operator SerializedVector3(Vector3 p_Vector3)
                {
                    return new SerializedVector3(p_Vector3.x, p_Vector3.y, p_Vector3.z);
                }
            }

            [Serializable]
            public struct SerializedVector2
            {
                public float x;
                public float y;

                public SerializedVector2(float p_X, float p_Y)
                {
                    x = p_X;
                    y = p_Y;
                }

                public SerializedVector2(Vector2 p_Vector)
                {
                    x = p_Vector.x;
                    y = p_Vector.y;
                }

                public override bool Equals(object obj)
                {
                    if ((obj is SerializedVector2) == false)
                    {
                        return false;
                    }

                    var s = (SerializedVector2) obj;
                    return x == s.x && y == s.y;
                }

                public Vector2 ToVector2()
                {
                    return new Vector2(x, y);
                }

                public static bool operator ==(SerializedVector2 a, SerializedVector2 b)
                {
                    return a.x == b.x && a.y == b.y;
                }

                public static bool operator !=(SerializedVector2 a, SerializedVector2 b)
                {
                    return a.x != b.x || a.y != b.y;
                }

                public static implicit operator Vector2(SerializedVector2 p_SVector2)
                {
                    return new Vector2(p_SVector2.x, p_SVector2.y);
                }

                public static implicit operator SerializedVector2(Vector2 p_Vector2)
                {
                    return new SerializedVector2(p_Vector2.x, p_Vector2.y);
                }
            }

        #endregion
    }
}
