using System;
using System.Collections.Generic;
using UnityEngine;
using static EnemyDefine;

namespace Data
{
    [Serializable]
    public class MonsterLoader : ICsvLoader<int, MonsterData>
    {
        List<MonsterData> array = new List<MonsterData>();

        // csv 파싱  -> List 저장
        public void LoadFromCSV(string csvText)
        {
            var lines = csvText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            int headerRow = -1;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("Mon_index") && lines[i].Contains("Mon_name"))
                {
                    headerRow = i;
                    break;
                }
            }

            if (headerRow == -1)
            {
                Debug.LogError("헤더를 찾지 못했습니다.");
                return;
            }

            for (int i = headerRow + 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] tokens = line.Split(',');

                if (tokens.Length < 4) continue;

                try
                {
                    MonsterData monster = new MonsterData
                    {
                        monsterId = int.Parse(tokens[0]),
                        monsterName = tokens[1],
                        monsterGrade = (EnemyGrade)int.Parse(tokens[2]),
                        monsterAttackType = (EnemyAttackType)int.Parse(tokens[3])
                    };

                    array.Add(monster);
                }
                catch
                {
                    Debug.LogWarning($"[{i + 1}줄] 파싱 실패: {line}");
                }
            }
        }

        public Dictionary<int, MonsterData> MakeDict()
        {
            Dictionary<int, MonsterData> dict = new Dictionary<int, MonsterData>();

            foreach(MonsterData monster in array)
            {
                dict.Add(monster.monsterId, monster);
            }
            return dict;
        }
    }

    [Serializable]
    public class MonsterSkillLoader : ICsvLoader<EnemySkillType, MonsterSkillData>
    {
        List<MonsterSkillData> array = new List<MonsterSkillData>();

        // csv 파싱  -> List 저장
        public void LoadFromCSV(string csvText)
        {
            var lines = csvText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            int headerRow = -1;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("monsterSkillId") && lines[i].Contains("skillType"))
                {
                    headerRow = i;
                    break;
                }
            }

            if (headerRow == -1)
            {
                Debug.LogError("헤더를 찾지 못했습니다.");
                return;
            }

            for (int i = headerRow + 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] tokens = line.Split(',');

                if (tokens.Length < 4) continue;

                try
                {
                    MonsterSkillData skill = new MonsterSkillData
                    {
                        monsterSkillId = int.Parse(tokens[0]),
                        skillType = (EnemySkillType)Enum.Parse(typeof(EnemySkillType), tokens[1]),
                        skillOwner = (DragonType)Enum.Parse(typeof(DragonType), tokens[2]),
                        checkPivot = (EnemyCheckPivot)Enum.Parse(typeof(EnemyCheckPivot), tokens[3]),
                        isParallel = tokens[4] == "1" ? true : false,
                        skillPriority= int.Parse(tokens[5]),
                        checkRadius = float.Parse(tokens[6]),
                        cooldownTime = float.Parse(tokens[7]),
                    };

                    array.Add(skill);
                }
                catch
                {
                    Debug.LogWarning($"[{i + 1}줄] 파싱 실패: {line}");
                }
            }
        }

        public Dictionary<EnemySkillType, MonsterSkillData> MakeDict()
        {
            Dictionary<EnemySkillType, MonsterSkillData> dict = new Dictionary<EnemySkillType, MonsterSkillData>();

            foreach (MonsterSkillData skill in array)
            {
                dict.Add(skill.skillType, skill);
            }
            return dict;
        }
    }

    [Serializable]
    public class TutorialDataLoader : ICsvLoader<string,TutorialDataBase>
    {
        List<TutorialDataBase> array = new List<TutorialDataBase>();

        // csv 파싱  -> List 저장
        public void LoadFromCSV(string csvText)
        {
            var lines = csvText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            int headerRow = -1;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("tutorialId") && lines[i].Contains("tutorialTitle"))
                {
                    headerRow = i;
                    break;
                }
            }

            if (headerRow == -1)
            {
                Debug.LogError("헤더를 찾지 못했습니다.");
                return;
            }

            for (int i = headerRow + 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] tokens = line.Split(',');

                if (tokens.Length < 4) continue;

                try
                {
                    TutorialDataBase data = new TutorialDataBase
                    {
                        tutorialId = tokens[0],
                        tutorialTitle = tokens[1],
                        tutorialPageTitle = tokens[2],
                        description = tokens[3],
                        resourceDirectory = tokens[4],
                        resourceName = tokens[5],
                        tutorialImage = Resources.Load<Sprite>($"{tokens[4]}/{tokens[5]}")
                    };

                    array.Add(data);
                }
                catch
                {
                    Debug.LogWarning($"[{i + 1}줄] 파싱 실패: {line}");
                }
            }
        }

        public Dictionary<string, TutorialDataBase> MakeDict()
        {
            Dictionary<string, TutorialDataBase> dict = new Dictionary<string, TutorialDataBase>();

            foreach (TutorialDataBase data in array)
            {
                dict.Add(data.tutorialId, data);
            }
            return dict;
        }
    }
}