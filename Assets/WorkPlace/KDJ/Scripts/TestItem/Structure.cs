using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Structure : MonoBehaviour, IInteractable
{
    public string StructureName = "New Structure"; // ������ �̸�
    public abstract void Interact();
}
