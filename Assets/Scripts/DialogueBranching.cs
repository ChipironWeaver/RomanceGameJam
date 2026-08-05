using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

[Serializable]
public class DialogueBranching
{
    [SerializeField] private List<DialogueEventCondition> _eventConditions;
    [SerializeField] private List<DialogueReputationCondition> _reputationConditions;
    
    [SerializeField] private DialogueConditionType _conditionType;
    [SerializeField] private float _conditionNumber;
    [SerializeField,MinMaxRangeSlider(0,100)] private float _conditionPercentage;

    public bool GetBool()
    {
        int totalResults = 0;
        int totalTrue= 0;
        foreach (DialogueEventCondition condition in _eventConditions)
        {
            if (condition.GetBool())
            {
                totalTrue++;
            }
            totalResults++;
        }
        foreach (DialogueReputationCondition condition in _reputationConditions)
        {
            if (condition.GetBool())
            {
                totalTrue++;
            }
            totalResults++;
        }

        switch (_conditionType)
        {
            case DialogueConditionType.AtLeastOrEqualNumber:
                return totalTrue >= _conditionNumber;
            case DialogueConditionType.AtMostOrEqualNumber:
                return totalTrue <= _conditionNumber;
            case DialogueConditionType.AtLeastOrEqualPercent:
                return totalTrue / totalResults * 100 >= _conditionNumber;
            case DialogueConditionType.AtMostOrEqualPercent:
                return totalTrue / totalResults * 100 <= _conditionNumber;
            case DialogueConditionType.OnlyOne:
                return totalTrue == 1;
            case DialogueConditionType.All:
                return totalTrue == totalResults;
        }
        return false;
    }
    
    private enum DialogueConditionType
    {
        AtLeastOrEqualNumber,
        AtMostOrEqualNumber,
        AtLeastOrEqualPercent,
        AtMostOrEqualPercent,
        OnlyOne,
        All,
    }
    
    [Serializable]
    private class DialogueEventCondition
    {
        public string eventName;
        public DatableCharacters eventCharacter;
        public bool invertCondition;
        
        public bool GetBool()
        {
            GameState.CharacterEvent.TryGetValue(eventName, out DatableCharacters character);
            return character ==  eventCharacter ^ invertCondition;
        }
    }

    [Serializable]
    private class DialogueReputationCondition
    {
        public ReputationConditionType reputationType;
        public DatableCharacters reputationCharacter;
        public float conditionNumber;

        public enum ReputationConditionType
        {
            MoreThan,
            LessThan,
        }
        
        public bool GetBool()
        {
            switch (reputationType)
            {
                case ReputationConditionType.MoreThan:
                    return conditionNumber <= GameState.GetCharacterReputation(reputationCharacter);
                case ReputationConditionType.LessThan:
                    return conditionNumber >= GameState.GetCharacterReputation(reputationCharacter);
            }
            return false;
        }
    }
}