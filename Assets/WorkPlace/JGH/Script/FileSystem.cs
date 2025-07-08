using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DesignPattern;


// ========== 데이터 클래스들 ==========
[System.Serializable]
public class GameData
{
    public int currentDay;
    public double oxygenRemaining;
    public double electricalEnergy;
    public double shelterDurability;
    public bool isToDay;
}


public class FileSystem : Singleton<FileSystem>
{
    private string settingPath;
    private string gameDataPath;
    private string eventFilePath;
    public string eventTempFilePath;
    
    public Dictionary<int, EventController> eventDict = new();


    private bool shouldLoadItemsOnStart = false;

    private bool isInitialized = false;

    protected override void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("FileSystem 인스턴스가 이미 존재합니다. 중복 제거.");
            Destroy(gameObject);
            return;
        }

        base.Awake(); // 싱글톤 인스턴스 초기화


        // 플랫폼별 최적 경로 설정
        string dataDirectory = GetPlatformDataDirectory();

        // 데이터 디렉토리 생성
        if (EnsureDirectoryExists(dataDirectory))
        {
            // 파일 경로 설정
            settingPath = Path.Combine(dataDirectory, "setting.json");
            gameDataPath = Path.Combine(dataDirectory, "gamedata.json");
            eventFilePath = Path.Combine(dataDirectory, "event.json");
            eventTempFilePath = Path.Combine(dataDirectory, "eventTemp.json");

            // 핵심 수정 3: 초기화 성공 표시
            isInitialized = true;
        }
        else
        {
            Debug.LogError("FileSystem 초기화 실패!");
            isInitialized = false;
        }

        DontDestroyOnLoad(gameObject);
    }


    // ========== 플랫폼별 경로 설정 ==========
    private string GetPlatformDataDirectory()
    {
        // 플랫폼과 관계없이 일관된 경로 사용
        string baseDirectory = Application.persistentDataPath;

        return baseDirectory;
    }

    private bool EnsureDirectoryExists(string directoryPath)
    {
        try
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                Debug.Log($"데이터 디렉토리 생성: {directoryPath}");
            }

            // 쓰기 권한 테스트
            return TestDirectoryPermissions(directoryPath);
        }
        catch (Exception e)
        {
            Debug.LogError($"디렉토리 생성/확인 실패: {e.Message}");
            Debug.LogError($"경로: {directoryPath}");

            // 플랫폼별 권한 안내
            return false;
        }
    }

    private bool TestDirectoryPermissions(string directoryPath)
    {
        try
        {
            string testFile = Path.Combine(directoryPath, "permission_test.tmp");
            File.WriteAllText(testFile, "permission test");

            if (File.Exists(testFile))
            {
                File.Delete(testFile);
                Debug.Log($"디렉토리 쓰기 권한 확인 완료: {directoryPath}");
                return true;
            }

            return false;
        }
        catch (Exception e)
        {
            Debug.LogError($"디렉토리 쓰기 권한 없음: {e.Message}");
            return false;
        }
    }

    // ========== 설정 파일 관리 ==========
    public void SaveSetting(SettingData data)
    {
        if (!isInitialized)
        {
            Debug.LogError("FileSystem이 초기화되지 않았습니다!");
            return;
        }

        try
        {
            string json = JsonUtility.ToJson(data, true);

            string tempPath = settingPath + ".tmp";
            File.WriteAllText(tempPath, json);

            if (File.Exists(settingPath))
            {
                File.Delete(settingPath);
            }

            File.Move(tempPath, settingPath);

            Debug.Log($"설정 저장 완료: {settingPath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"설정 저장 실패: {e.Message}");
        }
    }

    public SettingData LoadSetting()
    {
        if (!isInitialized)
        {
            Debug.LogError("FileSystem이 초기화되지 않았습니다!");
            return GetDefaultSetting();
        }

        try
        {
            if (File.Exists(settingPath))
            {
                string json = File.ReadAllText(settingPath);

                if (string.IsNullOrEmpty(json))
                {
                    Debug.LogWarning("설정 파일이 비어있음, 기본값 반환");
                    return GetDefaultSetting();
                }

                SettingData data = JsonUtility.FromJson<SettingData>(json);
                Debug.Log($"설정 불러오기 완료: {settingPath}");
                return data ?? GetDefaultSetting();
            }
            else
            {
                Debug.Log("설정 파일 없음, 기본값 반환");
                return GetDefaultSetting();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"설정 불러오기 실패: {e.Message}");
            return GetDefaultSetting();
        }
    }

    private SettingData GetDefaultSetting()
    {
        return new SettingData
        {
            fullscreen = false,
            quality = 1,
            bgmVolume = 1f,
            sfxVolume = 1f
        };
    }

    // ========== 게임 데이터 관리 ==========
    public void SaveGameData(GameData data)
    {
        if (!isInitialized)
        {
            Debug.LogError("FileSystem이 초기화되지 않았습니다!");
            return;
        }

        try
        {
            string json = JsonUtility.ToJson(data, true);

            string tempPath = gameDataPath + ".tmp";
            File.WriteAllText(tempPath, json);

            if (File.Exists(gameDataPath))
            {
                File.Delete(gameDataPath);
            }

            File.Move(tempPath, gameDataPath);

            Debug.Log($"게임 데이터 저장 완료: {gameDataPath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"게임 데이터 저장 실패: {e.Message}");
        }
    }

    public GameData LoadGameData()
    {
        if (!isInitialized)
        {
            Debug.LogError("FileSystem이 초기화되지 않았습니다!");
            return GetDefaultGameData();
        }

        try
        {
            if (File.Exists(gameDataPath))
            {
                string json = File.ReadAllText(gameDataPath);

                if (string.IsNullOrEmpty(json))
                {
                    Debug.LogWarning("게임 데이터 파일이 비어있음");
                    return null;
                }

                GameData data = JsonUtility.FromJson<GameData>(json);
                Debug.Log($"게임 데이터 불러오기 완료: {gameDataPath}");
                return data ?? GetDefaultGameData();
            }
            else
            {
                Debug.Log("게임 데이터 파일 없음");
                return null;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"게임 데이터 불러오기 실패: {e.Message}");
            return null;
        }
    }

    public GameData GetDefaultGameData()
    {
        return new GameData
        {
            currentDay = 1,
            oxygenRemaining = 60f,
            electricalEnergy = 100f,
            shelterDurability = 200f,
            isToDay = false
        };
    }

    public void ApplyGameData(GameData data)
    {
        Debug.Log("게임 데이터 적용 시작");

        StartCoroutine(WaitForStatusSystemAndApply(data));
    }

    private System.Collections.IEnumerator WaitForStatusSystemAndApply(GameData data)
    {
        float waitTime = 0f;
        float maxWaitTime = 15f;

        while (StatusSystem.Instance == null && waitTime < maxWaitTime)
        {
            Debug.Log($"StatusSystem 인스턴스 대기 중... ({waitTime:F1}초)");
            yield return new UnityEngine.WaitForSeconds(0.5f);
            waitTime += 0.5f;
        }

        if (StatusSystem.Instance == null)
        {
            Debug.LogError("StatusSystem 인스턴스를 찾을 수 없습니다!");
            yield break;
        }

        try
        {
            StatusSystem.Instance.SetCurrentDay(data.currentDay);
            StatusSystem.Instance.SetOxygen(data.oxygenRemaining);
            StatusSystem.Instance.SetEnergy(data.electricalEnergy);
            StatusSystem.Instance.SetDurability(data.shelterDurability);
            StatusSystem.Instance.SetIsToDay(data.isToDay);

            Debug.Log("StatusSystem 데이터 적용 완료");
        }
        catch (Exception e)
        {
            Debug.LogError($"StatusSystem 데이터 적용 실패: {e.Message}");
        }

        if (MenuSystem.Instance != null && MenuSystem.Instance.MainMenu != null)
        {
            MenuSystem.Instance.MainMenu.SetActive(false);
        }
    }
    
    // ========== 이벤트 파일 관리 ==========
    public void SaveEventData(string value)
    {
        if (EventManager.Instance != null)
        {
            File.WriteAllText(eventFilePath, value);
        }
        else
        {
            Debug.LogError("EventManager 인스턴스가 존재하지 않습니다!");
        }
    } 
    
    public Dictionary<int, int> LoadEventData()
    {
        Dictionary<int, int> loadedData = new Dictionary<int, int>();
        if (File.Exists(eventFilePath))
        {
            string json = File.ReadAllText(eventFilePath);

            json = json.Trim('{', '}');
            loadedData = json.Split(',')
                .Select(pair => pair.Split(':'))
                .ToDictionary(
                    parts => int.Parse(parts[0]),
                    parts => int.Parse(parts[1])
                );
            
        }
        return loadedData;
    }
    
      public void SaveTempEventData(string value)
        {
            if (EventManager.Instance != null)
            {
                File.WriteAllText(eventTempFilePath, value);
            }
            else
            {
                Debug.LogError("EventManager 인스턴스가 존재하지 않습니다!");
            }
        } 
        
        public Dictionary<int, int> LoadTempEventData()
        {
            Dictionary<int, int> loadedData = new Dictionary<int, int>();
            if (File.Exists(eventTempFilePath))
            {
                string json = File.ReadAllText(eventTempFilePath);
    
                json = json.Trim('{', '}');
                loadedData = json.Split(',')
                    .Select(pair => pair.Split(':'))
                    .ToDictionary(
                        parts => int.Parse(parts[0]),
                        parts => int.Parse(parts[1])
                    );
                
            }
            return loadedData;
        }
    

    // ========== 파일 존재 확인 메서드들 ==========
    public bool HasSaveData()
    {
        return isInitialized && File.Exists(gameDataPath);
    }

    public void DeleteGameSaveData()
    {
        if (!isInitialized)
        {
            Debug.LogError("FileSystem이 초기화되지 않았습니다!");
            return;
        }

        try
        {
            
            if (File.Exists(gameDataPath))
            {
                File.Delete(gameDataPath);
                Debug.Log("게임 데이터 삭제 완료");
            }
            
            if (File.Exists(eventFilePath))
            {
                File.Delete(eventFilePath);
                Debug.Log("이벤트 데이터 삭제 완료");
            }        
            
            if (File.Exists(eventTempFilePath))
            {
                File.Delete(eventTempFilePath);
                Debug.Log("이벤트 임시 데이터 삭제 완료");
            }        
            
        }
        catch (Exception e)
        {
            Debug.LogError($"저장 데이터 삭제 실패: {e.Message}");
        }
    }

}