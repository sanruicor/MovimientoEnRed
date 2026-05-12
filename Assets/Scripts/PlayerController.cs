using Unity.Netcode;
using UnityEngine;


public class PlayerController : NetworkBehaviour
{
    private float moveSpeed = 5f;
    private float jumpForce = 6f;

    private PlayerInputActions m_Input;
    private Rigidbody m_Rb;
    private bool m_IsGrounded;

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
        bool jump = m_Input.Player.Jump.WasPressedThisFrame();

        SendInputServerRpc(moveInput, jump);
    }

    [Rpc(SendTo.Server)]
    private void SendInputServerRpc(Vector2 moveInput, bool jump)
    {
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y) * moveSpeed;
        m_Rb.linearVelocity = new Vector3(move.x, m_Rb.linearVelocity.y, move.z);

        if (jump && m_IsGrounded)
            m_Rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}