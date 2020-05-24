using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class ChangeColor : MonoBehaviour
{   
    Transform[] childObjects;
    public List<Transform> childColorList = new List<Transform>();
 
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
 
        FillNodes();
 
        for( int i=0; i< childColorList.Count; i++)
        {
            Vector3 currentPos = childColorList[i].position;
 
            if(i>0)
            {
                Vector3 prePos = childColorList[i-1].position;
                Gizmos.DrawLine(prePos,currentPos);
            }
        }
    }
 
    void FillNodes()
    {
        childColorList.Clear();
 
        childObjects = GetComponentsInChildren<Transform>();
 
        foreach (Transform child in childObjects)
        {
            if ( child != this.transform)
            {
                childColorList.Add(child);
            }
        }
    }
 
}
