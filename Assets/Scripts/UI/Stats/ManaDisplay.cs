using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManaDisplay : MonoBehaviour
{
    [SerializeField] Mana mana;
    [SerializeField] Image manaFill;
    [SerializeField] TextMeshProUGUI manaText = null;

    public void SetMana(Mana mana)
    {
        if(this.mana != null)
        {
            this.mana.OnManaModified -= Mana_OnManaModified;
        }

        if (mana != null) 
        {
            this.mana = mana;
            this.mana.OnManaModified += Mana_OnManaModified;
            UpdateMana();
        }
        else
        {
            manaText.text = "0/0";
            manaFill.fillAmount = 0;
        }
    }

    private void Mana_OnManaModified()
    {
        UpdateMana();
    }

    private void UpdateMana()
    {
        // gameObject is disabled before this function is called
        if (manaFill == null)
        {
            Debug.Log("Mana Fill is Null");
            return;
        }

        manaFill.fillAmount = mana.GetManaPercent();
        manaText.text = $"{(int) mana.GetManaPoint() } / {(int) mana.GetMaxMana()}";
    }
}
