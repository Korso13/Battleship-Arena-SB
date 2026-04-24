using UnityEngine;

public class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
{
    private static T _singleton;
    public static T Singleton => _singleton;

    public virtual void Awake()
    {
        if (_singleton == null)
        {
            //Debug.Log($"[SingletonBehaviour][{gameObject.name}] created");
            _singleton = (T)this;
            DontDestroyOnLoad( _singleton );
        }
        else if (_singleton != this)
        {
            //Debug.Log($"[SingletonBehaviour][{gameObject.name}] attempted to create double. Destroying");
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        //Debug.Log($"[SingletonBehaviour][{gameObject.name}] destroyed");
    }
}
