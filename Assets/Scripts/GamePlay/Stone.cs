using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Stone : MonoBehaviour
{
    public Route currentRoute;
    int routePosition;
    int namePos;
    Route r;

    

    public int steps, dice1Value,dice2Value;

    bool isMoving;

    public int GetDiceValue( int value)
    {
        dice1Value = value;
        return dice1Value;
    }
    public int GetDice2Value( int value)
    {
        dice2Value = value;
        return dice2Value;
    }

    
    public void Movement(int a )
    {      
        steps = a;
        if( steps !=0)
        {
            StartCoroutine(Move());
        }               
    }

    IEnumerator Move()
    {
        if (isMoving)
        {
            yield break;
        }

        isMoving = true;
        while(steps>0)
        {
            routePosition++;
            
            routePosition %= currentRoute.childNodeList.Count;

            

            Vector3 nextPos = currentRoute.childNodeList[routePosition].position;
            while(MoveToNextNode(nextPos))
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);

            steps--;
            //routePosition++;
        }

        r = GameObject.Find("Route").GetComponent<Route>(); /* without this line, code will cause NullReferenceException: Object reference not set to an instance of an 
        object - This error is caused when an object is trying to be used by a script but does not refer to an instance of an object.*/
        if (routePosition == currentRoute.childNodeList.Count)
        {
            namePos = 0;
        }
        else if (routePosition > currentRoute.childNodeList.Count)
            namePos = routePosition-1- currentRoute.childNodeList.Count;
        else
            namePos = routePosition;        

        isMoving = false;
    }

    bool MoveToNextNode (Vector3 goal)
    {
        return goal !=(transform.position = Vector3.MoveTowards(transform.position,goal,8f * Time.deltaTime));

    }
}
