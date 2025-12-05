using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AirBar : MonoBehaviour
{
    [SerializeField] private Image airBar;

    public void UpdateAirBar(float currentAir, float maxAir)
    {
        float percentage = (float) currentAir / maxAir;
        airBar.fillAmount = percentage;
        if (percentage <= 0.1)
        {
            airBar.color = Color.red;
        }
        else if (percentage <= 0.25)
        {
            airBar.color = Color.yellow;
        }
        else
        {
            airBar.color = Color.white;
        }
    }

    public void ChangeColor(Color color)
    {
        airBar.color = color;
    }

}
