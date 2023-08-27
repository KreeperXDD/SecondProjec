using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class AmmoWidget : MonoBehaviour
{
    public TMPro.TMP_Text AmmoText;

    public void Refresh(int ammoCount)
    {
        AmmoText.text = ammoCount.ToString();
        
    }
}
