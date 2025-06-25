using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineableResource : MonoBehaviour
{
    [Header("���� ����")]
    [SerializeField] public int maxHealth = 5;
    private float currentHealth;

    [Header("��� ������ ����")]
    [Tooltip("�ı� �� Ƣ��� ������ ������")]
    public GameObject lootPrefab;
    [Tooltip("������ Ƣ����� ��(Impulse)")]
    public float lootLaunchForce = 5f;
    [Tooltip("���� ���� ����߸��� �ʹٸ� ���� ����")]
    public int lootCount = 1;

    [Header("������ �����")]
    public float shrinkDuration = 2f;

    private bool isBeingMined = false;
    private bool lootSpawned = false;   // �ߺ� ��� ����

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (isBeingMined && currentHealth > 0)
        {
            const float miningDamagePerSecond = 1f;
            currentHealth -= miningDamagePerSecond * Time.deltaTime;
            if (currentHealth <= 0)
            {
                // �� ��� ������ �����
                SpawnLoot();
                // �� ������� ����
                StartCoroutine(FadeOutAndDestroy());
            }
        }
    }

    public void StartMining() => isBeingMined = true;
    public void StopMining() => isBeingMined = false;

    /* -------------------------- NEW: ������ ��� -------------------------- */
    private void SpawnLoot()
    {
        if (lootSpawned || lootPrefab == null) return;
        lootSpawned = true;

        for (int i = 0; i < lootCount; i++)
        {
            // ��¦ ���ʿ� ����
            Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
            GameObject loot = Instantiate(lootPrefab, spawnPos, Quaternion.identity);

            // Rigidbody�� ������ ���� ���� + �������� ���� ����
            if (loot.TryGetComponent<Rigidbody>(out var rb))
            {
                Vector3 dir = (Random.insideUnitSphere + Vector3.up).normalized;
                rb.AddForce(dir * lootLaunchForce, ForceMode.Impulse);
            }
        }
    }
    /* -------------------------------------------------------------------- */

    private IEnumerator FadeOutAndDestroy()
    {
        isBeingMined = false;

        Vector3 originalScale = transform.localScale;
        float timer = 0f;

        while (timer < shrinkDuration)
        {
            float t = timer / shrinkDuration;
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            timer += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
