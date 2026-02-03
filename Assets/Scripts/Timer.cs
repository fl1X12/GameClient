using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] float moveTimer = GameConstants.Move_Duration;
    
    public float fillFraction;
    public bool outOfTime;
    float timerValue;

    void Start()
    {
        outOfTime=false;
        timerValue=moveTimer;
    }

    void Update()
    {
        UpdateTimer();
    }

    public void ResetTimer()
    {
        timerValue=moveTimer;
    }

    public void CancelTimer()
    {
        timerValue = 0;
    }

    void UpdateTimer()
    {
        timerValue -= Time.deltaTime;

        if (timerValue > 0)
        {
            fillFraction=timerValue/moveTimer;
        }
        else
        {
            outOfTime=true;
        }
    }

    
}
