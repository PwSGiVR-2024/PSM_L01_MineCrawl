using UnityEngine;

public interface ISkill
{
    string SkillName { get; }
    int ManaCost { get; }
    void Activate(ICharacter user, ICharacter target);
}
