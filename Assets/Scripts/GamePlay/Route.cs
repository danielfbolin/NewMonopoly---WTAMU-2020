using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Route : MonoBehaviour
{
    Transform[] childObjects;


    // name of titles 

    public List<Transform> childNodeList = new List<Transform>();


    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        FillNodes();

        for( int i=0; i< childNodeList.Count; i++)
        {
            Vector3 currentPos = childNodeList[i].position;

            if(i>0)
            {
                Vector3 prePos = childNodeList[i-1].position;
                Gizmos.DrawLine(prePos,currentPos);
            }
        }
    }

    void FillNodes()
    {
        childNodeList.Clear();

        childObjects = GetComponentsInChildren<Transform>();

        foreach (Transform child in childObjects)
        {
            if ( child != this.transform)
            {
                childNodeList.Add(child);
            }
        }
    }
}
