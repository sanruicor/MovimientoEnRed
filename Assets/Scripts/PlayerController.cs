using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(DynamicNetworkTransform))]
public class PlayerController : NetworkBehaviour
{
    private float moveSpeed  = 5f;
    private float jumpForce  = 6f;
    private float panelLimit = 4.5f;

    private PlayerInputActions m_Input;
    private Rigidbody m_Rb;
    private DynamicNetworkTransform m_DynamicTransform;

    // Inputs locales del dueño
    private Vector2 m_LocalMoveInput;
    private bool m_LocalJumpInput;

    // Inputs guardados en el servidor para los clientes remotos
    private Vector2 m_ServerRemoteMoveInput;
    private bool m_ServerRemoteJumpInput;

    private bool IsGrounded() =>
        Physics.Raycast(transform.position, Vector3.down, 1.1f);

    public override void OnNetworkSpawn()
    {
        m_Rb = GetComponent<Rigidbody>();
        m_DynamicTransform = GetComponent<DynamicNetworkTransform>();

        // Escuchar el cambio de modo global para actualizar físicas y componentes
        AuthorityModeManager.Instance.CurrentMode.OnValueChanged += OnAuthorityModeChanged;
        UpdateComponentState(AuthorityModeManager.Instance.CurrentMode.Value);

        if (!IsOwner) return;

        m_Input = new PlayerInputActions();
        m_Input.Player.Enable();
    }

    private void OnAuthorityModeChanged(AuthorityMode oldMode, AuthorityMode newMode)
    {
        UpdateComponentState(newMode);
    }

    private void UpdateComponentState(AuthorityMode mode)
    {
        // Forzamos al NetworkTransform a reevaluar quién tiene la autoridad
        // Llamar a este método interno refresca los permisos de red del objeto
        m_DynamicTransform.OnNetworkDespawn();
        m_DynamicTransform.OnNetworkSpawn();

        bool isClientAuth = mode == AuthorityMode.ClientAuthority;

        // Control estricto del Rigidbody en el servidor para evitar conflictos con el cliente dueño
        if (IsServer && !IsOwner)
        {
            m_Rb.isKinematic = isClientAuth;
        }
        if (IsOwner)
        {
            m_Rb.isKinematic = false;
        }
    }

    private void Update()
    {
        if (!IsOwner) return;

        m_LocalMoveInput = m_Input.Player.Move.ReadValue<Vector2>();
        if (m_Input.Player.Jump.WasPressedThisFrame())
        {
            m_LocalJumpInput = true;
        }
    }

    private void FixedUpdate()
    {
        AuthorityMode mode = AuthorityModeManager.Instance.CurrentMode.Value;

        if (IsOwner)
        {
            HandleOwnerPhysics(mode);
            m_LocalJumpInput = false; 
        }
        
        if (IsServer)
        {
            HandleServerRemotePhysics(mode);
        }
    }

    private void HandleOwnerPhysics(AuthorityMode mode)
    {
        switch (mode)
        {
            case AuthorityMode.ServerAuthority:
                // No se mueve localmente. Envía input y espera a que el servidor replique su posición
                SendInputServerRpc(m_LocalMoveInput, m_LocalJumpInput);
                break;

            case AuthorityMode.ServerAuthorityWithRewind:
                // Predicción (Client-Side Prediction): se mueve de inmediato
                ApplyMovement(m_LocalMoveInput, m_LocalJumpInput);
                ClampPosition();
                SendInputServerRpc(m_LocalMoveInput, m_LocalJumpInput);
                break;

            case AuthorityMode.ClientAuthority:
                // Autoridad de cliente pura: se mueve de forma autónoma
                ApplyMovement(m_LocalMoveInput, m_LocalJumpInput);
                ClampPosition();
                break;
        }
    }

    private void HandleServerRemotePhysics(AuthorityMode mode)
    {
        if (mode == AuthorityMode.ClientAuthority) return;

        if (!IsOwner)
        {
            // El servidor simula el movimiento del Cliente 2 basándose en los RPC recibidos
            ApplyMovement(m_ServerRemoteMoveInput, m_ServerRemoteJumpInput);
            ClampPosition();
            m_ServerRemoteJumpInput = false; 
        }
        else
        {
            // El Host se simula a sí mismo en el servidor de forma directa
            ApplyMovement(m_LocalMoveInput, m_LocalJumpInput);
            ClampPosition();
        }
    }

    private void ApplyMovement(Vector2 moveInput, bool jump)
    {
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y) * moveSpeed;
        m_Rb.linearVelocity = new Vector3(move.x, m_Rb.linearVelocity.y, move.z);

        if (jump && IsGrounded())
        {
            m_Rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void ClampPosition()
    {
        Vector3 position = transform.position;
        position.x = Mathf.Clamp(position.x, -panelLimit, panelLimit);
        position.z = Mathf.Clamp(position.z, -panelLimit, panelLimit);
        transform.position = position;
    }

    [Rpc(SendTo.Server)]
    private void SendInputServerRpc(Vector2 moveInput, bool jump)
    {
        if (IsOwner) return;

        m_ServerRemoteMoveInput = moveInput;
        if (jump)
        {
            m_ServerRemoteJumpInput = true;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (AuthorityModeManager.Instance != null)
        {
            AuthorityModeManager.Instance.CurrentMode.OnValueChanged -= OnAuthorityModeChanged;
        }
        m_Input?.Dispose();
    }
}