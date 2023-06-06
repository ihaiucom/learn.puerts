using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zeng;

public class NewBehaviourScript : MonoBehaviour
{
    private Zeng.Coroutine t;
    // Start is called before the first frame update
    void Start()
    {
        //TestAwait3.Run();
        t = Zeng.CoroutineManager.I.Start(CorutineTest());
        //Zeng.CoroutineManager.I.Start(AAAA());

        //Zeng.CoroutineManager.I.Start(TestStop(), "TestStop");
    }

    IEnumerator TestStop()
    {
        yield return new Zeng.WaitForSeconds(1);
        Zeng.CoroutineManager.I.Stop(t);
    }

    // Update is called once per frame
    void Update()
    {
        if(Zeng.CoroutineManager.I.Count > 0)
        {
            Zeng.CoroutineManager.I.Update(Time.deltaTime, Time.frameCount);
        }


    }

    IEnumerator CorutineTest()
    {
        UnityEngine.Debug.Log(Time.frameCount + "[CoroutineTest] ��ʼ");
        yield return null;
        UnityEngine.Debug.Log(Time.frameCount + "[CoroutineTest] null");
        yield return new Zeng.WaitForSeconds(1);

        yield return AAAA();
        //UnityEngine.Debug.Log(Time.frameCount + "[CoroutineTest] AAAA 1");
        yield return AAAA();

        UnityEngine.Debug.Log(Time.frameCount + "[CoroutineTest] ����");
    }


    IEnumerator AAAA()
    {
        UnityEngine.Debug.Log(Time.frameCount + "[AAAA] ��ʼ");
        yield return new Zeng.WaitForSeconds(1);
        UnityEngine.Debug.Log(Time.frameCount + "[AAAA] ��ʼ 1");

        //yield break;
        //UnityEngine.Debug.Log(Time.frameCount + "[AAAA]  yield break;");
        yield return new Zeng.WaitForSeconds(1);
        //UnityEngine.Debug.Log(Time.frameCount + "[AAAA] ��ʼ 2");
        UnityEngine.Debug.Log(Time.frameCount + "[AAAA] ����");
    }
}
