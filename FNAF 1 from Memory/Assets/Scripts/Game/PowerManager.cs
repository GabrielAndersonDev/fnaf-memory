using System;
using UnityEngine;

public class PowerManager : MonoBehaviour
{
    public static PowerManager Singleton { get; private set; }

    private float PowerFloat;
    private int foxyHitMult;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    private float drainRate;

    [SerializeField]
    private float drainIncrease;
    private int inUseCount;

    private int PowerInt;
    public event Action<int> Power;
    public event Action PowerOutage;
    private bool isOutage;

    private void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Singleton = this;
        }
    }

    private void Start()
    {
        foxyHitMult = 1;
        PowerFloat = 100f;
        inUseCount = 1;
        isOutage = false;

        AssignInUseChange();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Singleton.isGameOver)
        {
            return;
        }

        if (PowerFloat > 0f)
        {
            if (isOutage)
            {
                return;
            }

            PowerFloat -= (drainRate + (drainIncrease * inUseCount)) * Time.deltaTime;
            PowerFloat = Mathf.Max(0f, PowerFloat);
        }
        else
        {
            isOutage = true;
            PowerOutage.Invoke();
        }

        if ((int)PowerFloat != PowerInt)
        {
            PowerInt = (int)PowerFloat;
            Power.Invoke(PowerInt);
        }
    }

    private void AssignInUseChange()
    {
        OfficeManager.Singleton.LDoorBtn += InUseChange;
        OfficeManager.Singleton.LLightBtn += InUseChange;
        OfficeManager.Singleton.RDoorBtn += InUseChange;
        OfficeManager.Singleton.RLightBtn += InUseChange;
        CamsManager.Singleton.OnCamFlip += InUseChange;
        GameManager.Singleton.KillPlayer += OnGameEnd;
        GameManager.Singleton.NightWin += OnGameEnd;
    }

    private void OnDisable()
    {
        OfficeManager.Singleton.BtnPressed -= InUseChange;
        CamsManager.Singleton.OnCamFlip -= InUseChange;
        GameManager.Singleton.KillPlayer -= OnGameEnd;
        GameManager.Singleton.NightWin -= OnGameEnd;
    }

    private void InUseChange(bool isLeft, bool isLight, bool isPressed)
    {
        if (isPressed)
        {
            inUseCount++;
        }
        else
        {
            if (inUseCount == 1)
            {
                Debug.LogError("PowerManager: inUseCount is already 1, trying to remove another source.");
                Debug.Break();
            }

            inUseCount--;
        }
    }

    public void FoxyAttack()
    {
        PowerFloat -= (7 * foxyHitMult);
        foxyHitMult++;
        PowerFloat = Mathf.Max(0f, PowerFloat);
    }

    private void OnGameEnd()
    {

        OfficeManager.Singleton.BtnPressed -= InUseChange;
        CamsManager.Singleton.OnCamFlip -= InUseChange;
    }
}
