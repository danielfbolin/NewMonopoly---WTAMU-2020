using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;
/*#if UNITY_EDITOR
using UnityEditor;
#endif*/

[CreateAssetMenu(menuName = "Singletons/MasterManager")]
public class MasterManager : SingletonScriptableObject<MasterManager>
{
    [SerializeField]
    private GameSettings _gameSettings;
    
    public static GameSettings GameSettings { get {return Instance._gameSettings; } }
    [SerializeField]
    private List<NetworkedPrefab> _networkedPrefab = new List<NetworkedPrefab>();

    public static GameObject  NetworkInstantiate(GameObject obj, Vector3 position, Quaternion rotation) {
        foreach (NetworkedPrefab networkedPrefab in Instance._networkedPrefab)
        {
            if(networkedPrefab.Prefab == obj)
            {
                if(networkedPrefab.Path != string.Empty)
                {
                    Debug.Log(networkedPrefab.Path);
                    GameObject result= PhotonNetwork.Instantiate(networkedPrefab.Path, position, rotation);
                    return result;
                }
                else{
                    Debug.LogError("Path is empty for gameobject name" + networkedPrefab.Prefab);
                    return null;
                }
            }
        }
        return null;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void PopulateNetworkedPrefabs()
    {
/*#if UNITY_EDITOR        
        Instance._networkedPrefab.Clear();
        GameObject[] results = Resources.LoadAll<GameObject>("");
        for( int i=0; i<results.Length; i++)
        {
            if(results[i].GetComponent<PhotonView>() != null)
            {
                string path = AssetDatabase.GetAssetPath(results[i]);
                Instance._networkedPrefab.Add(new NetworkedPrefab(results[i],path));
            }

        }
       
#endif*/
        var loadedObjects = Resources.LoadAll("GameObjects", typeof(GameObject)).Cast<GameObject>();
    }
}
