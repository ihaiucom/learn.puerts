using System.Collections;
using System.Collections.Generic;

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

        public Coroutine Start(IEnumerator enumerator, string name = null)
        {
            Coroutine c = new Coroutine(enumerator, name, this);
            coroutineList.AddLast(c);
            return c;
        }

        public void Stop(Coroutine coroutine) {
            if (coroutineList.Contains(coroutine))
            {
                coroutineToStop.AddLast(coroutine);
            }
        }

        public void Update(float deltaTime, int frameIndex)
        {
            ////UnityEngine.Debug.Log(frameIndex + " [CoroutineManager]  Update;");
            LinkedListNode<Coroutine> node = coroutineList.First;
            int i = 0;
            while(node != null)
            {
                Coroutine corutine = node.Value;

                ////UnityEngine.Debug.Log(frameIndex + " [CoroutineManager]  Update node " + i);
                bool ret = false;
                bool toStop = false;
                if (corutine != null) {
                    toStop = coroutineToStop.Contains(corutine);
                    if(!toStop)
                    {
                        ret = corutine.MoveNext(deltaTime, frameIndex);
                        //UnityEngine.Debug.Log(frameIndex + " [CoroutineManager]  Update corutine.MoveNext ret=" + ret + ", " + corutine);
                    }
                }

                if(!ret)
                {

                    //UnityEngine.Debug.Log(frameIndex + " [CoroutineManager]  coroutineList.Remove(node);  , " + corutine);
                    corutine.OnStop();
                    coroutineList.Remove(node);
                    coroutineToStop.Remove(corutine);
                }

                node = node.Next;
                i++;
            }
        }

    }

    public sealed class Coroutine : IWait
    {
        private CoroutineManager manager;
        private IEnumerator routine;
        public string name;

        private Coroutine subCoroutine;
        public Coroutine(IEnumerator routine, string name = null, CoroutineManager manager = null) {
            this.manager = manager;
            this.routine = routine;
            this.name = !string.IsNullOrEmpty(name) ? name : routine.ToString();
        }

        public bool MoveNext(float deltaTime, int frameIndex)
        {
            if(routine == null) {
                return false;
            }

            bool moveNext = true;
            if (subCoroutine == null) {
                //UnityEngine.Debug.Log(frameIndex + " [Coroutine]" + this + ", routine.Current=" + routine.Current);
                IWait wait = routine.Current as IWait;
                if (wait != null)
                {
                    moveNext = wait.Tick(deltaTime, frameIndex);
                }
                else
                {

                    IEnumerator enumerator = routine.Current as IEnumerator;
                    if (enumerator != null)
                    {
                        subCoroutine = new Coroutine(enumerator);
                    }
                }
            }

            if (subCoroutine != null)
            {


                bool subRet = subCoroutine.MoveNext(deltaTime, frameIndex);
                //UnityEngine.Debug.Log(frameIndex + " [Coroutine] subCoroutine" + this + ", subRet=" + subRet);
                if (!subRet)
                {
                    subCoroutine.OnStop();
                    subCoroutine = null;
                }
                moveNext = !subRet;
            }




            if (!moveNext)
            {
                //UnityEngine.Debug.Log(frameIndex + " [Coroutine] not movenext, " + this);
                return true;
            }
            else
            {
                //UnityEngine.Debug.Log(frameIndex + " [Coroutine] movenext, " + this);
                return routine.MoveNext();
            }

        }

        public void Stop()
        {
            if(this.manager != null)
            {
                this.manager.Stop(this);
            }
        }

        internal void OnStop()
        {
            //UnityEngine.Debug.Log(" [Coroutine] OnStop, " + this);
            routine = null;
            subCoroutine = null;
            manager = null;
        }


        bool IWait.Tick(float deltaTime, int frameIndex)
        {
            //UnityEngine.Debug.Log(frameIndex + " [Coroutine] Tick:" + this);
            return routine == null;
        }

        public override string ToString()
        {
            return "Coroutine(" + this.name + ")";
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
            return waitFrame <= 0;
        }
    }
}
