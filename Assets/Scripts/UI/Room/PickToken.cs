using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PickToken : MonoBehaviour
{
    private ExitGames.Client.Photon.Hashtable _myCustomProperties = new ExitGames.Client.Photon.Hashtable();

    private void SetTokens(string token)
    {
        _myCustomProperties["Token"] = token;
        PhotonNetwork.SetPlayerCustomProperties(_myCustomProperties);
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + ": " + token);
    }

    public void OnClick_Cylinder()
    {
        SetTokens("Cylinder");
    }


    public void OnClick_Cube()
    {
        SetTokens("Cube");
    }


    public void OnClick_Capsule()
    {
        SetTokens("Capsule");
    }


    public void OnClick_Sphere()
    {
        SetTokens("Sphere");
    }
}
