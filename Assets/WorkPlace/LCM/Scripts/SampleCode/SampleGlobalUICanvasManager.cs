using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPattern;

public class SampleGlobalUICanvasManager : Singleton<SampleGlobalUICanvasManager>
{
    private void Awake()
    {
        // ���� Ŭ������ Singleton<GlobalUICanvasManager>�� �ʱ�ȭ �޼��带 ȣ���մϴ�.
        // �� �ȿ��� DontDestroyOnLoad(this.gameObject)�� ó���˴ϴ�.
        SingletonInit();
    }
}
