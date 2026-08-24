using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameStateLoader : MonoBehaviour
{
    public GameStateScene gameStateScene;
    public bool loadOnStart;
    public bool startOnStart;
    private string _saveName = "save";
    public void Start()
    {
        if (gameStateScene && loadOnStart)
        {
            if (gameStateScene.saveIndex > 0)
            {
                if (!gameStateScene.isNewGame)
                {
                    print("loading save " + gameStateScene.saveIndex);
                    bool managedToLoad = ApplySetting(LoadSave(gameStateScene.saveIndex));
                    print("Did the save load : " + managedToLoad);
                    gameStateScene.saveIndex = 0;
                }
                else
                {
                    GameSequencer.CurrentIndex = 0;
                    SaveIndex(gameStateScene.saveIndex);
                }

                if (startOnStart)
                {
                    GameSequencer.Instance.StartGame(Mathf.Max(GameSequencer.CurrentIndex - 1,0));
                }
            }
        }
    }

    public bool ApplySetting(GameSaveClass save)
    {
        if (GameState.Instance && GameSequencer.Instance && save != null)
        {
            GameState.PlayerName = save.playerName;
            GameSequencer.CurrentIndex = save.currentSequencerIndex;
            GameSequencer.LatestScore = save.latestGameplayScore;
            GameState.Day = save.currentDay;
            GameState.DariaReputation = save.dariaReputation;
            GameState.AngelinaReputation = save.angelinaReputation;
            GameState.KarinReputation = save.karinReputation;
            
            Dictionary<string,MainCharacters> charaEvent = new Dictionary<string, MainCharacters>();

            for (int i = 0; i < save.eventKeys.Count; i++)
            {
                charaEvent.Add(save.eventKeys[i], save.eventValue[i]);
            }
            
            GameState.CharacterEvent = charaEvent;
            return true;
        }
        return false;
    }
    
    public GameSaveClass LoadSave(int saveIndex)
    {
        if (SaveSystem.FileExist(_saveName + saveIndex))
        {
            return SaveSystem.Load<GameSaveClass>(_saveName + saveIndex);
        }
        return null;
    }

    public bool Save()
    {
        if(gameStateScene.saveIndex > 0) return SaveIndex(gameStateScene.saveIndex);
        return false;
    }
    
    private bool SaveIndex(int saveIndex)
    {
        if (GameState.Instance && GameSequencer.Instance)
        {

            List<string> keys = new List<string>();
            List<MainCharacters> values = new List<MainCharacters>();

            foreach (string key in GameState.CharacterEvent.Keys)
            {
                keys.Add(key);
            }
            foreach (MainCharacters value in GameState.CharacterEvent.Values)
            {
                values.Add(value);
            }
            
            GameSaveClass save = new GameSaveClass
            {
                playerName = GameState.PlayerName,
                currentSequencerIndex =  GameSequencer.CurrentIndex,
                latestGameplayScore = GameSequencer.LatestScore,
                currentDay = GameState.Day,
                dariaReputation =  GameState.DariaReputation,
                angelinaReputation =  GameState.AngelinaReputation,
                karinReputation =   GameState.KarinReputation,
                eventKeys =  keys,
                eventValue =   values,
            };
            
            SaveSystem.Save(save, _saveName + saveIndex);
            
            return true;
        }
        return false;
    }

    [Serializable]
    public class GameSaveClass
    {
        public string playerName;
        public int currentSequencerIndex;
        public int latestGameplayScore;
        public int currentDay;
        public float dariaReputation;
        public float angelinaReputation;
        public float karinReputation;
        public List<string> eventKeys;
        public List<MainCharacters> eventValue;
    }
    
    void OnEnable()
    {
        Singleton();
    }
    public static GameStateLoader Instance{ get; private set; }
    void Singleton()
    {
        if (Instance !=null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
}