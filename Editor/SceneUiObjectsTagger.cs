using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace LegendaryTools.UI.Editor
{
    public class SceneUiObjectsTagger
    {
        private static readonly List<System.Type> ComponentPriorityList = new List<System.Type>()
        {
            typeof(Canvas),
            typeof(CanvasGroup),
            
            typeof(VerticalLayoutGroup),
            typeof(HorizontalLayoutGroup),
            typeof(GridLayoutGroup),
            
            typeof(Button),
            typeof(Toggle),
            typeof(Slider),
            typeof(ScrollRect),
            typeof(Scrollbar),
            typeof(InputField),
            typeof(Dropdown),
            
            typeof(Mask),
            typeof(RectMask2D),
            typeof(Text),
            typeof(Image),
            typeof(RawImage),
            
            typeof(ToggleGroup),
        };

        [MenuItem("Tools/LegendaryTools/Tag All UI Objects in Scene")]
        public static void TagGameObjects()
        {
            foreach (GameObject obj in Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if(obj.name.Contains('[') && obj.name.Contains(']')) continue;
                foreach (System.Type type in ComponentPriorityList)
                {
                    Component component = obj.GetComponent(type);
                    if (component != null)
                    {
                        obj.name = $"[{type.Name}] {obj.name}";
                        break;
                    }
                }
            }
        }
    }
}