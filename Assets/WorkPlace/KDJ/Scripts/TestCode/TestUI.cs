using DesignPattern;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestUI : MonoBehaviour
{
    [SerializeField] private GameObject _interactUI;
    [SerializeField] private TMP_Text _itemName;
    [SerializeField] private TMP_Text _air;
    [SerializeField] private TMP_Text _electric;
    [SerializeField] private Image _interactDelay;
    [SerializeField] private Image _hotbar1;
    [SerializeField] private Image _hotbar2;
    [SerializeField] private Image _hotbar3;
    [SerializeField] private GameObject _shift;
    [SerializeField] private GameObject _space;

    private ObseravableProperty<bool> _isInInteract = new();
    private int _curhotbar;

    private void Start()
    {
        _isInInteract.Subscribe(SetInteractUI);
        PlayerManager.Instance.AirGauge.Subscribe(SetTextUI);
        PlayerManager.Instance.ElecticGauge.Subscribe(SetTextUI);
        SetTextUI(1);
    }

    private void Update()
    {
        if (PlayerManager.Instance.Player == null) return;
        SetInteractText();
        SetHotbarHighlight();
        SetInteractDelay(PlayerManager.Instance.InteractDelay);
    }

    private void FixedUpdate()
    {
        ToggleKeyUI();
    }

    private void OnDestroy()
    {
        _isInInteract.Unsubscribe(SetInteractUI);
        PlayerManager.Instance.AirGauge.Unsubscribe(SetTextUI);
        PlayerManager.Instance.ElecticGauge.Unsubscribe(SetTextUI);
    }

    private void SetInteractUI(bool value)
    {
        if (_isInInteract.Value)
        {
            _interactUI.SetActive(true);

        }
        else
        {
            _interactUI.SetActive(false);
        }

        if (PlayerManager.Instance.InteractableItem == null && PlayerManager.Instance.InteractableStructure == null)
        {
            _interactUI.SetActive(false);
        }
    }
    private void SetTextUI(double value)
    {
        _air.text = "��� : " + PlayerManager.Instance.AirGauge.Value.ToString("F1");
    }

    private void SetInteractDelay(float value)
    {
        _interactDelay.fillAmount = value;
    }

    private void ToggleKeyUI()
    {
        if (PlayerManager.Instance.Player != null)
            if (PlayerManager.Instance.Player.CanJump)
            {
                _space.SetActive(true);
            }
            else
            {
                _space.SetActive(false);
            }

        if (PlayerManager.Instance.Player != null)
            if (!PlayerManager.Instance.Player.Controller.isGrounded && PlayerManager.Instance.CanUseJetpack)
            {
                _shift.SetActive(true);
            }
            else
            {
                _shift.SetActive(false);
            }
    }

    private void SetInteractText()
    {
        _isInInteract.Value = PlayerManager.Instance.IsInIntercation;

        if (PlayerManager.Instance.InteractableItem != null)
        {
            if (!_itemName.text.Equals(PlayerManager.Instance.InteractableItem.name))
            {
                _itemName.text = PlayerManager.Instance.InteractableItem.itemData.itemName;
            }
        }
        else if (PlayerManager.Instance.InteractableStructure != null)
        {
            if (!_itemName.text.Equals(PlayerManager.Instance.InteractableStructure.name))
            {
                _itemName.text = PlayerManager.Instance.InteractableStructure.StructureName;
            }
        }
        else if (PlayerManager.Instance.InteractableTestItem != null)
        {
            if (!_itemName.text.Equals(PlayerManager.Instance.InteractableTestItem.itemData.name))
            {
                _itemName.text = PlayerManager.Instance.InteractableTestItem.itemData.name;
            }
        }
    }

    private void SetHotbarHighlight()
    {
        if (_curhotbar == InputManager.Instance.CurHotbar)
        {
            return;
        }
        else
        {
            _curhotbar = InputManager.Instance.CurHotbar;
            switch (_curhotbar)
            {
                case 1:
                    _hotbar1.enabled = true;
                    _hotbar2.enabled = false;
                    _hotbar3.enabled = false;
                    break;
                case 2:
                    _hotbar1.enabled = false;
                    _hotbar2.enabled = true;
                    _hotbar3.enabled = false;
                    break;
                case 3:
                    _hotbar1.enabled = false;
                    _hotbar2.enabled = false;
                    _hotbar3.enabled = true;
                    break;
            }
        }
    }
}
