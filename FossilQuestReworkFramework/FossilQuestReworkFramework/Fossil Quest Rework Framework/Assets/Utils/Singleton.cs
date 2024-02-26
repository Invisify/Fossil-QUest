using UnityEngine;

/// <summary>
/// Be aware this will not prevent a non singleton constructor
///   such as `T myT = new T();`
/// To prevent that, add `protected T () {}` to your singleton class.
/// 
/// As a note, this is made as MonoBehaviour because we need Coroutines.
/// </summary>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                var type = typeof(T);
                var objects = FindObjectsOfType<T>();

                if (objects.Length > 0)
                {
                    _instance = objects[0];
                    if (objects.Length > 1)
                    {
                        Debug.LogWarning("There is more than one instance of Singleton of type \"" + type + "\". Keeping the first. Destroying the others.");
                        for (var i = 1; i < objects.Length; i++)
                            Destroy(objects[i].gameObject);
                    }
                    return _instance;
                }

                var gameObject = new GameObject();
                gameObject.name = type.ToString();

                _instance = gameObject.AddComponent<T>();
            }
            return _instance;
        }
    }
}
