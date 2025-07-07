using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


namespace Assets.WorkPlace.LHG.Scripts.EventSystem
{
    public class EventController : MonoBehaviour
    {
        private GameEventData data;
        private Storage storage = Storage.Instance;
        private Button button;
        [SerializeField] private GameEventData eventData;
        private enum EventState
        {
            Invalid,
            Valid,
            Done
        }
        private EventState eventState = EventState.Invalid;
        private void Awake()
        {
            data = eventData;
            button = GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(OnButtonClick);
                RefreshButton();
            }
            else
            {
                Debug.LogError("Button component is missing on the EventController GameObject.");
            }
        }
        public bool CanComplete()
        {
            if (data.requiredItemB != null)
            {
                if (storage.GetItemCount(data.requiredItemB) <= data.requiredAmountB)
                    return false;
            }
            if (storage.GetItemCount(data.requiredItemA) >= data.requiredAmountA)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public void ActivateEvent()
        {
            eventState = EventState.Valid;
        }
        public void CompleteEvent()
        {
            if (eventState == EventState.Valid)
            {
                if (CanComplete())
                {
                    OnEventCompleted();
                    // 이벤트 완료 후 UI 업데이트
                    EventUI.Instance.UpdateEventList();
                }
                else
                {
                    Debug.Log("이벤트를 완료할 수 없습니다. 필요한 아이템이 부족합니다.");
                }
            }
        }
        protected virtual void OnEventCompleted()
        {
            eventState = EventState.Done;
            // 필요한 아이템 제거
            if (data.requiredItemA != null)
            {
                storage.RemoveItem(data.requiredItemA, data.requiredAmountA);
            }
            if (data.requiredItemB != null)
            {
                storage.RemoveItem(data.requiredItemB, data.requiredAmountB);
            }
            // 이벤트 완료 후 처리 로직
            Debug.Log($"이벤트 {data.title}이 완료되었습니다.");
            refreshButton();
        }
        private void OnButtonClick()
        {
            EventUI.Instance.SetEventListTitleText(data, gameObject.GetSiblingIndex());
        }
        public void RefreshButton()
        {
            gameObject.SetActive(true);
            if (eventState == EventState.Invalid)
            {
                gameObject.SetActive(false);
            }
            else if (eventState == EventState.Valid)
            {
                button.SetInteractable(true);
            }
            else if (eventState == EventState.Done)
            {
                button.SetInteractable(false);
            }
        }
    }
}


