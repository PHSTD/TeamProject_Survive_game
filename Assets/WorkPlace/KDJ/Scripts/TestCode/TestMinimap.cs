using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMinimap : MonoBehaviour
{
    private void Update()
    {
        transform.position = new Vector3(
            PlayerManager.Instance.Player.transform.position.x,
            100f, // ������ ����
            PlayerManager.Instance.Player.transform.position.z
        );
    }
}
