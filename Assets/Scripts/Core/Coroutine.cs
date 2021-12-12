using System;
using System.Collections;
using UnityEngine;

namespace core
{
    public class Coroutine
    {

        public class Job
        {
            public int Counter;
            public bool Busy => Counter > 0;

            public System.Collections.IEnumerable Wait(int until = 0)
            {
                while (Counter > until) yield return null;
            }

        }

        System.Collections.Generic.List<IEnumerator> _Stack = new System.Collections.Generic.List<IEnumerator>();
        float _WakeTime;
        GameObject _Parent;
        Job _Job;

        static System.Collections.Generic.List<Coroutine> _Running = new System.Collections.Generic.List<Coroutine>();

        static Timer _TickTimer = new Timer(0)
        {
            run = Coroutine.Tick
        };
        static Coroutine _Current;
        static _Wait _W = new _Wait();

        IEnumerator Routine => _Stack.Count > 0 ? _Stack[_Stack.Count - 1] : null;

        public static void Start(GameObject parent, Func<IEnumerable> routine, Job job = null)
        {
            var co = new Coroutine
            {
                _Parent = parent,
                _Job = job
            };
            co._Stack.Add(routine.Invoke().GetEnumerator());
            _Running.Add(co);
            if (job != null) job.Counter++;
        }

        public static object Wait(float duration)
        {
            _W.millis = duration;
            return _W;
        }


        internal static void Tick()
        {
            var now = Time.frameCount;
            var index = _Running.Count - 1;
            while (index >= 0)
            {
                var ptr = _Running[index];
                if (ptr._WakeTime > 0)
                {
                    if (now < ptr._WakeTime)
                    {
                        index--;
                        continue;
                    }
                    ptr._WakeTime = 0;
                }

                var routine = ptr.Routine;
                var parentGone = ptr._Parent == null;
                var done = routine == null || parentGone || !routine.MoveNext();
                if (done)
                {
                    if (!parentGone && ptr._Stack.Count > 0)
                        ptr._Stack.RemoveAt(ptr._Stack.Count - 1);
                    else
                    {
                        ptr._Parent = null;
                        _Running.RemoveAt(index);
                        if (ptr._Job != null) ptr._Job.Counter--;
                    }
                }
                else
                {
                    var current = routine.Current;
                    if (current == _W)
                    {
                        ptr._WakeTime = now + _W.millis;
                    }
                    else if (current is IEnumerable enm)
                    {
                        ptr._Stack.Add(enm.GetEnumerator());
                    }
                }
                index--;
            }
        }


        class _Wait
        {
            internal float millis;
        }
    }
}