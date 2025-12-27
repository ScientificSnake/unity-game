using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using TMPro.EditorUtilities;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapRegister : MonoBehaviour
{
    private static readonly Vector2 _BaseMiniMapScale = new Vector2(1920 - 80, 1080 - 80);

    //private static int _IconSortingOrder = 2003;

    private CanvasGroup _canvasGroup;

    private static Vector3 IconScale = new Vector3(0.3f, 0.3f, 0.3f);

    private static float MiniMapScale;
    private static List<IMiniMapTrackable> TrackedObjs = new();

    private static Dictionary<int, GameObject> MiniMapIcons = new();

    public static bool MiniMapShown;
    private static MiniMapRegister Instance;
    private Image BgImage;
    
    private static void CalculateMiniMapScale()
    {
        Vector2 MapDimensions = new();

        MapDimensions.x = Mathf.Abs(ManagerScript.CurrentLevelManagerInstance.RootLevelData.MinXY.x -
                                    ManagerScript.CurrentLevelManagerInstance.RootLevelData.MaxXY.x);
        MapDimensions.y = Mathf.Abs(ManagerScript.CurrentLevelManagerInstance.RootLevelData.MinXY.y -
                                    ManagerScript.CurrentLevelManagerInstance.RootLevelData.MaxXY.y);

        float xScale = _BaseMiniMapScale.x / MapDimensions.x;
        float yScale = _BaseMiniMapScale.y / MapDimensions.y;

        float scale = Mathf.Min(xScale, yScale);
        
        MiniMapScale = scale;
    }

    public static void Register(IMiniMapTrackable obj)
    {
        TrackedObjs.Add(obj);
        MiniMapIcons.TryAdd(obj.Rigidbody2.gameObject.GetInstanceID(), null);
    }

    public static void DeRegister(IMiniMapTrackable obj)
    {
        TrackedObjs.Remove(obj);
        int key = obj.Rigidbody2.gameObject.GetInstanceID();
        if (MiniMapIcons.TryGetValue(key, out GameObject icon) && icon != null)
        {
            Destroy(icon);
        }
        MiniMapIcons.Remove(key);
    }

    public static void EnableMiniMap()
    {
        Instance._canvasGroup.alpha = 1;
        MiniMapShown = true;
        CalculateMiniMapScale();
        print($"<color=green> minimap scale is {MiniMapScale}");
    }

    public static void DisableMiniMap()
    {
        Instance.ClearMiniMap();
        Instance._canvasGroup.alpha = 0;
        
        MiniMapShown = false;
    }

    private void ClearMiniMap()
    {
        List<GameObject> Children = new();

        foreach (Transform childTransform in transform)
        {
            Children.Add(childTransform.gameObject);
        }

        for (int i = Children.Count - 1; i > -1; i--)
        {
            Destroy(Children[i]);
        }
        MiniMapIcons.Clear();
    }
    public void RenderMiniMap()
    {
        print($"<color=yellow>minimap rendering, {TrackedObjs.Count} objects being tracked");
        for (int i = TrackedObjs.Count - 1; i >= 0; i--)
        {
            IMiniMapTrackable trackedObj = TrackedObjs[i];

            // Clean up destroyed objects
            if (trackedObj == null || trackedObj.Rigidbody2 == null)
            {
                TrackedObjs.RemoveAt(i);
                continue;
            }

            int key = trackedObj.Rigidbody2.gameObject.GetInstanceID();
            GameObject icon;

            if (!MiniMapIcons.TryGetValue(key, out icon))
            {
                icon = null;
            }

            // Create icon if
            if (icon == null)
            {
                icon = new GameObject($"{key}_Icon");
                Image iconImg = icon.AddComponent<Image>();
                iconImg.sprite = trackedObj.MiniMapIcon;
                icon.transform.SetParent(transform, false);
                MiniMapIcons[key] = icon;
            }

            // Update every frame
            Vector2 iconPos = trackedObj.Rigidbody2.position * MiniMapScale;
            RectTransform rectTransform;

            if (!icon.TryGetComponent(out rectTransform))
            {
                rectTransform = icon.AddComponent<RectTransform>();
            }

            rectTransform.anchoredPosition = iconPos;
            rectTransform.rotation = Quaternion.Euler(0, 0, trackedObj.Rigidbody2.rotation);
            rectTransform.localScale = IconScale;

            if (trackedObj.SeenByPlayer)
            {
                Color color = icon.GetComponent<Image>().color;
                color.a = 1;
                icon.GetComponent<Image>().color = color;
            }
            else
            {
                Color color = icon.GetComponent<Image>().color;
                color.a = 0;
                icon.GetComponent<Image>().color = color;
            }
        }
    }

    public void Update()
    {
        if (MiniMapShown)
        {
            RenderMiniMap();
        }
    }

    public void Awake()
    {
        Instance = this;
        BgImage = gameObject.GetComponent<Image>();
        _canvasGroup = gameObject.GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        DisableMiniMap();
    }
}

public interface IMiniMapTrackable
{
    public bool SeenByPlayer { get; }
    public Rigidbody2D Rigidbody2 { get; }
    public Sprite MiniMapIcon { get; }
}
