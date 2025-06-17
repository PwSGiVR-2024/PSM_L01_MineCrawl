using UnityEngine;
using UnityEngine.EventSystems;

public class SkillTooltipHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public SkillTooltip skillTooltip;
    public SkillsSO skill;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (skillTooltip != null && skill != null)
            skillTooltip.Show($"<b>{skill.SkillName}</b>\n{skill.Description}");
        else
            Debug.LogWarning("SkillTooltip or Skill is not assigned in SkillTooltipHandler!");
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        if (skillTooltip != null)
            skillTooltip.Hide();
        else
            Debug.LogWarning("SkillTooltip is not assigned in SkillTooltipHandler!");
    }

}
