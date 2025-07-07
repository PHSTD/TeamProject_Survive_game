using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Image itemIcon;
    public CanvasGroup canvasGroup { get; private set; }

    public Item myItem { get; set; }
    public InventorySlot activeSlot { get; set; }

    [SerializeField] private TextMeshProUGUI quantityText;
    private int _currentQuantity;
    public int CurrentQuantity
    {
        get { return _currentQuantity; }
        set
        {
            _currentQuantity = value;
            if (quantityText != null)
            {
                // ������ 1�� ���ϸ� �����, �ƴϸ� ǥ��
                quantityText.gameObject.SetActive(_currentQuantity > 1);
                quantityText.text = _currentQuantity.ToString();
            }
        }
    }
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        itemIcon = GetComponent<Image>();
        // quantityText = GetComponentInChildren<TMP_Text>();

        if (canvasGroup == null) Debug.LogWarning("InventoryItem: CanvasGroup�� �����ϴ�! " + gameObject.name);
        if (itemIcon == null) Debug.LogWarning("InventoryItem: Image ������Ʈ�� �����ϴ�! " + gameObject.name);
    }

    public void Initialize(Item item, InventorySlot parent)
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
        if (itemIcon == null) 
        {
            itemIcon = GetComponent<Image>();
        }

        if (itemIcon == null)
        {
            Debug.LogError("InventoryItem: Initialize���� Image ������Ʈ�� ã�� �� �����ϴ�. ������ ������ Ȯ���ϼ���.");
            return; // �� �̻� �������� ����
        }

        activeSlot = parent;
        activeSlot.myItemUI = this;
        activeSlot.myItemData = item;
        myItem = item;
        itemIcon.sprite = item.icon;
        CurrentQuantity = 1;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1; // ���̰�
            canvasGroup.blocksRaycasts = true; // ����ĳ��Ʈ ��� (�巡�� �����ϵ���)
        }

        transform.SetParent(parent.transform); // �ʱ� �θ� ����
        transform.localPosition = Vector3.zero; //��ġ �ʱ�ȭ

    }

    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    if(eventData.button == PointerEventData.InputButton.Left)
    //    {
    //        Inventory.Instance.SetCarriedItem(this);
    //    }
    //}

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (myItem != null)
        {
            // SampleUIManager (�Ǵ� Inventory)�� SetItemDescription �޼��带 ȣ���Ͽ� ������ ǥ���մϴ�.
            if(SampleUIManager.Instance != null)
            SampleUIManager.Instance.SetItemDescription(myItem.description);
        }
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // ���콺�� ������ ������ ����� ������ ����ϴ�.
        SampleUIManager.Instance.SetItemDescription("");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canvasGroup == null) return;

        // Inventory���� CarriedItem ����
        Inventory.Instance.SetCarriedItem(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // ���� �������� ���ư����� ó���մϴ�.
        if (Inventory.CarriedItem != null)
        {
            if (Inventory.CarriedItem == null)
            {
                return;
            }

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            bool droppedOnUI = false;
            foreach (var result in results)
            {
                // ��� ����� InventorySlot�̰ų� InventoryItem(�ڱ� �ڽ� ����)�� �ƴ� �ٸ� UI ����� ���
                // (�� �κ��� ������Ʈ�� UI ������ ���� �� �����ϰ� ������ �� �ֽ��ϴ�.)
                if (result.gameObject.GetComponent<InventorySlot>() != null || result.gameObject.GetComponent<InventoryItem>() != null)
                {
                    droppedOnUI = true;
                    break;
                }
            }
            if (!droppedOnUI) // UI ���� ��ӵ��� �ʾҴٸ� ���忡 ����
            {
                Inventory.Instance.DropItemFromSlot(Inventory.CarriedItem);
            }
            else // UI ���� ��ӵǾ����� InventorySlot�� �ƴϾ��ų� ��� ó������ �ʾҴٸ� ���� ��ġ�� �ǵ���
            {
                if (Inventory.CarriedItem != null)
                {
                    if (Inventory.CarriedItem.activeSlot != null) // ���� ������ �ִٸ� �װ����� �ǵ���
                    {
                        Inventory.CarriedItem.activeSlot.SetItem(Inventory.CarriedItem);
                        // �ֹ� ����ȭ�� �ʿ��ϴٸ� ���⼭ �߰�
                        Inventory.Instance.CheckAndSyncSlotIfHotbar(Inventory.CarriedItem.activeSlot);
                    }
                    else // ���� ������ ���� �������̶�� (��: ���� �����Ǿ� ���콺�� �پ��ִ� ������)
                    {
                        Destroy(Inventory.CarriedItem.gameObject); // �ܼ��� �ı�
                    }
                    Inventory.CarriedItem = null; // ���������� ��� �ִ� ������ ����
                }
            }
        }
    }
}
