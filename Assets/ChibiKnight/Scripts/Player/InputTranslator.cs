using UnityEngine;
using UnityEngine.InputSystem;

public class InputTranslator : MonoBehaviour
{
    public float horizontalInput;
    public bool jumpPressed;
    public bool jumpHeld;
    public bool slashPressed;
    public bool walkPressed;
    public bool walkHeld;

    private PlayerInput m_input;

    public void Disable()
    {
        if (this.enabled)
        {
            Reset();
        }
        m_input.enabled = false;
        this.enabled = false;
    }

    public void Enable()
    {
        if (this.enabled == false)
        {
            Reset();
        }
        m_input.enabled = true;
        this.enabled = true;
    }

    private void OnHorizontalInput(InputValue value)
    {
        if (enabled == true)
        {
            horizontalInput = value.Get<float>();

            if (horizontalInput < 1 && horizontalInput > -1)
            {
                horizontalInput = 0;
            }
        }
    }

    private void OnJump(InputValue value)
    {
        if (enabled == true)
        {
            var isTrue = value.Get<float>() == 1;
            jumpPressed = isTrue;
            jumpHeld = isTrue;
        }
    }

    private void OnSlash(InputValue value)
    {
        if (enabled == true)
        {
            slashPressed = value.Get<float>() == 1;
        }
    }

    private void OnWalk(InputValue value)
    {
        if (enabled == true)
        {
            var isTrue = value.Get<float>() == 1;
            walkPressed = isTrue;
            walkHeld = isTrue;
        }
    }

    void Awake()
    {
        m_input = GetComponent<PlayerInput>();
    }

    private void Update()
    {

    }

    private void LateUpdate()
    {
        jumpPressed = false;
        slashPressed = false;
        walkPressed = false;
    }

    private void Reset()
    {
        horizontalInput = 0;
        jumpPressed = false;
        jumpHeld = false;
        slashPressed = false;
        walkPressed = false;
        walkHeld = false;
    }
}
