using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class PlayerController : NetworkBehaviour
{
    private float moveSpeed  = 5f;
    private float jumpForce  = 6f;
    private float panelLimit = 4.5f;

    private PlayerInputActions m_Input;
    private Rigidbody m_Rb;

    private bool IsGrounded() =>
        Physics.Raycast(transform.position, Vector3.down, 1.1f);

    public override void OnNetworkSpawn()
    {
        m_Rb = GetComponent<Rigidbody>();
    
        if (!IsOwner)
        {
            enabled = false;
            return;
        }

        m_Input = new PlayerInputActions();
        m_Input.Player.Enable();
    }

    private void Update()
    {
        Vector2 moveInput = m_Input.Player.Move.ReadValue<Vector2>();
        bool    jump      = m_Input.Player.Jump.WasPressedThisFrame();

        switch (AuthorityModeManager.Instance.CurrentMode.Value)
        {
            case AuthorityMode.ServerAuthority:
                GetComponent<NetworkTransform>().AuthorityMode = NetworkTransform.AuthorityModes.Server;
                SendInputServerRpc(moveInput, jump);
                break;

            case AuthorityMode.ServerAuthorityWithRewind:
                GetComponent<NetworkTransform>().AuthorityMode = NetworkTransform.AuthorityModes.Server;
                SendInputServerRpc(moveInput, jump);
                break;

            case AuthorityMode.ClientAuthority:
                GetComponent<NetworkTransform>().AuthorityMode = NetworkTransform.AuthorityModes.Owner;
                SendInputServerRpc(moveInput, jump);
                break;
        }
    }

    [Rpc(SendTo.Server)]
    private void SendInputServerRpc(Vector2 moveInput, bool jump)
    {
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y) * moveSpeed;
        m_Rb.linearVelocity = new Vector3(move.x, m_Rb.linearVelocity.y, move.z);

        if (jump && IsGrounded())
        {
            m_Rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        Vector3 position = transform.position;
        position.x = Mathf.Clamp(position.x, -panelLimit, panelLimit);
        position.z = Mathf.Clamp(position.z, -panelLimit, panelLimit);
        transform.position = position;
    }
}