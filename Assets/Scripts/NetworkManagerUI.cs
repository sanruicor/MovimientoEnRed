using Unity.Netcode;
using UnityEngine;

public class NetworkManagerUI : MonoBehaviour
{
    private NetworkManager m_Net;

    private void Awake() => m_Net = GetComponent<NetworkManager>();

    private void OnGUI()
    {
        if (!m_Net.IsClient && !m_Net.IsServer)
        {
            GUILayout.BeginArea(new Rect(10, 10, 200, 80));
            if (GUILayout.Button("Host"))   m_Net.StartHost();
            if (GUILayout.Button("Client")) m_Net.StartClient();
            GUILayout.EndArea();
            return;
        }

        if (AuthorityModeManager.Instance == null) return;

        GUILayout.BeginArea(new Rect(10, 10, 220, 110));
        GUILayout.Label("Modo: " + AuthorityModeManager.Instance.CurrentMode.Value);
        if (GUILayout.Button("Server Authority"))
            AuthorityModeManager.Instance.RequestModeChangeServerRpc(AuthorityMode.ServerAuthority);
        if (GUILayout.Button("Server Authority + Rewind"))
            AuthorityModeManager.Instance.RequestModeChangeServerRpc(AuthorityMode.ServerAuthorityWithRewind);
        if (GUILayout.Button("Client Authority"))
            AuthorityModeManager.Instance.RequestModeChangeServerRpc(AuthorityMode.ClientAuthority);
        GUILayout.EndArea();
    }
}