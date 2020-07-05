/*
 * (c) Copyright 2020 Lokel Digital Pty Ltd.
 * https://www.lokeldigital.com
 * 
 * LokelPackage can be used under the Creative Commons License AU by Attribution
 * https://creativecommons.org/licenses/by/3.0/au/legalcode
 */

using Unity.Burst;

using Unity.Collections;
using Unity.Jobs;


namespace Lokel.Collections
{
    public static class QueueJob<T> where T : struct
    {
        public static JobHandle AsyncEnqueue(NativeQueue<T> queue, T value, JobHandle dependency)
        {
            var job = new EnqueueJob<T>()
            {
                Queue = queue,
                Value = value
            };
            return IJobExtensions.Schedule(job, dependency);
        }

        public static JobHandle AsycIterate<I>(
            NativeQueue<T> queue,
            I iterator,
            JobHandle dependency
        ) where I : struct, IQueueIterator<T>
        {
            (NativeArray<T> array, OutValue<int> count) = queue.InternalGetStorage();
            iterator.Store = array;
            iterator.Count = count;

            return IJobExtensions.Schedule(iterator, dependency);
        }
    }

    [BurstCompile]
    internal struct EnqueueJob<T> : IJob
        where T : struct
    {
        public NativeQueue<T> Queue;

        [ReadOnly] public T Value;

        public void Execute() => Queue.TryEnqueue(Value);
    }

}
