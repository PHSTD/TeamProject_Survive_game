using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Structure : MonoBehaviour, IInteractable
{
    public string StructureName = "New Structure"; // 구조물 이름
    public int InteractCount = 0; // 상호작용 횟수
    public abstract void Interact();
}
