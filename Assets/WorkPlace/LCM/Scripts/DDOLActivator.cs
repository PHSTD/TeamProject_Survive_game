using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DDOLActivator : MonoBehaviour
{

    public static DDOLActivator Instance { get; private set; }

    [SerializeField]
    private string[] _targetSceneNames;

    private GameObject _objectToControl;

    private void Awake()
    {
        // �̱��� �ʱ�ȭ
        if (Instance != null && Instance != this)
        {
            // �̹� �ν��Ͻ��� �����ϸ� ���� ������Ʈ�� �ı� (�ߺ� ���� ����)
            Destroy(this.gameObject);
            Debug.LogWarning($"DDOLActivator: Duplicate instance detected and destroyed: {this.name}");
            return; // �ı��Ǿ����Ƿ� �� �̻� �������� ����
        }

        // ���� ������Ʈ�� �̱��� �ν��Ͻ��� ����
        Instance = this;

        // �� ������Ʈ�� �� ��ȯ �� �ı����� �ʵ��� ����
        // �� ���� (transform.parent == null)�� ���� �ֻ��� ������Ʈ�� ���� DDOL�ϰڴٴ� �ǹ��Դϴ�.
        // ���� �� DDOLActivator�� �ٸ� DDOL ������Ʈ�� �ڽ����� �� ���� �ִٸ� ���ǹ��� �޶��� �� �ֽ��ϴ�.
        // ������ �Ϲ������� DDOL ������Ʈ�� �ֻ����� ��찡 �����ϴ�.
        if (transform.parent == null)
        {
            DontDestroyOnLoad(this.gameObject);
            Debug.Log($"DDOLActivator: '{this.name}'�� DontDestroyOnLoad�� �����߽��ϴ�.");
        }
        else
        {
            // �θ� �ִ� ���, �θ� DDOL ó���� ���̹Ƿ� ������ DontDestroyOnLoad ȣ������ ����
            Debug.Log($"DDOLActivator: '{this.name}'�� �θ� '{transform.parent.name}'�� ���� �����˴ϴ�.");
        }

        // ������ ������Ʈ�� �� ��ũ��Ʈ�� ���� GameObject �ڽ��Դϴ�.
        _objectToControl = this.gameObject;

        // �� �ε� �̺�Ʈ�� ���� (Awake���� OnEnable�� �̵��Ͽ� ��������� ȣ��)
        // Awake���� OnEnable/OnDisable ������ ó���ϴ� ���� �� �� ������ �� �ֽ��ϴ�.
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log($"DDOLActivator: '{this.name}' OnSceneLoaded �̺�Ʈ ���� �Ϸ�.");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ���� �ε�� ���� �̸��� _targetSceneNames �迭�� ���ԵǾ� �ִ��� Ȯ���մϴ�.
        bool shouldBeActive = false;
        foreach (string sceneName in _targetSceneNames)
        {
            if (scene.name == sceneName)
            {
                shouldBeActive = true;
                break;
            }
        }

        // ������ ������Ʈ�� Ȱ��ȭ ���¸� �����մϴ�.
        if (_objectToControl != null)
        {
            _objectToControl.SetActive(shouldBeActive);
            Debug.Log($"DDOL ������Ʈ '{_objectToControl.name}' Ȱ��ȭ ����: {shouldBeActive} (��: {scene.name})");
        }
    }
}
