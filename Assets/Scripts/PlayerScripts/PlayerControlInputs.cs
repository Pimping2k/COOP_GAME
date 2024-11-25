using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControlInputs : MonoBehaviour
{
    [SerializeField] private SixthSenseAbilityComponent _abilityComponent;
    
    private IA_PlayerInputs _iaPlayerInputs;

    private void OnEnable()
    {
        _iaPlayerInputs = new IA_PlayerInputs();
        _iaPlayerInputs.Input.TurnAbility.performed += OnTurnAbility;
        _iaPlayerInputs.Enable();
    }

    private void OnTurnAbility(InputAction.CallbackContext obj)
    {
        _abilityComponent.TurnOn();
    }
}