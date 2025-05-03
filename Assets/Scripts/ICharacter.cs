using UnityEngine;

public interface ICharacter
{
    string Name { get; }
    int Level { get; }
    CharacterStats Stats { get; }
    CharacterRaceSO Race { get; }
    CharacterClassSO Class { get; }

    void TakeDamage(int amount);
    void Heal(int amount);
    void UseSkill(ISkill skill, ICharacter target);
    void GainExperience(int amount);
}
