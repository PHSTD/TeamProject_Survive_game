using UnityEngine;

namespace Test
{
    [CreateAssetMenu(menuName = "Item/Test Tool Item")]
    public class TestToolItem : Item
    {
        [Header("Tool Specific Info")]
        public float miningPower = 1f; // ä�� ������ ä���� (��: ������ ü���� ��� ��)
        public ToolType toolType;       // �� ������ ���� (��: ���, ����, �� ��) 
        public GameObject toolPrefab;     // �÷��̾� �տ� �鸱 ������Ʈ ������
        public GameObject toolObject;     // ������ ���� ������Ʈ (��: ���, ���� ��)
        public TestToolAction toolAction; // ������ �ൿ�� �����ϴ� ��ũ��Ʈ

        public override void Use(GameObject user)
        {
            // �θ� Item Ŭ������ Use �޼ҵ带 ȣ���մϴ�.
            //base.Use(user); // ����� ��
            toolAction.Action((int)miningPower);
            Debug.Log($"{itemName}�� ����߽��ϴ�. (������ ����)");
        }
    }

    public enum ToolType
    {
        //������ ���� ����
        None,
        Pickaxe,
        Axe,
        Shovel
    }
}

