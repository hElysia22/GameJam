using UnityEngine;

public class WindManager : MonoBehaviour
{
    public static WindManager Instance {  get; private set; }

    [Header("全局风参数")]
    public Vector2 GlobalWindDir;
    public float GlobalWindPower;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public Vector2 GetWindForcAtPosition(Vector2 worldPos)
    {
        return GlobalWindDir * GlobalWindPower;
    }

    public void setGlobalWind(Vector2 dir, float power)
    {
        GlobalWindDir = dir;
        GlobalWindPower = power;
    }
}
