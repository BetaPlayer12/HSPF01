using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SystemInputTranslator : MonoBehaviour
{
    public bool resetPressed;

    private PlayerInput m_input;

    private void OnReset(InputValue value)
    {
        if (enabled == true)
        {
            var isTrue = value.Get<float>() == 1;
            resetPressed = isTrue;
        }
    }

    void Awake()
    {
        m_input = GetComponent<PlayerInput>();
    }

    private void LateUpdate()
    {
        resetPressed = false;
    }

    private void Reset()
    {
        resetPressed = false;
    }
}
