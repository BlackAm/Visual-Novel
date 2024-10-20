#if !SERVER_DRIVE

using System;
using UnityEngine.UI;
using UnityEngine;

namespace BlackAm
{
    public class WorldMapUI : AbstractUI
    {
        private Image _worldMapImage;

        private Transform _playerWorldMapImage;
        private Transform _cameraWorldMapImage;
        private Transform _rotateParent;
        private Transform _marks;
        private RectTransform _worldMapTransform;
        public GameObject npcMarker;

        public void Init(float scale = 1)
        {
            //scale = 1;
            _worldMapImage = GetComponent<Image>("MapImage/RotateParent/Image");
            npcMarker = Find("MapImage/RotateParent/Image/NPC").gameObject;
            npcMarker.SetActive(false);
            SetImageColor(_worldMapImage, new Color32(255,255,255,150));
            _playerWorldMapImage = Find("MapImage/RotateParent/Marks/PlayerView");
            _cameraWorldMapImage = Find("MapImage/RotateParent/Marks/CameraView");
            _rotateParent = Find("MapImage/RotateParent");
            _marks = Find("MapImage/RotateParent/Marks");

            _worldMapTransform = _worldMapImage.GetComponent<RectTransform>();

            float markSize = 1 / scale;
            _rotateParent.GetComponent<RectTransform>().localScale = new Vector3(scale, scale);
            _marks.GetComponent<RectTransform>().localScale = new Vector3(markSize, markSize);
            //_marks.Find("Player").GetComponent<Image>().enabled = false;
        }

        public void SetImageColor(Image image, Color color)
        {
            image.color = color;
        }

        public void SetWorldMapSprite(Sprite sprite)
        {
            _worldMapImage.sprite = sprite;
        }
        
        // momo6346
        // 월드맵 변경사항: 마커고정이 아닌 전체 맵 출력 후 
        // 플레이어의 움직임에 따라 마커도 같이 움직임.
        public void SetWorldMapPosition(Vector3 position)
        {
            //_worldMapTransform.localPosition = position;
            _playerWorldMapImage.localPosition = position;
            _cameraWorldMapImage.localPosition = position;
        }
        //테스트 메니저 출력을 위해 플레이어 마커의 위치를 가져옴
        public Vector3 GetWorldMapPosition(){
            return _playerWorldMapImage.localPosition;
        }
        public void RotatePlayerMark(float degree)
        {
            _playerWorldMapImage.transform.localRotation = Quaternion.Euler(0, 0, degree);
        }
        public void RotateCameraMark(float degree)
        {
            _cameraWorldMapImage.transform.localRotation = Quaternion.Euler(0, 0, degree);
        }

        public void SetParentMaker(Transform p_Maker){
            p_Maker.SetParent(_worldMapImage.transform, false);
        }
    }
}
#endif