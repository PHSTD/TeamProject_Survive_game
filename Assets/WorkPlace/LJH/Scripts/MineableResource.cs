using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineableResource : MonoBehaviour
{
    [Header("���� ����")]
    [SerializeField]public int maxHealth = 5;
    private float currentHealth;        //float ��! (���� ��������)

    [Header("��� ����")]
    public GameObject lootPrefab;
    public float lootLaunchForce = 5f;

    [Header("�ı� ����")]
    public float shrinkDuration = 2f;

    private bool isBeingMined = false;
    private int lastWholeHp;            //ü���� '���� �κ�'�� ����

    /* -------------------- Unity -------------------- */
    private void Start()
    {
        currentHealth = maxHealth;
        lastWholeHp = maxHealth;      // ó���� ���� ����
    }

    private void Update()
    {
        if (!isBeingMined || currentHealth <= 0f) return;

        //�ʴ� 1�� ������
        currentHealth -= 1f * Time.deltaTime;
        currentHealth = Mathf.Max(currentHealth, 0f);   // ���� ����

        //ü���� 1 �𿴴��� Ȯ��
        int currentWholeHp = Mathf.FloorToInt(currentHealth);
        if (currentWholeHp < lastWholeHp)        // �� ĭ ����������
        {
            SpawnLoot();                         // ��� 1��
            lastWholeHp = currentWholeHp;        // ���� ����
        }

        //0�� �Ǿ����� �ı� ����
        if (currentHealth <= 0f)
            StartCoroutine(FadeOutAndDestroy());
    }

    /* -------------------- Mining Control -------------------- */
    public void StartMining() => isBeingMined = true;
    public void StopMining() => isBeingMined = false;

    /* -------------------- Loot Spawn -------------------- */
    private void SpawnLoot()
    {
        if (lootPrefab == null) return;

        Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
        GameObject loot = Instantiate(lootPrefab, spawnPos, Quaternion.identity);

        if (loot.TryGetComponent<Rigidbody>(out var rb))
        {
            Vector3 dir = (Random.insideUnitSphere + Vector3.up).normalized;
            rb.AddForce(dir * lootLaunchForce, ForceMode.Impulse);
        }
    }

    /* -------------------- Fade & Destroy -------------------- */
    private IEnumerator FadeOutAndDestroy()
    {
        isBeingMined = false;

        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer == null)
        {
            Debug.LogWarning("MeshRenderer�� �����ϴ�.");
            Destroy(gameObject);
            yield break;
        }

        Material[] materials = renderer.materials; // ���׸��� �ν��Ͻ� �迭

        // �� ���׸����� �ʱ� ���� ����
        Color[] startColors = new Color[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            startColors[i] = materials[i].color;
        }

        float timer = 0f;

        while (timer < shrinkDuration)
        {
            float t = timer / shrinkDuration;
            float fade = Mathf.SmoothStep(1f, 0f, t);

            for (int i = 0; i < materials.Length; i++)
            {
                Color newColor = startColors[i];
                newColor.a = fade;
                materials[i].color = newColor;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}