using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zeng;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //TestAwait3.Run();
        Zeng.CoroutineManager.I.Start(CorutineTest());
        //Zeng.CoroutineManager.I.Start(AAAA());
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
        UnityEngine.Debug.Log("[CoroutineTest] 开始");
        //yield return new Zeng.WaitForSeconds(0.5f);

        //UnityEngine.Debug.Log("[CoroutineTest] wait for seconds <0.5f> over. begin wait for frame:3");
        yield return new Zeng.WaitForFrame(3);

        yield return AAAA();

        UnityEngine.Debug.Log("[CoroutineTest] 结束");
    }


    IEnumerator AAAA()
    {
        UnityEngine.Debug.Log("[AAAA] 开始");
        //yield return new Zeng.WaitForSeconds(0.5f);

        //UnityEngine.Debug.Log("[CoroutineTest] wait for seconds <0.5f> over. begin wait for frame:3");
        yield return new Zeng.WaitForFrame(1);
        UnityEngine.Debug.Log("[AAAA] 开始 1");
        yield return new Zeng.WaitForFrame(1);
        UnityEngine.Debug.Log("[AAAA] 开始 2");

        UnityEngine.Debug.Log("[AAAA] 结束");
    }
}
