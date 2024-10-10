using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LegendaryTools.UI
{
    [Serializable]
    public class GameObjectListing<TGameObject, TData>
        where TGameObject : Component, GameObjectListing<TGameObject, TData>.IListingItem
        where TData : IEquatable<TData>
    {
        public interface IListingItem
        {
            void Init(TData item);
            void UpdateUI(TData item);
        }
        
        public readonly Func<TData[]> DataProvider;

        public bool ForceDestroyBeforeAdd;
        public List<TGameObject> Listing = new List<TGameObject>();
        public Transform Parent;
        public TGameObject Prefab;
        
        private Transform prefabTransform;

        private readonly Dictionary<TData, TGameObject> gameObjectTable = new Dictionary<TData, TGameObject>();
        
        public event Action<TGameObject> OnPreDestroy;

        public GameObjectListing()
        {
        }

        public GameObjectListing(TGameObject prefab, Transform parent, Func<TData[]> dataProvider) : this()
        {
            Prefab = prefab;
            Parent = parent;
            DataProvider = dataProvider;
        }
        
        public virtual List<TGameObject> Generate()
        {
            return DataProvider != null ? GenerateList(DataProvider.Invoke()) : null;
        }

        public virtual List<TGameObject> GenerateList(TData[] items, Predicate<TData> filter = null)
        {
            if (ForceDestroyBeforeAdd) DestroyAll();

            foreach (TData currentItem in items)
            {
                if (filter != null)
                {
                    if (filter.Invoke(currentItem))
                        CreateOrUpdate(currentItem);
                }
                else
                    CreateOrUpdate(currentItem);
            }

            if (ForceDestroyBeforeAdd) return Listing;
            
            HashSet<TData> updatedItemCollection = new HashSet<TData>(items);
            foreach (KeyValuePair<TData, TGameObject> pair in gameObjectTable)
            {
                if (!updatedItemCollection.Contains(pair.Key))
                    Destroy(pair.Key);
            }

            return Listing;
        }

        private void CreateOrUpdate(TData currentItem)
        {
            if (gameObjectTable.TryGetValue(currentItem, out TGameObject go) && !ForceDestroyBeforeAdd)
                go.UpdateUI(currentItem);
            else
                CreateGameObject(currentItem);
        }

        public virtual void DestroyAll()
        {
            for (int i = 0; i < Listing.Count; i++)
            {
                if (Listing[i] == null) continue;
                OnPreDestroy?.Invoke(Listing[i]);
                Object.Destroy(Listing[i].gameObject);
            }

            Listing.Clear();
            gameObjectTable.Clear();
        }

        public virtual void Destroy(TData item)
        {
            if (item == null) return;
            if (gameObjectTable.TryGetValue(item, out TGameObject go))
            {
                OnPreDestroy?.Invoke(go);
                Listing.Remove(go);
                Object.Destroy(go);
            }
            gameObjectTable.Remove(item);
        }

        protected virtual TGameObject CreateGameObject(TData item)
        {
            TGameObject newGameObject = InstantiateFromPrefab(item, Prefab);
            Transform goTransform = newGameObject.transform;
            if (prefabTransform == null)
            {
                prefabTransform = Prefab.transform;
            }

            Listing.Add(newGameObject);
            gameObjectTable.Add(item, newGameObject);

            goTransform.SetParent(Parent);
            goTransform.localPosition = prefabTransform.localPosition;
            goTransform.localScale = prefabTransform.localScale;
            goTransform.localRotation = prefabTransform.localRotation;
            newGameObject.Init(item);

            return newGameObject;
        }

        protected virtual TGameObject InstantiateFromPrefab(TData item, TGameObject prefab)
        {
            return Object.Instantiate(Prefab);
        }
    }
}