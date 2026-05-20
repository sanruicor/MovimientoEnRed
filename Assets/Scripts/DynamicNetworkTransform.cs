using Unity.Netcode.Components;
using UnityEngine;

// Un único componente que cambia su comportamiento interno dinámicamente
[DisallowMultipleComponent]
public class DynamicNetworkTransform : NetworkTransform
{
    protected override bool OnIsServerAuthoritative()
    {
        // Si el mánager no existe o estamos en los modos de Servidor, devolvemos true.
        // Si estamos en ClientAuthority, devolvemos false (dando autoridad al dueño).
        if (AuthorityModeManager.Instance == null) return true;

        return AuthorityModeManager.Instance.CurrentMode.Value != global::AuthorityMode.ClientAuthority;
    }
}