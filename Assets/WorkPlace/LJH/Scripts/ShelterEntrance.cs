using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShelterEntrance : Structure
{
    [Tooltip("SceneSystem�� ��ϵ� �����͡� ������ ���ư��ϴ�.")]
    [SerializeField] private ResultUI _resultUI;

    public override void Interact()
    {
        _resultUI.OnResultUI();
        // SceneSystem.Instance.LoadShelterScene();
    }
}
