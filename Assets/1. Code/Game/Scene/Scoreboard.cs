using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Scoreboard : MonoBehaviour
{

    public int[] points;
    public float transitionTime = 10f;

    public int progressBarsMaxLength = 100;
    public bool retractedBars = false;

    [HideInInspector]
    public int[] pointChangeDisplay;
    [HideInInspector]
    public float pointChangeDisplayTime = 0f;
    public TextMeshProUGUI[] pointChangeDisplayTexts;
    public float pointChangeHangtime = 1f;


    public TextMeshProUGUI[] pointDisplays;
    public Image[] progressBars;

    public void Init(){
        points = new int[Game.players.Length];
        pointChangeDisplay = new int[Game.players.Length];
    }

    public void OnPointsChange(int player, int amount){
        pointChangeDisplayTime = pointChangeHangtime;
        pointChangeDisplay[player] += amount;
    }

    void Update()
    {
        if (pointChangeDisplayTexts.Length > 0)
        {
            if (pointChangeDisplayTime > 0)
            {
                pointChangeDisplayTime -= Time.unscaledDeltaTime;
                for (int i = 0; i < pointChangeDisplayTexts.Length; i++)
                {
                    if (pointChangeDisplay[i] > 0)
                    {
                        pointChangeDisplayTexts[i].gameObject.SetActive(true);
                        pointChangeDisplayTexts[i].text = $"+<color=green>{pointChangeDisplay[i]}</color>";
                    }
                }
            }
            else
            {
                for (int i = 0; i < pointChangeDisplay.Length; i++)
                {
                    pointChangeDisplay[i] = 0;
                    if (pointChangeDisplayTexts[i].isActiveAndEnabled)
                        pointChangeDisplayTexts[i].gameObject.SetActive(false);
                }
            }
        }
        for (int i = 0; i < points.Length; i++)
        {
            if(points[i] - Game.players[i].points > 5)
                points[i] += Mathf.RoundToInt((points[i] - Game.players[i].points > 0) ? (transitionTime * Time.deltaTime) : (-transitionTime * Time.deltaTime));
            else
                points[i] = Game.players[i].points;
            pointDisplays[i].text = $"{Game.players[i].name}: {points[i]}";
        }

        if(progressBars != null && progressBars.Length > 0){
            int max = int.MinValue;

            for (int i = 0; i < points.Length; i++){
                if((int)points[i] > max)
                    max = (int)points[i];
            }

            for (int i = 0; i < points.Length; i++){
                float desired = retractedBars ? 0f :    ((float)points[i] / (float)max) * progressBarsMaxLength;
                progressBars[i].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Lerp(progressBars[i].rectTransform.sizeDelta.x, desired, Time.deltaTime * transitionTime));
                progressBars[i].color = Game.players[i].color;
            }
        }
    }
}
