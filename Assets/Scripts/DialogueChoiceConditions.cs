using System;

[Flags]
public enum DialogueChoiceConditions
{
    None = 0,
    InvertCondition = 1 << 0,
    Reputation = 1 << 1,
        
    
}