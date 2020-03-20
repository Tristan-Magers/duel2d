using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private const string typeName = "UniqueGameName";
    private const string gameName = "RoomName";

    private void StartServer()
    {
        //Network.InitializeServer(4, 25000, !Network.HavePublicAddress());
        //MasterServer.RegisterHost(typeName, gameName);
    }
   

    void OnServerInitialized()
    {
        Debug.Log("Server Initializied");
    }
}
