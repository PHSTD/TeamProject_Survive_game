using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public GameObject Ranpy;
    public GameObject Choi;
    public GameObject Park;
    public GameObject Rachel;
    public GameObject Gang;

    public TMP_Text ScriptText;
    public TMP_Text NameText;

    public UnityEngine.UI.Image BackgroundImage;

    private int currentLine = 0;

    private List<DialogueLine> dialogues = new List<DialogueLine>
    {
        new DialogueLine { speaker = "������ �ڻ�", text = "����, �̰� ��ȯ�� ����? �̰� �� ���⿡��", appear = new List<string> { "Gang" }, bgm = "None", sfx = "" },
        new DialogueLine { speaker = "���𼼿콺", text = "�ȳ��ϼ���. ����� ��ȯ�� �����𼼿콺�� �Դϴ�.", appear = new List<string> { "Gang" }, bgm = "None", sfx = "" },
        new DialogueLine { speaker = "������ �ڻ�", text = "���� ��¦�̾�!!!...�޿�, �� ��ȯ�� ����� �� �־�?", appear = new List<string> { "Gang" }, bgm = "None", sfx = "" },
        new DialogueLine { speaker = "���𼼿콺", text = "���� �����𼼿콺���� ���� �������� ���� ������ �������Դϴ�. �ش� ������ ��ü�� �ʿ��մϴ�.", appear = new List<string> { "Gang" }, bgm = "None", sfx = "" },
        new DialogueLine { speaker = "������ �ڻ�", text = "����, �� ���� ������ �� �ִ� ����� �˷��ٷ�?", appear = new List<string> { "Gang" }, bgm = "None", sfx = "" },
        new DialogueLine { speaker = "���𼼿콺", text = "���� ����� Ȯ���մϴ�. ���ڻ� �����ء����� �����͸� �����մϴ١����� �Ϸ��߽��ϴ�.", appear = new List<string> { "Gang" }, bgm = "None", sfx = "" },
        new DialogueLine { speaker = "������ �ڻ�", text = "�׷�, ���� �� Ż���� �� �־�!", appear = new List<string> { "Gang" }, bgm = "None", sfx = "" },
        // ��� �߰� ����
    };

    void Start()
    {
        BackgroundImage.sprite = null;
        BackgroundImage.color = new Color(0f, 0f, 0f, 0.6f);

        Ranpy.SetActive(true);
        ShowNextLine();
    }

    // ���� ���� �Ѿ�� ��ư�� �Լ�
    public void OnClickNext()
    {
        ShowNextLine();
    }

    // ��ü ��縦 ��ŵ�ϴ� ��ư�� �Լ�
    public void OnClickSkip()
    {
        // ��� ĳ���� �����
        SetCharacterVisibility(new List<string>());
        SceneSystem.Instance.LoadSceneWithDelay(SceneSystem.Instance.GetShelterSceneName());
    }

    void ShowNextLine()
    {
        if (currentLine >= dialogues.Count)
        {
            ScriptText.text = "";
            NameText.text = "";


            SceneSystem.Instance.LoadSceneWithDelay(SceneSystem.Instance.GetShelterSceneName());
            return;
        }

        var line = dialogues[currentLine];
        ScriptText.text = line.text;
        NameText.text = line.speaker;

        SetCharacterVisibility(line.appear);

        // BGM ó��
        if (!string.IsNullOrEmpty(line.bgm))
        {
            if (line.bgm == "None")
            {
                AudioSystem.Instance.StopBGM();
            }
            else
            {
                AudioSystem.Instance.PlayBGMByName(line.bgm);
            }
        }

        // SFX ó��
        if (!string.IsNullOrEmpty(line.sfx))
        {
            AudioSystem.Instance.PlaySFXByName(line.sfx);
        }

        currentLine++;
    }


    void SetCharacterVisibility(List<string> activeList)
    {
        Choi.SetActive(false);
        Park.SetActive(false);
        Rachel.SetActive(false);
        Gang.SetActive(false);

        foreach (string name in activeList)
        {
            switch (name)
            {
                case "Choi": Choi.SetActive(true); break;
                case "Park": Park.SetActive(true); break;
                case "Rachel": Rachel.SetActive(true); break;
                case "Gang": Gang.SetActive(true); break;
            }
        }

    }
}
