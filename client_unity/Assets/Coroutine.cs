using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zeng
{
    public class CoroutineManager {

        private static CoroutineManager i;
        public static CoroutineManager I {
            get {
                if(i == null)
                {
                    i = new CoroutineManager();
                }
                return i;
            }
        }


        private LinkedList<Coroutine> coroutineList= new LinkedList<Coroutine>();
        private LinkedList<Coroutine> coroutineToStop = new LinkedList<Coroutine>();

        public int Count {
            get {
                return coroutineList.Count;
            }
        }

        public Coroutine Start(IEnumerator enumerator)
        {
            Coroutine c = new Coroutine(enumerator);
            coroutineList.AddLast(c);
            return c;
        }

        public void Stop(Coroutine coroutine) {
            coroutineToStop.AddLast(coroutine);
        }

        public void Update(float deltaTime, int frameIndex)
        {
            //UnityEngine.Debug.Log(frameIndex + " [CoroutineManager]  Update;");
            LinkedListNode<Coroutine> node = coroutineList.First;
            int i = 0;
            while(node != null)
            {
                Coroutine corutine = node.Value;

                //UnityEngine.Debug.Log(frameIndex + " [CoroutineManager]  Update node " + i);
                bool ret = false;
                bool toStop = false;
                if (corutine != null) {
                    toStop = coroutineToStop.Contains(corutine);
                    if(!toStop)
                    {
                        ret = corutine.MoveNext(deltaTime, frameIndex);
                        UnityEngine.Debug.Log(frameIndex + " [CoroutineManager]  Update corutine.MoveNext ret=" + ret);
                    }
                }

                if(!ret)
                {

                    UnityEngine.Debug.Log(frameIndex + " [CoroutineManager]  coroutineList.Remove(node);");
                    coroutineList.Remove(node);
                    if(toStop)
                    {
                        coroutineToStop.Remove(corutine);
                    }
                }

                node = node.Next;
                i++;
            }
        }

    }

    public sealed class Coroutine
    {
        private IEnumerator routine;
        public Coroutine(IEnumerator routine) {
            this.routine = routine;
        }

        public bool MoveNext(float deltaTime, int frameIndex)
        {
            if(routine == null) {
                return false;
            }

            IWait wait = routine.Current as IWait;
            bool moveNext = true;
            if(wait != null)
            {
                moveNext = wait.Tick(deltaTime, frameIndex);
            }

            if(!moveNext)
            {
                UnityEngine.Debug.Log(frameIndex + " [Coroutine] not movenext");
                return true;
            }
            else
            {
                UnityEngine.Debug.Log(frameIndex + " [Coroutine] movenext");
                return routine.MoveNext();
            }

        }

        public void Stop()
        {
            routine = null;
        }

    }

    public interface IWait {
        bool Tick(float deltaTime, int frameIndex);
    }

    public class WaitForSeconds : IWait {
        public float waitTime = 0;
        public WaitForSeconds(float time) {
            waitTime = time;
        }

        bool IWait.Tick(float deltaTime, int frameIndex)
        {
            waitTime -= deltaTime;
            UnityEngine.Debug.Log(frameIndex + " [WaitForSeconds] now left:" + waitTime);
            return waitTime <= 0;
        }
    }

    public class WaitForFrame : IWait {
        public int waitFrame = 0;
        public WaitForFrame(int frame)
        {
            waitFrame = frame;
        }

        bool IWait.Tick(float deltaTime, int frameIndex)
        {
            waitFrame--;
            UnityEngine.Debug.Log(frameIndex + " [WaitForFrame] now left:" + waitFrame);
            return waitFrame <= 0;
        }
    }
}
