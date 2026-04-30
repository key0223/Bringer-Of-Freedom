using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class BuffUI : MonoBehaviour
{
    [SerializeField] Transform contents;
    [SerializeField] GameObject buffSlotPrefab;

    List<GameObject> activeDebuffUI = new List<GameObject>();

    public void UpdateUI(List<Debuff> debuffs)
    {
        foreach(GameObject obj in activeDebuffUI)
        {
            Destroy(obj);
        }

        activeDebuffUI.Clear();

        foreach(Debuff debuff in debuffs)
        {
            GameObject slot = Instantiate(buffSlotPrefab,contents);
            BuffSlot buffSlot = slot.GetComponent<BuffSlot>();

            buffSlot.SetSlot(GetIcon(debuff.DebuffType));

            activeDebuffUI.Add(slot);
        }
    }


    Sprite GetIcon(DebuffType debuffType )
    {
        switch(debuffType)
        {
            case DebuffType.DEBUFF_BURN:
                return Resources.Load<Sprite>("Sprites/DEBUFF_BURN");
            case DebuffType.DEBUFF_STUN:
                return Resources.Load<Sprite>("Sprites/DEBUFF_STUN");
            case DebuffType.DEBUFF_FREEZE:
                return Resources.Load<Sprite>("Sprites/DEBUFF_FREEZE");
            case DebuffType.DEBUFF_POISON:
                return Resources.Load<Sprite>("Sprites/DEBUFF_POISON");
            case DebuffType.DEBUFF_PARALYSIS:
                return Resources.Load<Sprite>("Sprites/DEBUFF_PARALYSIS");
            default:
                return null;
        }
    }
}
