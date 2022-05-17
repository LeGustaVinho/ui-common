using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LegendaryTools.UI
{
    public class GameObjectListing<TGameObject, TData>
        where TGameObject : Component, GameObjectListing<TGameObject, TData>.IListingItem
    {
        public readonly Func<TData[]> DataProvider;

        public bool AutoDestroyAllBeforeAdd = true;
        public List<TGameObject> Listing = new List<TGameObject>();
        public Transform Parent;
        public TGameObject Prefab;
        private Transform prefabTransform;

        public GameObjectListing()
        {
        }

        public GameObjectListing(TGameObject prefab, Transform parent, Func<TData[]> dataProvider) : this()
        {
            Prefab = prefab;
            Parent = parent;
            DataProvider = dataProvider;
        }

        public event Action<TGameObject> OnPreDestroy;

        public virtual List<TGameObject> Generate()
        {
            return DataProvider != null ? GenerateList(DataProvider.Invoke()) : null;
        }

        public virtual List<TGameObject> GenerateList(TData[] itens, Predicate<TData> filter = null)
        {
            if (AutoDestroyAllBeforeAdd)
            {
                DestroyAll();
            }

            for (int i = 0; i < itens.Length; i++)
            {
                if (filter != null)
                {
                    if (filter.Invoke(itens[i]))
                    {
                        CreateGameObject(itens[i]);
                    }
                }
                else
                {
                    CreateGameObject(itens[i]);
                }
            }

            return Listing;
        }

        public virtual void DestroyAll()
        {
            for (int i = 0; i < Listing.Count; i++)
            {
                if (Listing[i] != null)
                {
                    OnPreDestroy?.Invoke(Listing[i]);
                    Object.Destroy(Listing[i].gameObject);
                }
            }

            Listing.Clear();
        }

        protected virtual TGameObject CreateGameObject(TData item)
        {
            TGameObject newGO = InstantiateFromPrefab(item, Prefab);
            Transform newGOTransform = newGO.transform;
            if (prefabTransform == null)
            {
                prefabTransform = Prefab.transform;
            }

            Listing.Add(newGO);

            newGOTransform.SetParent(Parent);
            newGOTransform.localPosition = prefabTransform.localPosition;
            newGOTransform.localScale = prefabTransform.localScale;
            newGOTransform.localRotation = prefabTransform.localRotation;
            newGO.Init(item);

            return newGO;
        }

        protected virtual TGameObject InstantiateFromPrefab(TData item, TGameObject prefab)
        {
            return Object.Instantiate(Prefab);
        }

        public interface IListingItem
        {
            void Init(TData item);
        }
    }
}