
using UnityEngine;

public enum PoolObjectType
{
    Exchild
}

public class Factory : Singleton<Factory>
{
    /// <summary>
    /// 노이즈 반지름
    /// </summary>
    public float noisePower = 0.5f;

    ExChildPool exChildPool;


    protected override void OnInitialize()
    {
        base.OnInitialize();

        exChildPool = GetComponentInChildren<ExChildPool>(true);
        if (exChildPool != null)
            exChildPool.Initialize();

    }

    public GameObject GetObject(PoolObjectType type, Vector3? position = null, Vector3? euler = null)
    {
        GameObject result = null;
        switch (type)
        {
            case PoolObjectType.Exchild:
                result = exChildPool.GetObject(position, euler).gameObject;
                break;
            default:
                break;
        }
        return result;
    }
}

