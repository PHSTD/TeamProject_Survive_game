using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EventManager : MonoBehaviour
{
    // public static EventManager Instance { get; private set; }

    // public UnityEvent OnReturnShelter = new();

    // private Dictionary<int, GameEventData> eventDict = new();
    // private int _curGameDay;
    // private GameEventData _curEventData;

    // public List<GameEventData> CurEvents = new();
    // public List<GameEventData> FinishedEvents = new();
    // public List<Button> CurEventButtons = new();
    // public List<Button> FinishedButtons = new();

    // [SerializeField] public Transform EventContents;
    // [SerializeField] private Button eventButtonPrefab;
    // [SerializeField] public EventUI EventUI;

    // private void Awake()
    // {
    //     if (Instance != null)
    //     {
    //         Destroy(gameObject);
    //         return;
    //     }

    //     Instance = this;
    //     DontDestroyOnLoad(gameObject);

    //     InitializeCurrentDay();
    //     LoadAllEvents();
    // }

    // private void Start()
    // {
    //     StartCoroutine(DelayedEventStart());
    //     OnReturnShelter.AddListener(OnReturnShelterScene);
    // }

    // private IEnumerator DelayedEventStart()
    // {
    //     yield return new WaitUntil(() => Storage.Instance != null);
    //     EventStart();
    // }

    // private void OnReturnShelterScene()
    // {
    //     StartCoroutine(CoOnReturnShelterScene());
    // }

    // private IEnumerator CoOnReturnShelterScene()
    // {
    //     yield return new WaitForSeconds(1f);

    //     bool isComplete = false;
    //     Debug.Log("코루틴 진입 체크!");
    //     foreach(Button obj in CurEventButtons)
    //     {
    //         if(obj == null)
    //         {
    //             isComplete = true;
    //             break;
    //         }
    //     }
    //     Debug.Log("null 체크!");

    //     if(CurEvents.Count() != CurEvents.Count() || isComplete)
    //     {
    //         CurEventButtons.Clear();
    //         FinishedButtons.Clear();
    //         SafeDestroyButtons();
    //         yield return new WaitForSeconds(1f);

    //         for (int i = 0; i < CurEvents.Count(); i++)
    //         {
    //             CreateEventButton(i);
    //         }
    //         AttachButton(CurEventButtons);
    //     }
    //     for (int i = 0; i < FinishedEvents.Count(); i++)
    //     {
    //         CreateFinishedButton(i);
    //     }

    //     AttachButton(FinishedButtons);
    // }

    // private void RefreshButton()
    // {
    //     if (CurEvents.Count() != CurEventButtons.Count())
    //     {
    //         for (int i = 0; i < CurEvents.Count(); i++)
    //         {
    //             CreateEventButton(i);
    //         }
    //         AttachButton(CurEventButtons);
    //     }

    //     for (int i = 0; i< FinishedEvents.Count(); i++)
    //     {
    //         CreateFinishedButton(i);
    //     }
    // }   

    // private void InitializeCurrentDay()
    // {
    //     if (StatusSystem.Instance != null)
    //     {
    //         _curGameDay = StatusSystem.Instance.GetCurrentDay();
    //     }
    //     else
    //     {
    //         Debug.LogWarning("StatusSystem.Instance is not available yet. Using default value for current day.");
    //         _curGameDay = 1;
    //     }
    // }

    // private void LoadAllEvents()
    // {
    //     var events = Resources.LoadAll<GameEventData>("Events");
    //     foreach (var e in events)
    //     {
    //         if (!eventDict.ContainsKey(e.id))
    //         {
    //             eventDict[e.id] = e;
    //         }
    //         else
    //         {
    //             Debug.LogWarning($"중복된 이벤트 ID : {e.id}");
    //         }
    //     }

    //     Debug.Log($"이벤트 {eventDict.Count}개 로드 완료");
    // }

    // public GameEventData GetEventById(int id)
    // {
    //     eventDict.TryGetValue(id, out var result);
    //     return result;
    // }

    // public void EventStart()
    // {
    //     try
    //     {
    //         Debug.Log("EventStart 시작");

    //         // 기존 이벤트와 버튼 정리
    //         //ClearAllEvents();

    //         // 새로운 이벤트 생성
    //         GenerateDailyEvent();
    //         GenerateRandomEvent();

    //         OnReturnShelterScene();
    //         RefreshEventUI();

    //         // UI 업데이트
    //         RefreshEventUI();

    //         EventUI.Instance?.UpdateUncompletedEventList();

    //         // 인덱스 동기화 검증

    //         Debug.Log("EventStart 완료");
    //     }
    //     catch (System.Exception ex)
    //     {
    //         Debug.LogError($"EventStart에서 오류 발생: {ex.Message}\n{ex.StackTrace}");
    //     }
    // }

    // // 모든 이벤트와 버튼을 안전하게 정리하는 함수
    // private void ClearAllEvents()
    // {
    //     Debug.Log("ClearAllEvents 시작");

    //     // 1. 먼저 기존 버튼들을 안전하게 정리
    //     SafeDestroyButtons();

    //     // 2. 리스트 초기화
    //     CurEvents.Clear();
    //     CurEventButtons.Clear();

    //     Debug.Log("ClearAllEvents 완료");
    // }

    // // 안전한 버튼 파괴 함수
    // private void SafeDestroyButtons()
    // {
    //     Debug.Log($"SafeDestroyButtons 시작 - CurEventButtons: {CurEventButtons.Count}개");

    //     // eventContents가 null인지 확인
    //     if (EventContents == null)
    //     {
    //         Debug.LogWarning("eventContents is null!");
    //         return;
    //     }

    //     // 1. CurEventButtons 리스트의 버튼들을 안전하게 파괴
    //     for (int i = CurEventButtons.Count - 1; i >= 0; i--)
    //     {
    //         if (CurEventButtons[i] != null)
    //         {
    //             try
    //             {
    //                 // 이벤트 리스너 제거
    //                 CurEventButtons[i].onClick.RemoveAllListeners();

    //                 // 오브젝트 파괴
    //                 if (CurEventButtons[i].gameObject != null)
    //                 {
    //                     DestroyImmediate(CurEventButtons[i].gameObject);
    //                 }
    //             }
    //             catch (System.Exception ex)
    //             {
    //                 Debug.LogWarning($"버튼 파괴 중 오류: {ex.Message}");
    //             }
    //         }
    //     }

    //     // 2. eventContents의 모든 자식을 안전하게 파괴
    //     List<Transform> childrenToDestroy = new List<Transform>();

    //     // 먼저 모든 자식을 리스트에 수집
    //     for (int i = 0; i < EventContents.childCount; i++)
    //     {
    //         Transform child = EventContents.GetChild(i);
    //         if (child != null)
    //         {
    //             childrenToDestroy.Add(child);
    //         }
    //     }

    //     // 수집된 자식들을 파괴
    //     foreach (Transform child in childrenToDestroy)
    //     {
    //         if (child != null && child.gameObject != null)
    //         {
    //             try
    //             {
    //                 // 버튼 컴포넌트가 있다면 리스너 제거
    //                 Button button = child.GetComponent<Button>();
    //                 if (button != null)
    //                 {
    //                     button.onClick.RemoveAllListeners();
    //                 }

    //                 DestroyImmediate(child.gameObject);
    //             }
    //             catch (System.Exception ex)
    //             {
    //                 Debug.LogWarning($"자식 오브젝트 파괴 중 오류: {ex.Message}");
    //             }
    //         }
    //     }

    //     Debug.Log("SafeDestroyButtons 완료");
    // }

    // // 기존의 DettachButton 함수를 더 안전하게 수정
    // public void DettachButton()
    // {
    //     if (EventContents == null)
    //     {
    //         Debug.LogWarning("eventContents is null in DettachButton!");
    //         return;
    //     }

    //     // 안전한 방법으로 자식들을 분리
    //     List<Transform> children = new List<Transform>();

    //     // 현재 자식들을 리스트에 복사
    //     for (int i = 0; i < EventContents.childCount; i++)
    //     {
    //         Transform child = EventContents.GetChild(i);
    //         if (child != null)
    //         {
    //             children.Add(child);
    //         }
    //     }

    //     // 복사된 리스트를 통해 안전하게 분리
    //     foreach (Transform child in children)
    //     {
    //         if (child != null && child.parent == EventContents)
    //         {
    //             try
    //             {
    //                 child.SetParent(null);
    //             }
    //             catch (System.Exception ex)
    //             {
    //                 Debug.LogWarning($"자식 분리 중 오류: {ex.Message}");
    //             }
    //         }
    //     }
    // }

    // // 인덱스 동기화 검증 함수
    // private void ValidateIndexSync()
    // {
    //     if (CurEvents.Count != CurEventButtons.Count)
    //     {
    //         Debug.LogError($"인덱스 불일치 발견! CurEvents: {CurEvents.Count}, CurEventButtons: {CurEventButtons.Count}");
    //         RecoverIndexSync();
    //     }
    //     else
    //     {
    //         Debug.Log($"인덱스 동기화 확인: {CurEvents.Count}개 이벤트");
    //     }
    // }

    // // 인덱스 동기화 복구 함수
    // private void RecoverIndexSync()
    // {
    //     Debug.LogWarning("인덱스 동기화 복구 시도 중...");

    //     // 기존 버튼들 모두 제거
    //     SafeDestroyButtons();
    //     CurEventButtons.Clear();

    //     // 이벤트 수만큼 버튼 재생성
    //     for (int i = 0; i < CurEvents.Count; i++)
    //     {
    //         CreateEventButton(i);
    //     }

    //     // UI 재구성
    //     RefreshEventUI();
    // }

    // public void AttachButton(List<Button> list)
    // {
    //     if (EventContents == null)
    //     {
    //         Debug.LogWarning("eventContents is null in AttachButton!");
    //         return;
    //     }

    //     for (int i = 0; i < list.Count; i++)
    //     {
    //         // null 체크 강화
    //         if (list[i] == null || list[i].gameObject == null)
    //         {

    //             Debug.LogWarning($"Button at index {i} is null or destroyed!");
    //             continue;
    //         }

    //         try
    //         {
    //             list[i].transform.SetParent(EventContents);

    //             if (list == CurEventButtons)
    //             {
    //                 int capturedIndex = i;
    //                 list[i].onClick.RemoveAllListeners();

    //                 list[i].onClick.AddListener(() =>
    //                 {
    //                     if (IsValidEventIndex(capturedIndex))
    //                     {
    //                         EventUI?.SetEventListTitleText(CurEvents[capturedIndex], capturedIndex);
    //                     }
    //                     else
    //                     {
    //                         Debug.LogError($"Invalid event index: {capturedIndex}, CurEvents.Count: {CurEvents.Count}");
    //                     }
    //                 });
    //             }
    //         }
    //         catch (System.Exception ex)
    //         {
    //             Debug.LogError($"AttachButton에서 오류 발생: {ex.Message}");
    //         }
    //     }
    // }

    // // 유효한 이벤트 인덱스인지 확인
    // private bool IsValidEventIndex(int index)
    // {
    //     return index >= 0 && index < CurEvents.Count && index < CurEventButtons.Count;
    // }

    // // UI 새로고침
    // private void RefreshEventUI()
    // {
    //     try
    //     {
    //         DettachButton();
    //         AttachButton(CurEventButtons);
    //         AttachButton(FinishedButtons);
    //     }
    //     catch (System.Exception ex)
    //     {
    //         Debug.LogError($"RefreshEventUI에서 오류 발생: {ex.Message}");
    //     }
    // }

    // public void GenerateDailyEvent()
    // {
    //     // 중복 방지
    //     if (CurEvents.Any(e => e.id == 10001))
    //     {
    //         Debug.Log("DailyEvent(id:10001)는 이미 존재하여 중복 생성하지 않음.");
    //         return;
    //     }

    //     if (!eventDict.ContainsKey(10001))
    //     {
    //         Debug.LogError("Daily event (id: 10001) not found in eventDict!");
    //         return;
    //     }

    //     var dailyEvent = eventDict[10001];
    //     dailyEvent.GenerateRandomDuraValue();
    //     dailyEvent.isComplete = false;

    //     AddEventWithButton(dailyEvent);
    //     Debug.Log("내구도 수리 이벤트 발생(매일)");
    // }

    // // 이벤트와 버튼을 동시에 추가하는 함수
    // private void AddEventWithButton(GameEventData eventData)
    // {
    //     if (eventData == null)
    //     {
    //         Debug.LogError("eventData is null in AddEventWithButton!");
    //         return;
    //     }

    //     try
    //     {
    //         // 1. 이벤트 추가
    //         CurEvents.Add(eventData);

    //         // 2. 버튼 생성 및 추가
    //         int newIndex = CurEvents.Count - 1;
    //         CreateEventButton(newIndex);

    //         Debug.Log($"이벤트 추가: {eventData.title}, 인덱스: {newIndex}");
    //     }
    //     catch (System.Exception ex)
    //     {
    //         Debug.LogError($"AddEventWithButton에서 오류 발생: {ex.Message}");
    //     }
    // }

    // // 버튼 생성 함수
    // private void CreateEventButton(int eventIndex)
    // {
    //     if (eventButtonPrefab == null)
    //     {
    //         Debug.LogError("eventButtonPrefab is null!");
    //         return;
    //     }

    //     try
    //     {
    //         Button newButton = Instantiate(eventButtonPrefab);
    //         if (newButton != null)
    //         {
    //             CurEventButtons.Add(newButton);
    //             EventUI?.SetEventSubUIBtnTitle(newButton.gameObject, eventIndex);
    //         }
    //         else
    //         {
    //             Debug.LogError("Failed to instantiate event button!");
    //         }
    //     }
    //     catch (System.Exception ex)
    //     {
    //         Debug.LogError($"CreateEventButton에서 오류 발생: {ex.Message}");
    //     }
    // }

    // private void CreateFinishedButton(int eventIndex)
    // {
    //     if (eventButtonPrefab == null)
    //     {
    //         Debug.LogError("eventButtonPrefab is null!");
    //         return;
    //     }

    //     try
    //     {
    //         Button newButton = Instantiate(eventButtonPrefab);
    //         if (newButton != null)
    //         {
    //             FinishedButtons.Add(newButton);
    //         }
    //         else
    //         {
    //             Debug.LogError("Failed to instantiate event button!");
    //         }
    //     }
    //     catch (System.Exception ex)
    //     {
    //         Debug.LogError($"CreateEventButton에서 오류 발생: {ex.Message}");
    //     }
    // }

    // public void GenerateRandomEvent()
    // {
    //     try
    //     {
    //         int currentDay = StatusSystem.Instance.GetCurrentDay();
    //         List<int> availableEvents = new List<int>();

    //         // 일차별 사용 가능한 이벤트 ID 결정
    //         if (currentDay <= 3)
    //         {
    //             availableEvents.AddRange(new int[] { 10002, 10003, 10004 });
    //             Debug.Log("1~3일차 이벤트 풀 설정");
    //         }
    //         else if (currentDay <= 6)
    //         {
    //             availableEvents.AddRange(new int[] { 10002, 10003, 10004, 10005, 10006 });
    //             Debug.Log("4~6일차 이벤트 풀 설정");
    //         }
    //         else
    //         {
    //             availableEvents.AddRange(new int[] { 10005, 10006, 10007, 10008, 10009 });
    //             Debug.Log("7일차~ 이벤트 풀 설정");
    //         }

    //         int eventsToGenerate = (currentDay <= 3) ? 1 : 2;

    //         for (int i = 0; i < eventsToGenerate && availableEvents.Count > 0; i++)
    //         {
    //             int randomIndex = Random.Range(0, availableEvents.Count);
    //             int selectedEventID = availableEvents[randomIndex];

    //             if(CurEvents.Any(e => e.id == selectedEventID))
    //             {
    //                 Debug.Log($"Random Index : {randomIndex}, num : {selectedEventID}");
    //                 availableEvents.RemoveAt(randomIndex);
    //                 randomIndex = Random.Range(0, availableEvents.Count);

    //                 selectedEventID = availableEvents[randomIndex];

    //                 Debug.Log($"Random Index : {randomIndex}, num : {selectedEventID}");
    //                 Debug.Log("**중복정리**");
    //             }
    //                 Debug.Log("**중복없음**");

    //             // 이미 존재하는 이벤트인지 확인
    //             if (eventDict.ContainsKey(selectedEventID))
    //             {
    //                 var eventData = eventDict[selectedEventID];
    //                 eventData.isComplete = false;

    //                 AddEventWithButton(eventData);
    //                 Debug.Log($"랜덤 이벤트 생성: ID {selectedEventID}");
    //             }

    //             availableEvents.RemoveAt(randomIndex);
    //             for (int j = 0; j <availableEvents.Count(); j++)
    //             {
    //                 Debug.Log(availableEvents[j]);
    //             }
    //         }
    //     }
    //     catch (System.Exception ex)
    //     {
    //         Debug.LogError($"GenerateRandomEvent에서 오류 발생: {ex.Message}");
    //     }
    // }

    // public bool DetermineEventComplete(GameEventData data)
    // {
    //     if (data == null)
    //     {
    //         Debug.LogError("GameEventData is null!");
    //         return false;
    //     }

    //     if (Storage.Instance == null)
    //     {
    //         Debug.LogError("Storage.Instance is null!");
    //         return false;
    //     }

    //     if (data.requiredItemA != null && data.requiredItemB == null)
    //         return data.requiredAmountA <= Storage.Instance.GetItemCount(data.requiredItemA);

    //     if (data.requiredItemA != null && data.requiredItemB != null)
    //         return data.requiredAmountB <= Storage.Instance.GetItemCount(data.requiredItemB);

    //     return false;
    // }

    // public void EventEffect(GameEventData data)
    // {
    //     if (data.isComplete)
    //     {
    //         StatusSystem.Instance.SetPlusDurability(data.PlusDurability);
    //         StatusSystem.Instance.SetPlusOxygenGainMultiplier(data.PlusOxygenEfficiency);
    //         StatusSystem.Instance.SetPlusEnergyGainMultiplier(data.PlusEnergyEfficiency);
    //     }
    //     else
    //     {
    //         StatusSystem.Instance.SetMinusDurability(data.RandomMinusDuraValue);
    //         StatusSystem.Instance.SetMinusDurability(data.MinusDurability);
    //         StatusSystem.Instance.SetMinusEnergy(data.MinusEnergy);
    //         StatusSystem.Instance.SetMinusOxygen(data.MinusOxygen);
    //         StatusSystem.Instance.SetMinusOxygenGainMultiplier(data.MinusOxygenEfficiency);
    //         StatusSystem.Instance.SetMinusEnergyGainMultiplier(data.MinusEnergyEfficiency);
    //     }

    //     Debug.Log($"완료여부 점검 : [EventEffect] {data.title}, isComplete: {data.isComplete}");
    // }

    // public void EventClear(GameEventData data, int eventIndex)
    // {
    //     if (!IsValidEventIndex(eventIndex))
    //     {
    //         Debug.LogError($"Invalid event index: {eventIndex}, CurEvents.Count: {CurEvents.Count}");
    //         return;
    //     }

    //     try
    //     {
    //         // 아이템 제거
    //         if (data.requiredItemA != null)
    //         {
    //             Storage.Instance.RemoveItem(data.requiredItemA, data.requiredAmountA);
    //         }

    //         if (data.requiredItemB != null)
    //         {
    //             Storage.Instance.RemoveItem(data.requiredItemB, data.requiredAmountB);
    //         }

    //         // 버튼을 완료된 버튼 리스트로 이동
    //         Button clearedButton = CurEventButtons[eventIndex];
    //         if (clearedButton != null)
    //         {
    //             clearedButton.interactable = false;
    //             FinishedButtons.Add(clearedButton);
    //         }
    //         FinishedEvents.Add(CurEvents[eventIndex]);

    //         // 동시에 제거하여 인덱스 동기화 유지
    //         CurEvents.RemoveAt(eventIndex);
    //         CurEventButtons.RemoveAt(eventIndex);

    //         // 남은 버튼들의 이벤트 핸들러 재설정
    //         UpdateRemainingButtonHandlers();

    //         // UI 새로고침
    //         RefreshEventUI();
    //         EventUI.Instance?.UpdateUncompletedEventList();

    //         Debug.Log($"이벤트 클리어: {data.title}, 남은 이벤트 수: {CurEvents.Count}");
    //     }
    //     catch (System.Exception ex)
    //     {
    //         Debug.LogError($"EventClear에서 오류 발생: {ex.Message}");
    //     }
    // }

    // // 남은 버튼들의 이벤트 핸들러 재설정
    // private void UpdateRemainingButtonHandlers()
    // {
    //     for (int i = 0; i < CurEventButtons.Count; i++)
    //     {
    //         if (CurEventButtons[i] != null && CurEventButtons[i].gameObject != null)
    //         {
    //             int capturedIndex = i;
    //             CurEventButtons[i].onClick.RemoveAllListeners();
    //             CurEventButtons[i].onClick.AddListener(() =>
    //             {
    //                 if (IsValidEventIndex(capturedIndex))
    //                 {
    //                     EventUI?.SetEventListTitleText(CurEvents[capturedIndex], capturedIndex);
    //                 }
    //             });
    //         }
    //     }
    // }

    // public List<GameEventData> GetUnCompletedEvents()
    // {
    //     return CurEvents.Where(e => !e.isComplete).ToList();
    // }
    public Dictionary<int, EventController> eventDict = new();
    [SerializeField] public Transform EventContents;
    public List<GameObject> eventButtons = new();
    private void Awake()
    {
        eventButtons = EventContents.Cast<Transform>()
            .Select(t => t.gameObject)
            .ToList();
        foreach (var go in eventButtons)
        {
            if (go != null)
            {
                go.SetActive(false); // 초기에는 모든 버튼을 비활성화
            }
        }
    }

    private void RefreshButtons() //지금까지 버튼을 새로 만들필요 있을 때, 만드는 거 대신 이거 호출만 하면 된다.
    {
        foreach (var controller in eventDict.Values)
        {
            controller.RefreshButton(); // 각 버튼의 EventController를 통해 새로고침
            // EventController 내부에서 버튼 활성화/비활성화 처리
            if (controller.gameObject.activeSelf)
            {
                if (controller.button.interactable == true)
                {
                    controller.gameObject.SetAsFirstSibling(); // 활성화된 버튼
                }
            }
            else
            {
                controller.gameObject.SetAsLastSibling(); //없는 버튼, 완료된 버튼은 자동으로 중간으로
            }
        }
        //foreach eventDic에서  Eventcontroller에서 각 이벤트들의 상태에 맞춰서
        //각자 활성화 시킵니다. 그리고, 배치(child의 index?)를 바꿔서 활성/비활성 버튼 배치합니다
    }
    private void DisableButtons()
    {
        eventButtons.ForEach(button => button.SetActive(false));
    }

    public string GetEventDataString()
    {
        string eventDataString = "{";
        foreach (var kvp in eventDict)
        {
            int id = kvp.Key;
            EventController controller = kvp.Value;
            int status = controller.GetEventStatus(); // 0: 미출현, 1: 활성화, 2: 완료
            eventDataString += $"{id}:{status},";
        }
        eventDataString = eventDataString.TrimEnd(',') + "}"; // 마지막 쉼표 제거
        return eventDataString;
    }
    //저장

    // 저장시 어떤걸? -> eventDict에 있는 EventController들의 상태를 저장합니다. -> "id(숫자)" : 0,1,2 * 0은 미출현, 1은 활성화, 2는 완료된 이벤트 -> eventDict에 eventController들이 있는데 거기서 가져오면됨


    //불러오기


}