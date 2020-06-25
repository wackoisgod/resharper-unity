using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;

namespace UnityEngine
{
    public class Debug
    {
        public static void Log(object message)
        {
        }
    }
}

namespace Unity
{
    namespace Jobs
    {
        [JobProducerType]
        public interface IJob
        {
            void Execute();
        }

        namespace LowLevel
        {
            namespace Unsafe
            {
                public class JobProducerTypeAttribute : Attribute
                {
                }
            }
        }
    }

    namespace Burst
    {
        public class BurstCompileAttribute : Attribute
        {
        }

        public class BurstDiscardAttribute : Attribute
        {
        }

    }

    namespace Collections
    {
        public struct NativeArray<T> : IDisposable, IEnumerable<T>, IEnumerable, IEquatable<NativeArray<T>>
            where T : struct
        {
            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public IEnumerator<T> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public bool Equals(NativeArray<T> other)
            {
                throw new NotImplementedException();
            }
        }
    }
}

namespace PrimitivesTests
{
    public class PrimitivesTests 
    {
        [BurstCompile]
        private struct PrimitivesTest1 : IJob
        {
            public void ContainsWarning()
            {
                string str2 = "asdasd"; //Burst error BC1033: Loading a managed string literal is not supported
            }

            void NoWarnings()
            {
                var bool1 = new bool();

                var sbyte1 = new sbyte();
                var byte1 = new byte();
                var short1 = new short();
                short1 = sbyte1;
                Debug.Log($"{short1}");
                var ushort1 = new ushort();
                ushort1 = byte1;
                Debug.Log($"{ushort1}");
                var int1 = new int();
                int1 = short1;
                Debug.Log($"{int1}");
                var uint1 = new uint();
                uint1 = ushort1;
                Debug.Log($"{uint1}");
                var long1 = new long();
                long1 = int1;
                Debug.Log($"{long1}");
                var ulong1 = new ulong();
                ulong1 = uint1;
                Debug.Log($"{ulong1}");

                var float1 = new float();
                var double1 = new double();
                double1 = float1;
                Debug.Log($"{double1}");
            }

            public void Execute()
            {
                NoWarnings();
                ContainsWarning();
            }
        }

        [BurstCompile]
        private struct PrimitivesTest2 : IJob
        {
            public void ContainsWarning()
            {
                string str2 = null;
                string str1 = null;
                str1 = str2;
                // Burst can operate with managed type if its value is null
                // I don't think it worth warning because of [NativeSetClassTypeToNullOnSchedule] attribute,
                // it may be desired behaviour
                if (str1 != null)
                {
                    char c = str2[0]; //Burst error BC1016: The managed function `System.String.get_Chars(System.String* this, int index)` is not supported
                }
            }

            public void Execute()
            {
                ContainsWarning();
            }
        }

        [BurstCompile]
        private struct PrimitivesTest3 : IJob
        {
            public int res;
            public void NoWarnings()
            {
                // burst compiler is able to create decimals cuz they are struct
                // but cannot operate with them because decimal uses internal calls
                // which is not supported by burst. 
                var decel = new decimal();
                res = decimal.ToInt32(decel); // CGTD Unable to find internal function
            }

            public void Execute()
            {
                NoWarnings();
                Debug.Log($"{res}");
            }
        }
    }
}