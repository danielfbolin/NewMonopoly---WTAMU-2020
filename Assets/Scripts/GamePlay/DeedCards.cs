using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeedCards : MonoBehaviour
{
    Transform[] childObjects;
    public List<Transform> childDeedCardList = new List<Transform>();

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        FillNodes();

        for( int i=0; i< childDeedCardList.Count; i++)
        {
            Vector3 currentPos = childDeedCardList[i].position;

            if(i>0)
            {
                Vector3 prePos = childDeedCardList[i-1].position;
                Gizmos.DrawLine(prePos,currentPos);
            }
        }
    }

    void FillNodes()
    {
        childDeedCardList.Clear();

        childObjects = GetComponentsInChildren<Transform>();

        foreach (Transform child in childObjects)
        {
            if ( child != this.transform)
            {
                childDeedCardList.Add(child);
            }
        }
    }
}

