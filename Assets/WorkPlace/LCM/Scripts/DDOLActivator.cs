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
        // 싱글톤 초기화
        if (Instance != null && Instance != this)
        {
            // 이미 인스턴스가 존재하면 현재 오브젝트를 파괴 (중복 생성 방지)
            Destroy(this.gameObject);
            Debug.LogWarning($"DDOLActivator: Duplicate instance detected and destroyed: {this.name}");
            return; // 파괴되었으므로 더 이상 진행하지 않음
        }

        // 현재 오브젝트를 싱글톤 인스턴스로 설정
        Instance = this;

        // 이 오브젝트를 씬 전환 시 파괴되지 않도록 설정
        // 이 조건 (transform.parent == null)은 보통 최상위 오브젝트일 때만 DDOL하겠다는 의미입니다.
        // 만약 이 DDOLActivator가 다른 DDOL 오브젝트의 자식으로 들어갈 수도 있다면 조건문이 달라질 수 있습니다.
        // 하지만 일반적으로 DDOL 오브젝트는 최상위인 경우가 많습니다.
        if (transform.parent == null)
        {
            DontDestroyOnLoad(this.gameObject);
            Debug.Log($"DDOLActivator: '{this.name}'를 DontDestroyOnLoad로 설정했습니다.");
        }
        else
        {
            // 부모가 있는 경우, 부모가 DDOL 처리될 것이므로 별도로 DontDestroyOnLoad 호출하지 않음
            Debug.Log($"DDOLActivator: '{this.name}'는 부모 '{transform.parent.name}'에 의해 관리됩니다.");
        }

        // 제어할 오브젝트는 이 스크립트가 붙은 GameObject 자신입니다.
        _objectToControl = this.gameObject;

        // 씬 로드 이벤트를 구독 (Awake에서 OnEnable로 이동하여 명시적으로 호출)
        // Awake에서 OnEnable/OnDisable 구독을 처리하는 것이 좀 더 안전할 수 있습니다.
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log($"DDOLActivator: '{this.name}' OnSceneLoaded 이벤트 구독 완료.");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 현재 로드된 씬의 이름이 _targetSceneNames 배열에 포함되어 있는지 확인합니다.
        bool shouldBeActive = false;
        foreach (string sceneName in _targetSceneNames)
        {
            if (scene.name == sceneName)
            {
                shouldBeActive = true;
                break;
            }
        }

        // 제어할 오브젝트의 활성화 상태를 설정합니다.
        if (_objectToControl != null)
        {
            _objectToControl.SetActive(shouldBeActive);
            Debug.Log($"DDOL 오브젝트 '{_objectToControl.name}' 활성화 상태: {shouldBeActive} (씬: {scene.name})");
        }
    }
}
