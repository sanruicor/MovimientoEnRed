using Unity.Netcode;

public enum AuthorityMode { ServerAuthority, ServerAuthorityWithRewind, ClientAuthority }

public class AuthorityModeManager : NetworkBehaviour
{
    public static AuthorityModeManager Instance;

    public NetworkVariable<AuthorityMode> CurrentMode = new(AuthorityMode.ServerAuthority, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Awake()
    {
        Instance = this;
    }

    [Rpc(SendTo.Server)]
    public void RequestModeChangeServerRpc(AuthorityMode newMode)
    {
        CurrentMode.Value = newMode;
    }
}