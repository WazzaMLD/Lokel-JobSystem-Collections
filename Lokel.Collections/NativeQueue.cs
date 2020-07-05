/*
 * (c) Copyright 2020 Lokel Digital Pty Ltd.
 * https://www.lokeldigital.com
 * 
 * LokelPackage can be used under the Creative Commons License AU by Attribution
 * https://creativecommons.org/licenses/by/3.0/au/legalcode
 */


using System;

using Unity.Collections;

namespace Lokel.Collections
{
    public struct NativeQueue<T> : IDisposable
        where T : struct
    {
        private Allocator _Allocator;
        private NativeArray<T> _Store;
        private OutValue<int> _Index;

        // For internal use by QueueJob work....
        internal (NativeArray<T> store, OutValue<int> count) InternalGetStorage()
            => (_Store, _Index);

        public bool IsCreated => _Store.IsCreated;

        public NativeQueue(int capacity, T initValue, Allocator allocator = Allocator.Persistent)
        {
            _Allocator = allocator;
            _Store = new NativeArray<T>(capacity, _Allocator);
            _Index = new OutValue<int>(0, _Allocator);
            ClearWithValue(initValue);
        }

        public void Dispose()
        {
            if (_Allocator == Allocator.Persistent)
            {
                if (_Store.IsCreated) _Store.Dispose();
                if (_Index.IsCreated) _Index.Dispose();
            }
        }

        public bool TryEnqueue(T value)
        {
            bool isOk = IsSlotAvailableToWrite();
            if (isOk) PushStack(value);
            return isOk;
        }

        public bool TryDequeue(out T value)
        {
            bool isOk = IsDataToRead();
            if (isOk) value = PopStack();
            else value = default;
            return isOk;
        }

        /// <summary>Visit each item in queue - list in is first visited order.</summary>
        /// <typeparam name="Tcontext">class or struct for context - object if N/A</typeparam>
        /// <param name="context">Caller info to pass each item - null if N/A</param>
        /// <param name="OnItem">Function to call on each item with context.</param>
        /// <returns></returns>
        public int VisitAllItemsNewestToOldest<Tcontext>(
            Tcontext context, Action<T, Tcontext> OnItem
        )
        {
            int count = 0;
            for(int index = _Index.Value - 1; index >= 0; index--)
            {
                OnItem(_Store[index], context);
                count++;
            }
            return count;
        }

        public int VisitAllItemsOldestToNewest<Tcontext>(
            Tcontext context,
            Action<T, Tcontext, int> OnItem
        )
        {
            int count = 0;
            for(int index = 0; index < _Index.Value; index++)
            {
                OnItem(_Store[index], context, index);
                count++;
            }
            return count;
        }

        public T PeekHeadOfQueue() => _Store[0];

        public int QueueDepth() => _Index.Value;

        public bool IsSlotAvailableToWrite()
            => IsCreated && 0 <= _Index.Value
            && _Index.Value < _Store.Length;
        public bool IsDataToRead() => IsCreated && 0 < _Index.Value;

        private T PopStack()
        {
            T value = _Store[0];
            for(int index = 1; index < _Store.Length && index < _Index.Value; index++)
            {
                _Store[index - 1] = _Store[index];
            }
            _Index.Value = _Index.Value - 1;
            return value;
        }

        private void PushStack(T value)
        {
            _Store[_Index.Value] = value;
            _Index.Value++;
        }

        private void ClearWithValue(T initValue)
        {
            for (int index = 0; index < _Store.Length; index++)
            {
                _Store[index] = initValue;
            }
        }
    }
}
