using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineableResource : MonoBehaviour
{
    [Header("���� ����")]
    public int maxHealth;
    private float currentHealth;        //float ��! (���� ��������)

    [Header("��� ����")]
    public GameObject lootPrefab;
    public float lootLaunchForce = 5f;

    [Header("�ı� ����")]
    public float shrinkDuration = 2f;

    public float dropCheckCooldown = 1f; // 1�ʿ� �� ���� ��� üũ
    private float dropTimer = 0f;

    /* -------------------- Unity -------------------- */
    private void Start()
    {
        // HpCount ������Ʈ ����
        HpCount hpCount = GetComponent<HpCount>();

        if (hpCount != null)
        {
            maxHealth = hpCount.InitialHp;       // �ִ� ü�� ����
            currentHealth = maxHealth;           // ���� ü�� �ʱ�ȭ
        }
        else
        {
            Debug.LogWarning($"{gameObject.name}�� HpCount ������Ʈ�� �����ϴ�. ����Ʈ maxHealth({maxHealth}) ���.");
            currentHealth = maxHealth;
        }
    }

    public void TakeMiningDamage(float miningDamage)
    {
        if (currentHealth <= 0f) return;

        currentHealth -= miningDamage;
        currentHealth = Mathf.Max(currentHealth, 0f);
        //Debug.Log($"{gameObject.name}�� {miningDamage}��ŭ �������� �޾ҽ��ϴ�");
        //Debug.Log($"{gameObject.name}�� ���� ü���� {currentHealth}�Դϴ�");

        dropTimer -= Time.deltaTime;

        if (dropTimer <= 0f)
        {
            float r = Random.value;
            Debug.Log($"Random.value = {r}");
            Debug.Log($"������ {r}");
            if (r < 0.1f)
            {
                Debug.Log(">> ��� �߻�!");
                SpawnLoot();
                UpdateEmissionColor();
            }

            dropTimer = dropCheckCooldown; // ��Ÿ�� �ʱ�ȭ
        }

        if (currentHealth <= 0f)
        {
            UpdateEmissionColor();
            Debug.Log($"{gameObject.name} ä�� �Ϸ�!");
        }
    }

    /* -------------------- Loot Spawn -------------------- */
    private void SpawnLoot()
    {
        //if (lootPrefab == null) return;

        //float chance = Random.value; // 0.0 ~ 1.0

        //if (chance > 0.1f) return; // 10% Ȯ���� ���

        // 1) ���� ��ġ: ���� 1.2m + ���� ��0.5m ����
        Vector3 spread = Random.insideUnitSphere * 1.0f;
        spread.y = Mathf.Abs(spread.y);          // �Ʒ��� �������� �ʰ�
        Vector3 spawnPos = transform.position + Vector3.up * 1.2f + spread;

        GameObject loot = Instantiate(lootPrefab, spawnPos, Quaternion.identity);

        // 2) ���� ��ü�� �浹 ����
        if (TryGetComponent<Collider>(out var parentCol) &&
            loot.TryGetComponent<Collider>(out var lootCol))
        {
            Physics.IgnoreCollision(parentCol, lootCol);
        }

        // 3) ���� ����ġ �� ����
        if (loot.TryGetComponent<Rigidbody>(out var rb))
        {
            Vector3 dir = (Vector3.up * 0.8f + Random.insideUnitSphere * 0.2f).normalized;
            dir.y = Mathf.Max(dir.y, 0.5f);            // �ּ� ���� 0.5 �̻�
            rb.AddForce(dir * lootLaunchForce, ForceMode.Impulse);
        }
    }

    private void UpdateEmissionColor()
    {
        HpCount hpCount = GetComponent<HpCount>();
        if (hpCount == null) return;

        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer == null) return;

        Material[] materials = renderer.materials;
        Color emitColor = hpCount.GetEmitColor(Mathf.RoundToInt(currentHealth));

        // ������ ��Ƽ���� �̸��� (��Ȯ�� ��ġ�ϴ� �̸�)
        string[] excludeMaterialNames = { "rockTrack (Instance)", "rock (Instance)" };

        for (int i = 0; i < materials.Length; i++)
        {
            Material mat = materials[i];
            string matName = mat.name;

            // ��Ȯ�� ��ġ�ϴ� �̸��̸� ����
            bool isExcluded = System.Array.Exists(excludeMaterialNames, name => name == matName);

            if (!isExcluded && mat.HasProperty("_EmissionColor"))
            {
                mat.SetColor("_EmissionColor", emitColor);
                mat.EnableKeyword("_EMISSION");             
            }
        }
    }
}