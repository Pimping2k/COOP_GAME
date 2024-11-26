using System;
using DefaultNamespace;
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

    private void OnDisable()
    {
        _iaPlayerInputs.Input.TurnAbility.performed -= OnTurnAbility;
        _iaPlayerInputs.Disable();
    }

    private void OnTurnAbility(InputAction.CallbackContext obj)
    {
        GlobalVariablesContainer.IsAbilityActive = !GlobalVariablesContainer.IsAbilityActive;
        
        if (GlobalVariablesContainer.IsAbilityActive)
        {
            _abilityComponent.TurnOn();
        }
        else
        {
            _abilityComponent.Turnoff();
        }
    }
}