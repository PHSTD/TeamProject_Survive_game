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

        // ���콺�� ��� �ٴϴ� ���������� ����
        Inventory.Instance.SetCarriedItem(this); 

        // �巡�� �߿��� �� ������ UI�� �ٸ� UI ��ҵ��� ����ĳ��Ʈ�� ���� �ʵ��� �մϴ�.
        canvasGroup.blocksRaycasts = false;
        // �巡�� �߿��� ��� �θ� Canvas�� �ֻ��� (DraggablesTransform)�� �����Ͽ� UI ������ ���� �������� �ʵ��� �մϴ�.
        transform.SetParent(Inventory.Instance.draggablesTransform);
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
            // InventorySlot�� SetItem �޼��带 �ٽ� ȣ���Ͽ� ���� �������� ���������ϴ�.
            activeSlot.SetItem(Inventory.CarriedItem);
        }
    }
}
