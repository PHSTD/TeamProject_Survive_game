using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DDOLActivator : MonoBehaviour
{
    [SerializeField]
    private string[] _targetSceneNames;

    private GameObject _objectToControl;

    private void Awake()
    {
        if(transform.parent == null)
        {
            DontDestroyOnLoad(this.gameObject);
        }

        _objectToControl = this.gameObject;
    }

    private void OnEnable()
    {
        // �� �ε� �̺�Ʈ�� �����մϴ�.
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // �� �ε� �̺�Ʈ ������ �����մϴ�. (������Ʈ�� �ı��� �� �߿�)
        SceneManager.sceneLoaded -= OnSceneLoaded;
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
