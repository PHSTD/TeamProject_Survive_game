using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftingItemUISlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] public Image itemIcon;

    // 이 슬롯이 어떤 레시피를 나타내는지 저장할 변수
    private Recipe _assignedRecipe;
    private CraftingManager _craftingManager;
    // Start is called before the first frame update

    // UI를 설정하고 레시피를 할당하는 메서드
    public void SetUI(Recipe recipeToAssign, CraftingManager manager)
    {
        _assignedRecipe = recipeToAssign;
        _craftingManager = manager;

        if (recipeToAssign != null && recipeToAssign.craftedItem != null)
        {
            if (itemIcon != null && recipeToAssign.craftedItem.icon != null)
            {
                itemIcon.sprite = recipeToAssign.craftedItem.icon;
            }
            else if (itemIcon != null) // 아이콘 스프라이트가 없을 경우 기본값 설정
            {
                itemIcon.sprite = null; // 또는 기본 스프라이트 할당
            }

        }
        else
        {
            // 레시피 정보가 없을 때 UI 초기화
            if (itemIcon != null) itemIcon.sprite = null;
        }
    }

    // 클릭 이벤트 처리 (이 슬롯을 클릭했을 때 호출될 함수)
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("CraftingItemUISlot 클릭됨: " + _assignedRecipe?.craftedItem?.itemName);
        if (_assignedRecipe != null)
        {
            _craftingManager.SelectRecipe(_assignedRecipe);
        }
    }
}
