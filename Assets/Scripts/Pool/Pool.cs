using System;
using System.Collections.Generic;

namespace Tools.UnityUtilities
{
    /// <typeparam name="T">Element of the pool</typeparam>
    public sealed class Pool<T>
    {
        private readonly LinkedList<T> free = new();
        private readonly LinkedList<T> used = new();

        public int FreeCount => free.Count;
        public int UsedCount => used.Count;

        public Action<T> OnFree { get; set; }
        public Action<T> OnUse { get; set; }

        private Func<T> factory;

        public bool AutoExpand = true;
        public int ExpansionStep = 10;

        public void SetFactory(Func<T> factory)
        {
            this.factory = factory;
        }

        private void FreeElement(T e, bool remove = true, bool add = true, bool fastAdd = false)
        {
            if (remove)
                used.Remove(e);

            if (add)
                TryAdd(free, e, fastAdd);

            if (e is IPoolable poolable)
            {
                poolable.IsActive = false;
                poolable.OnFree();
            }

            OnFree?.Invoke(e);
        }

        private void UseElement(T e, bool remove = true, bool add = true, bool fastAdd = false)
        {
            if (remove)
                free.Remove(e);

            if (add)
                TryAdd(used, e, fastAdd);

            if (e is IPoolable poolable)
            {
                poolable.IsActive = true;
                poolable.OnUse();
            }

            OnUse?.Invoke(e);
        }

        private void TryAdd(LinkedList<T> list, T element, bool force = false)
        {
            if (force)
            {
                list.AddFirst(element);
            }
            else
            {
                bool contains = false;
                IterateAllUsed((e, _) =>
                {
                    if (e.Equals(element))
                        contains = true;
                });

                if (!contains)
                    list.AddFirst(element);
            }
        }

        public void CreateElements(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var elem = factory();
                if (elem is IPoolable poolable)
                {
                    poolable.Initialize();
                    poolable.RequestReturn = () => ReturnElement(elem);
                }
                FreeElement(elem, remove: false, add: true, fastAdd: true);
            }
        }


        public T[] RecyclePool(int count)
        {
            var finalValues = new T[count];

            int index = 0;
            LinkedListNode<T> usedNode = used.First;
            var usedCount = used.Count;

            //Reissuing used instances + returning the rest of used
            for (int i = 0; i < usedCount; i++)
            {
                if (usedNode == null || index >= count)
                    break;

                finalValues[index++] = usedNode.Value;

                FreeElement(usedNode.Value, false, false);
                UseElement(usedNode.Value, false, false);

                usedNode = usedNode.Next;
            }

            //Get the rest from free pool
            var neededElements = (count /*- 1*/) - index;
            for (int i = 0; i < neededElements; i++)
            {
                if (index >= count)
                    break;

                GetElement(out var e);
                finalValues[index++] = e;
            }

            return finalValues;
        }

        public void GetElement(out T element)
        {
            element = default;

            var node = free.First;
            if (node == null && AutoExpand)
            {
                CreateElements(ExpansionStep);
                node = free.First;
            }

            if (node == null) 
                return;
            
            element = node.Value;
            UseElement(element, fastAdd: true);
        }

        public void ReturnElement(T element)
        {
            FreeElement(element);
        }

        public void ReturnAll()
        {
            IterateAllUsed(
                (x, i) => 
                {
                    FreeElement(x, remove: false, add: true, fastAdd: true); 
                }
            );

            used.Clear();
        }

        private void IterateAllUsed(Action<T, int> callback)
        {
            int index = 0;
            foreach (var value in used)
            {
                callback(value, index++);
            }
        }
    }
}