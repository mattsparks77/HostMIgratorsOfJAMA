using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AutoNetwork : NetworkManager {

    /// <summary>
    /// If this client is the host:
    /// This class should send a message to the Unity matchmaking service
    /// to start a match, and provide its public IP address so that other
    /// users can connect directly.
    /// 
    /// Does this include a port number? Or would it be better for all hosts
    /// to use a static port? Or is that impossible with the NAT in the way?
    /// 
    /// If this client is not the host:
    /// Look for any match on the Unity matchmaking service and connect
    /// directly to the address found there.
    /// </summary>

    void LookForActiveHost(){
        
    }

    void BecomeActiveHost(){
        
    }

}
