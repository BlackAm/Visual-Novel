using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Small script that makes it easy to create looping 2D sprite animations.
/// </summary>

public class UI2DSpriteAnimation : MonoBehaviour
{
    /// <summary>
    /// Index of the current frame in the sprite animation.
    /// </summary>

    public int frameIndex = 0;

    /// <summary>
    /// How many frames there are in the animation per second.
    /// </summary>

    [SerializeField] public int framerate = 20;

    /// <summary>
    /// Should this animation be affected by time scale?
    /// </summary>

    public bool ignoreTimeScale = true;

    /// <summary>
    /// Should this animation be looped?
    /// </summary>

    public bool loop = true;

    [HideInInspector]
    public bool intervalReboundLoop = false;
    public int[] intervalRebound;
    /// <summary>
    /// Actual sprites used for the animation.
    /// </summary>

    public Sprite[] frames;

    SpriteRenderer mUnitySprite;
    public UnityEngine.UI.Image mImageSprite;
    float mUpdate = 0f;

    /// <summary>
    /// Returns is the animation is still playing or not
    /// </summary>

    private PolygonCollider2D polygonCollider2D;
    public bool isPlaying { get { return enabled; } }

    /// <summary>
    /// Animation framerate.
    /// </summary>

    public int framesPerSecond { get { return framerate; } set { framerate = value; } }

    /// <summary>
    /// Continue playing the animation. If the animation has reached the end, it will restart from beginning
    /// </summary>
    public void Play()
    {
        if (frames != null && frames.Length > 0)
        {
            if (!enabled && !loop)
            {
                int newIndex = framerate > 0 ? frameIndex + 1 : frameIndex - 1;
                if (newIndex < 0 || newIndex >= frames.Length)
                    frameIndex = framerate < 0 ? frames.Length - 1 : 0;
            }
            mImageSprite.color = new Color32(255, 255, 255, 255);
            enabled = true;
            UpdateSprite();
        }
        polygonCollider2D = GetComponent<PolygonCollider2D>();
    }

    /// <summary>
    /// Pause the animation.
    /// </summary>

    public void Pause()
    {
        mImageSprite.color = new Color32(255, 255, 255, 0);
        enabled = false;
    }

    /// <summary>
    /// Reset the animation to the beginning.
    /// </summary>

    public void ResetToBeginning()
    {
        frameIndex = framerate < 0 ? frames.Length - 1 : 0;
        UpdateSprite();
    }

    /// <summary>
    /// Start playing the animation right away.
    /// </summary>

    void Start()
    {
        if (SceneManager.GetActiveScene().name.Equals("WithLoadingBar"))
        {
            Play();
        }
    }

    /// <summary>
    /// Advance the animation as necessary.
    /// </summary>

    void Update()
    {
        if (frames == null || frames.Length == 0)
        {
            enabled = false;
        }
        else if (framerate != 0)
        {
            float time = ignoreTimeScale ? Time.unscaledTime : Time.time;

            if (mUpdate < time)
            {
                mUpdate = time;
                int newIndex = framerate > 0 ? frameIndex + 1 : frameIndex - 1;

                if (!loop && (newIndex < 0 || newIndex >= frames.Length))
                {
                    enabled = false;
                    return;
                }
                if (!intervalReboundLoop)
                {
                    frameIndex = RepeatIndex(newIndex, frames.Length);
                    if (intervalRebound != null && intervalRebound.Length > 0 && frameIndex == 0) intervalReboundLoop = true;
                }
                if (intervalReboundLoop)
                {
                    frameIndex = RepeatIndex(newIndex, intervalRebound.Length);
                }
                UpdateSprite();
            }
        }
    }

    public int RepeatIndex(int val, int max)
    {
        if (max < 1) return 0;
        while (val < 0) val += max;
        while (val >= max) val -= max;
        return val;
    }

    /// <summary>
    /// Immediately update the visible sprite.
    /// </summary>

    void UpdateSprite()
    {
        if (mUnitySprite == null)
        {
            mUnitySprite = GetComponent<UnityEngine.SpriteRenderer>();
        }
        /*if (mUnitySprite == null && mImageSprite == null)
        {
            mImageSprite = GetComponent<UnityEngine.UI.Image>();
        }*/
        if (mUnitySprite == null && mImageSprite == null)
        {
            enabled = false;
            return;
        }

        float time = ignoreTimeScale ? Time.unscaledTime : Time.time;
        if (framerate != 0) mUpdate = time + Mathf.Abs(1f / framerate);

        if (!intervalReboundLoop)
        {
            if (mUnitySprite != null)
            {
                mUnitySprite.sprite = frames[frameIndex];
            }
            else if (mImageSprite != null)
            {
                mImageSprite.overrideSprite = frames[frameIndex];
            }
        }
        else
        {
            if (mUnitySprite != null)
            {
                mUnitySprite.sprite = frames[intervalRebound[frameIndex]];
            }
            else if (mImageSprite != null)
            {
                mImageSprite.overrideSprite = frames[intervalRebound[frameIndex]];
            }
        }
        PolygonColliderRefresh();
    }
    void PolygonColliderRefresh()
    {
        if (polygonCollider2D == null || mUnitySprite == null) return;

        for (int i = 0; i < polygonCollider2D.pathCount; i++)
        {
            polygonCollider2D.SetPath(i, new List<Vector2>());
        }
        polygonCollider2D.pathCount = mUnitySprite.sprite.GetPhysicsShapeCount();

        List<Vector2> path = new List<Vector2>();
        for (int i = 0; i < polygonCollider2D.pathCount; i++)
        {
            path.Clear();
            mUnitySprite.sprite.GetPhysicsShape(i, path);
            polygonCollider2D.SetPath(i, path.ToArray());
        }

    }
    public void Init()
    {
        frameIndex = 0;
        intervalReboundLoop = false;
    }
}
