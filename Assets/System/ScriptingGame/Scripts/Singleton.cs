using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance = null;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T),true);

                if (instance == null)
                {
                    string goName = typeof(T).ToString();
                    GameObject go = GameObject.Find(goName);

                    if (go == null)
                    {
                        go = new GameObject();
                        go.name = goName;
                    }

                    DontDestroyOnLoad(go);
                    instance = go.AddComponent<T>();
                }
            }

            return instance;
        }
    }
}