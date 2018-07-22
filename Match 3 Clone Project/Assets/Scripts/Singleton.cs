using UnityEngine;

/// <summary>
/// Singleton template.
/// </summary>
public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if(applicationIsQuitting)
            {
                return null;
            }

            if(instance == null)
            {
                instance = FindObjectOfType<T>();
                if(instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name;
                    instance = obj.AddComponent<T>();
                }
            }
            return instance;
        }
    }

    private static bool applicationIsQuitting = false;


    public virtual void Awake()
    {
        if(instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public virtual void OnDestroy()
    {
        applicationIsQuitting = true;
    }

    public void InitManually()
    {
        T tempInstance = Instance;
    }
}