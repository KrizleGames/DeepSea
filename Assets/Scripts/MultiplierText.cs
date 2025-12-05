using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MultiplierText : MonoBehaviour
{
    private TMP_Text multiplier;

    private void Awake()
    {
        multiplier = GetComponent<TMP_Text>();
    }

    public void UpdateText(int pointMultiplier)
    {
        if (pointMultiplier == 1)
        {
            multiplier.SetText("");
        }
        else
        {
            multiplier.SetText("X" + pointMultiplier);
        }
    }

}
