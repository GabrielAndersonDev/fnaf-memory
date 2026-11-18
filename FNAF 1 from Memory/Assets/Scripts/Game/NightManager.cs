using System;
using System.Collections;
using UnityEngine;

public class NightManager : MonoBehaviour
{
    public static NightManager Singleton { get; private set; }
    public int Hour { get;  private set; }
    private static WaitForSeconds waitForSeconds80 = new(80f);

    public event Action<int> HourChange;

    public bool isNightOver;

    private Coroutine HourRoutine;

    private void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Singleton = this;
        }
        Hour = 12;

        isNightOver = false;

        HourRoutine ??= StartCoroutine(HourCoroutine());
    }

    IEnumerator HourCoroutine()
    {
        while (!isNightOver)
        {
            if (Hour == 6)
            {
                WinNight();
            }

            yield return waitForSeconds80;
            Hour = Hour == 12 ? 1 : Hour++;
            HourChange.Invoke(Hour);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void WinNight()
    {
        isNightOver = true;
        Debug.Log("You won! Not implemented yet.");
        GameManager.Singleton.OnNightWin();
    }
}
