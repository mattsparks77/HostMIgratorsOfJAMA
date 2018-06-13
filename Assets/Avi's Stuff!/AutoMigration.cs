using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class AutoMigration : NetworkMigrationManager {

    [SerializeField] float reconnectDelay = 1f;
    [SerializeField] int maxReconnectAttempts = 10;
    IEnumerator reconnectCoroutine;

    protected override void OnClientDisconnectedFromHost(NetworkConnection conn, out SceneChangeOption sceneChange) {
        //base.OnClientDisconnectedFromHost(conn, out sceneChange);
        sceneChange = SceneChangeOption.StayInOnlineScene;

        PeerInfoMessage infoMessage;
        bool isNewHost;
        if(FindNewHost(out infoMessage, out isNewHost)){
            if(isNewHost){
                BecomeNewHost(infoMessage.port);
            } else {
                reconnectCoroutine = AttemptReconnect(reconnectDelay, infoMessage.address, infoMessage.port);
                StartCoroutine(reconnectCoroutine);
            }
        }

    }

    IEnumerator AttemptReconnect(float delay, string address, int port){
        for (int i = 0; i < maxReconnectAttempts; ++i){
            yield return new WaitForSecondsRealtime(delay);
            client.ReconnectToNewHost(address, port);
        }
    }

}
