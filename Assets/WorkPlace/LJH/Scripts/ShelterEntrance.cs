using UnityEngine;

public class ShelterEntrance : Structure
{
    [Tooltip("SceneSystem�� ��ϵ� �����͡� ������ ���ư��ϴ�.")]
    [SerializeField] private ResultUI _resultUI;

    private int _interactCount = 0;
    private int _interactCount2 = 0;
    private float _timer = 0f;

    private void Update()
    {
        _timer += Time.deltaTime;
        if (DayScriptSystem.Instance.DayScript.activeSelf)
        {
            Time.timeScale = 0f;
        }
    }

    private void LateUpdate()
    {
        if (MenuSystem.Instance == null || MenuSystem.Instance.PauseMenu == null)
            return;

        if (DayScriptSystem.Instance == null || DayScriptSystem.Instance.DayScript == null)
            return;

        // TODO: 7/8
        if (DayScriptSystem.Instance.DayScript.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        // if (!MenuSystem.Instance.PauseMenu.activeSelf)
        // {
        //     if (!DayScriptSystem.Instance.DayScript.activeSelf && Cursor.lockState != CursorLockMode.Locked &&
        //         !SampleUIManager.Instance.inventoryPanel.activeSelf && !_resultUI.Canvas.enabled && !MenuSystem.Instance.BackToMenuDialog.activeSelf &&
        //         !MenuSystem.Instance.SettingMenu.activeSelf && !MenuSystem.Instance.GameOverDialog.activeSelf)
        //     {
        //         Cursor.lockState = CursorLockMode.Locked;
        //         Time.timeScale = 1f;
        //         Debug.Log("커서, 타임스케일 초기화됨.");
        //     }
        // }
    }

    public override void Interact()
    {
        if (_timer < 100)
        {
            _interactCount++;


            switch (_interactCount)
            {
                // 파밍씬에서 결과 화면에 스크립트 나오는 문제로 인해 Case문 안으로 이동
                case 1:
                    DayScriptSystem.Instance.ShowDialoguse();
                    Cursor.lockState = CursorLockMode.None;
                    DayScriptSystem.Instance.SetDialogue(DayScriptSystem.Instance.ShToBack2());
                    break;
                case 2:
                    DayScriptSystem.Instance.ShowDialoguse();
                    Cursor.lockState = CursorLockMode.None;
                    DayScriptSystem.Instance.SetDialogue(DayScriptSystem.Instance.ShToBack2());
                    break;
                case 3:
                    DayScriptSystem.Instance.ShowDialoguse();
                    Cursor.lockState = CursorLockMode.None;
                    DayScriptSystem.Instance.SetDialogue(DayScriptSystem.Instance.ShToBack3());
                    break;
                case 4:
                    DayScriptSystem.Instance.ShowDialoguse();
                    Cursor.lockState = CursorLockMode.None;
                    DayScriptSystem.Instance.SetDialogue(DayScriptSystem.Instance.ShToBack4());
                    break; 
                default:
                    _resultUI.OnResultUI();
                    break;
            }
        }
        else
        {
            _interactCount2++;

            if (_interactCount2 == 1)
            {
                DayScriptSystem.Instance.ShowDialoguse();
                Cursor.lockState = CursorLockMode.None;
                DayScriptSystem.Instance.SetDialogue(DayScriptSystem.Instance.ShToBack1());
            }
            else 
                _resultUI.OnResultUI();
        }
    }
}
