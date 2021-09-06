using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaDisplay : MonoBehaviour
{
    [SerializeField] Mana mana;
    [SerializeField] Image manaFill;

    public void SetMana(Mana mana)
    {
        this.mana = mana;
    }

    private void Update()
    {
        if (mana != null)
        {
            manaFill.fillAmount = mana.GetManaPercent();
        }
    }
}
