using UnityEngine;

namespace LegendartTools.UI
{
    public class UISafeArea : MonoBehaviour
    {
        private RectTransform rectTransform;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowInInspector]
#endif
        private Rect lastSafeArea = Rect.zero;

        void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            ApplySafeArea();
        }

        void Update()
        {
            ApplySafeArea();
        }

        private void ApplySafeArea()
        {
            Rect safeArea = UnityEngine.Screen.safeArea;
            if (safeArea != lastSafeArea)
            {
                lastSafeArea = safeArea;

                Vector2 anchorMin = safeArea.position;
                Vector2 anchorMax = safeArea.position + safeArea.size;
                anchorMin.x /= UnityEngine.Screen.width;
                anchorMin.y /= UnityEngine.Screen.height;
                anchorMax.x /= UnityEngine.Screen.width;
                anchorMax.y /= UnityEngine.Screen.height;
                
                rectTransform.anchorMin = anchorMin;
                rectTransform.anchorMax = anchorMax;
            }
        }
    }
}