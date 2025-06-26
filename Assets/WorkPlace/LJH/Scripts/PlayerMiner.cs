using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMiner : MonoBehaviour
{
    private MineableResource currentTarget;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Resource"))
        {
            currentTarget = other.GetComponent<MineableResource>();
            if (currentTarget != null)
            {
                currentTarget.StartMining();
                Debug.Log("ä�� ����!");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Resource"))
        {
            if (currentTarget != null)
            {
                currentTarget.StopMining();
                Debug.Log("ä�� �ߴ�!");
            }
            currentTarget = null;
        }
    }
}
