using System;
using System.Collections.Generic;
using LegendaryTools.UI;
using UnityEngine;

namespace LegendaryTools
{
    public interface IProximityUi<T> where T : MonoBehaviour, ProximityDetector<T>.IProximityDetectable
    {
        event Action<ProximityUi<T>, T, UIFollowTransform> OnUiCreated;
        event Action<ProximityUi<T>, T, UIFollowTransform> OnUiRemoved;
    }

    [RequireComponent(typeof(Collider))]
    public class ProximityUi<T> : ProximityDetector<T>, IProximityUi<T> 
        where T : MonoBehaviour, ProximityDetector<T>.IProximityDetectable
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Required]
#endif
        public UIFollowTransform UiPrefab;
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Required]
#endif
        public Canvas Canvas;
        public Vector2 Offset;
        public Camera Camera;
        public Transform Target;

        public event Action<ProximityUi<T>, T, UIFollowTransform> OnUiCreated;
        public event Action<ProximityUi<T>, T, UIFollowTransform> OnUiRemoved;
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowInInspector]
#endif
        private Dictionary<T, UIFollowTransform> instances = new Dictionary<T, UIFollowTransform>();
        
        protected override void OnEnterDetected(T actor)
        {
            base.OnEnterDetected(actor);
            if (Canvas == null)
            {
                Debug.LogError("[ProximityUi:OnEnterDetected] You must provide a canvas", this);
            }
            if (UiPrefab == null)
            {
                Debug.LogError("[ProximityUi:OnEnterDetected] You must provide a UiPrefab", this);
            }

            if (instances.ContainsKey(actor)) return;

            UIFollowTransform uiFollowTransform = CreateUi(UiPrefab, Canvas);

            if (Camera == null)
            {
                Camera = Camera.main;
            }

            uiFollowTransform.Camera = Camera;
            uiFollowTransform.Canvas = Canvas;
            uiFollowTransform.Target = Target == null ? transform : Target;
            uiFollowTransform.Offset = Offset != Vector2.zero ? Offset : uiFollowTransform.Offset;
            
            instances.Add(actor, uiFollowTransform);
            OnUiCreated?.Invoke(this, actor, uiFollowTransform);
        }

        protected override void OnExitDetected(T actor)
        {
            if (instances.TryGetValue(actor, out UIFollowTransform uiFollowTransform))
            {
                OnUiRemoved?.Invoke(this, actor, uiFollowTransform);
                if (uiFollowTransform != null)
                {
                    DestroyUi(uiFollowTransform);
                }
                instances.Remove(actor);
            }
        }

        protected virtual UIFollowTransform CreateUi(UIFollowTransform uiPrefab, Canvas canvas)
        {
            return Instantiate(uiPrefab, canvas.transform);
        }
        
        protected virtual void DestroyUi(UIFollowTransform uiFollowTransform)
        {
            Destroy(uiFollowTransform.gameObject);
        }
    }
}