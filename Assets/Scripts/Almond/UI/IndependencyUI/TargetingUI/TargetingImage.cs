#if !SERVER_DRIVE

using k514;
using UnityEngine;
using UnityEngine.UI;

public class TargetingImage
{
    private static LamiereUnit _currentTarget;
    
    private SpriteRenderer _spriteRenderer;
    private Sprite _targetingImage;

    enum ImageStage
    {
        Targeting, 
        FixedTargeting, 
        
    }

    public static LamiereUnit CurrentTarget
    {
        get => _currentTarget;
        set
        {
            if(_currentTarget == value) return;
//            if(_currentTarget != null) 
//                _currentTarget.targetingImage.Disable();
//            _currentTarget = value;
//            _currentTarget.targetingImage.Enable();
        }
    }

    public TargetingImage(Unit unit)
    {
        var targetImage = new GameObject("TargetRenderer");
        targetImage.transform.SetParent(unit.transform);
        _spriteRenderer = targetImage.AddComponent<SpriteRenderer>();
        _spriteRenderer.transform.localPosition = Vector3.zero;
        _spriteRenderer.transform.Rotate(90,0,0);
        _spriteRenderer.transform.localScale = Vector3.one * 0.5f;
        
        var loadResult = LoadAssetManager.GetInstanceUnSafe.LoadAsset<Sprite>(
            ResourceType.Image, ResourceLifeCycleType.Scene,
            "Targeting1.png");
        
        _targetingImage = loadResult.Item2;
        _spriteRenderer.sprite = _targetingImage;
        Disable();
    }

    private void Enable()
    {
        _spriteRenderer.enabled = true;
    }

    private void Disable()
    {
        _spriteRenderer.enabled = false;
    }
}
#endif