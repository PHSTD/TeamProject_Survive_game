using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.WorkPlace.LHG.Scripts.EventSystem
{
    public class EventController : MonoBehaviour
    {
        public GameEventData data;
        private Storage storage;
        public Button button;
        [SerializeField] private GameEventData eventData;
        public enum EventState
        {
            Invalid,
            Valid,
            Done
        }
        public EventState eventState = EventState.Invalid;
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

        private void Start()
        {
            SetSubTitle();
        }

        private void SetSubTitle()
        {
            TMP_Text text = gameObject.GetComponentInChildren<TMP_Text>();
            text.SetText(data.title);
        }

        public bool CanComplete()
        {
            if (data.requiredItemB != null)
            {
                if (Storage.Instance.GetItemCount(data.requiredItemB) < data.requiredAmountB)
                    return false;
            }
            if (Storage.Instance.GetItemCount(data.requiredItemA) >= data.requiredAmountA)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void ActivateEvent()
        {
            eventState = EventState.Valid;
            RefreshButton();
        }
        public void CompleteEvent()
        {
            if (eventState == EventState.Valid)
            {
                if (CanComplete())
                {
                    OnEventCompleted();
                    // 이벤트 완료 후 UI 업데이트
                }
                else
                {
                    Debug.Log("이벤트를 완료할 수 없습니다. 필요한 아이템이 부족합니다.");
                }
                RefreshButton();
            }
        }
        protected virtual void OnEventCompleted()
        {
            eventState = EventState.Done;
            // 필요한 아이템 제거
            if (data.requiredItemA != null)
            {
                Storage.Instance.RemoveItem(data.requiredItemA, data.requiredAmountA);
            }
            if (data.requiredItemB != null)
            {
                Storage.Instance.RemoveItem(data.requiredItemB, data.requiredAmountB);
            }
            // 이벤트 완료 후 처리 로직
            Debug.Log($"이벤트 {data.title}이 완료되었습니다.");
            RefreshButton();
        }
        private void OnButtonClick()
        {
            EventUI.Instance.SetEventListTitleText(data, gameObject.transform.GetSiblingIndex());
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
                button.interactable = true;
            }
            else if (eventState == EventState.Done)
            {
                button.interactable =false;

            }
        }
    }
}


