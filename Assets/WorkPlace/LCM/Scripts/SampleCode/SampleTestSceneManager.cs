using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleTestSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject sceneSpecificQuestUI;

    // Start is called before the first frame update
    void Start()
    {
        if (sceneSpecificQuestUI != null)
        {
            sceneSpecificQuestUI.SetActive(true); // �׻� Ȱ��ȭ
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
