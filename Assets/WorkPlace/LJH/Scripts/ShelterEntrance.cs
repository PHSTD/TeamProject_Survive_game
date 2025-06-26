using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShelterEntrance : MonoBehaviour, IInteractable
{
    [Tooltip("SceneSystem�� ��ϵ� �����͡� ������ ���ư��ϴ�.")]
    public void Interact()
    {
        SceneSystem.Instance.LoadShelterScene();
    }
}
