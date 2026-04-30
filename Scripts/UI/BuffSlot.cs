using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffSlot : MonoBehaviour
{
    [SerializeField] Image iconImage;

    public void SetSlot(Sprite icon)
    {
        iconImage.sprite = icon;
    }
}
