using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Structure : MonoBehaviour, IInteractable
{
    public string StructureName = "New Structure"; // ������ �̸�
    public int InteractCount = 0; // ��ȣ�ۿ� Ƚ��
    public abstract void Interact();
}
