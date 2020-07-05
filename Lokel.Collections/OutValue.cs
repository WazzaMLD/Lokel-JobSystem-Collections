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
    public struct OutValue<T> : IDisposable
        where T : struct
    {
        private NativeArray<T> _Store;
        public T Value
        {
            get => _Store.IsCreated ? _Store[0] : default;
            set { if (_Store.IsCreated) _Store[0] = value; }
        }

        public bool IsCreated => _Store.IsCreated;

        public OutValue(T initValue, Allocator allocKind = Allocator.Persistent)
        {
            _Store = new NativeArray<T>(1, allocKind);
            Value = initValue;
        }

        public void Dispose()
        {
            if (_Store.IsCreated) _Store.Dispose();
        }
    }

}
