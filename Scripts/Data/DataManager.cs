using SimpleInputNamespace;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static EnemyDefine;

public interface ICsvLoader<Key, Value>
{
    void LoadFromCSV(string text);
    Dictionary<Key, Value> MakeDict();
}

public class DataManager : SingletonMonobehaviour<DataManager>
{
    // Data table의 정보를 Dictionary에 저장
    public Dictionary<int, MonsterData> MonsterDict { get; private set; } = new Dictionary<int, MonsterData>();
    public Dictionary<EnemySkillType, MonsterSkillData> MonsterSkillDict { get; private set; } = new Dictionary<EnemySkillType, MonsterSkillData>();
    public Dictionary<string, TutorialData> TutorialDict { get; private set; } = new Dictionary<string, TutorialData>();
    public Dictionary<PlayerInputAction, KeyCode> RuntimeKeyDict { get; private set; } = new Dictionary<PlayerInputAction, KeyCode>();

    protected override void Awake()
    {
        base.Awake();
        Init();
    }
    void Init()
    {
        MonsterDict = LoadCSV<Data.MonsterLoader, int, MonsterData>("Data/table_test").MakeDict();
        MonsterSkillDict = LoadCSV<Data.MonsterSkillLoader, EnemySkillType, MonsterSkillData>("Data/MonsterSkillTable").MakeDict();
        Init_TutorialData();
        RuntimeKeyDict = LoadRuntimeKeyCSV();
    }

    // CSV 파일 로딩 후, 지정한 Loader 클래스에 파싱시킨 뒤, Loader 객체를 반환합니다.
    Loader LoadCSV<Loader, Key, Value>(string path) where Loader : ICsvLoader<Key, Value>, new()
    {
        TextAsset textAsset = Resources.Load<TextAsset>(path);
        Loader loader = new Loader();
        loader.LoadFromCSV(textAsset.text);

        return loader;
    }

    #region Tutorial
    public TutorialData GetTutorial(string tutorialName)
    {
        TutorialData tutorialData;
        if (TutorialDict.TryGetValue(tutorialName, out tutorialData))
            return tutorialData;
        else return null;
    }
    void Init_TutorialData()
    {
        Dictionary<string, TutorialDataBase> rawDict = LoadCSV<Data.TutorialDataLoader, string, TutorialDataBase>("Data/TutorialDataTable").MakeDict();
        TutorialDict = ParseRawTutorialData(rawDict);
    }
    Dictionary<string, TutorialData> ParseRawTutorialData(Dictionary<string, TutorialDataBase> sourceDict)
    {
        Dictionary<string, TutorialData> dict = new Dictionary<string, TutorialData>();

        dict = sourceDict.GroupBy(kvp => kvp.Key.Split('_')[0])
                                             .ToDictionary(tutorial => tutorial.Key,
                                                           tutorial => new TutorialData
                                                           {
                                                               tutorialTitle = tutorial.Key,
                                                               pages = tutorial.Select(x => x.Value).ToArray()
                                                           });
        return dict;
    }
    #endregion

    #region Key mapping

    Dictionary<PlayerInputAction, KeyCode> LoadRuntimeKeyCSV()
    {
        Dictionary<PlayerInputAction, KeyCode> dict = new Dictionary<PlayerInputAction, KeyCode>();

        try
        {
            string csvText;

#if UNITY_EDITOR
            TextAsset textAsset = Resources.Load<TextAsset>("Data/KeyMapping");  // Resources/KeyMapping.csv
            if (textAsset == null)
            {
                return dict;
            }
            csvText = textAsset.text;
#else
        string path = Path.Combine(Application.streamingAssetsPath, "KeyMapping.csv");
        if (!File.Exists(path))
        {
            Debug.LogWarning($"StreamingAssets/KeyMapping.csv 없음");
            return dict;
        }
        csvText = File.ReadAllText(path);
        Debug.Log("[KeyCSV] File 로드 성공");
#endif

            string[] lines = csvText.Split('\n');

            foreach (string line in lines.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                string[] parts = line.Split(',');
                if (parts.Length < 2) continue;

                string actionStr = parts[0].Trim();
                string keyStr = parts[1].Trim();

                if (Enum.TryParse<PlayerInputAction>(actionStr, out PlayerInputAction action))
                {
                    if (Enum.TryParse<KeyCode>(keyStr, out KeyCode keyCode))
                    {
                        dict[action] = keyCode;
                        //Debug.Log($"{action} = {keyCode}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"KeyCSV 에러: {ex.Message}");
        }

        return dict;
    }

    string GetCSVPath()
    {
#if UNITY_EDITOR
        string editorPath = "Assets/StreamingAssets/KeyMapping.csv";
        if (System.IO.File.Exists(editorPath))
            return editorPath;
#endif
        return System.IO.Path.Combine(Application.streamingAssetsPath, "KeyMapping.csv");
    }
    #endregion
}
