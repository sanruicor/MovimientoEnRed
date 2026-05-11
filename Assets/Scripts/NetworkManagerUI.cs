using Unity.Netcode;
using UnityEngine;

public class NetworkManagerUI : MonoBehaviour
{
    private NetworkManager m_Net;

    private void Awake() => m_Net = GetComponent<NetworkManager>();

    private void OnGUI()
    {
        if (m_Net.IsClient || m_Net.IsServer) return;

        GUILayout.BeginArea(new Rect(10, 10, 200, 100));
        if (GUILayout.Button("Host"))   m_Net.StartHost();
        if (GUILayout.Button("Client")) m_Net.StartClient();
        GUILayout.EndArea();
    }
}
