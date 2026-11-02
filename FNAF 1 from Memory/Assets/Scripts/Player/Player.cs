using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;
    public float power;

    // foxy listens for in cam flips
    public bool inCams;

    // cam selected slows animatronics down or prevents freddy from doing his attack opportunity at 4b
    public Location camSelected;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
