/*
 * (c) Copyright 2020 Lokel Digital Pty Ltd.
 * https://www.lokeldigital.com
 * 
 * LokelPackage can be used under the Creative Commons License AU by Attribution
 * https://creativecommons.org/licenses/by/3.0/au/legalcode
 */




using Unity.Collections;
using Unity.Jobs;

namespace Lokel.Collections
{
    /// <summary>Holds context and has a computation update method - should be a struct.</summary>
    /// <typeparam name="T">a value type</typeparam>
    public interface IQueueIterator<T> : IJob where T : struct
    {
        NativeArray<T> Store { set; }
        OutValue<int> Count { set; }
    }
}
