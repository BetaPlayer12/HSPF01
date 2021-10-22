using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Reset : MonoBehaviour
{
    [SerializeField]
    private SystemInputTranslator m_input;

    void Update()
    {
        if (m_input.resetPressed)
        {
            Debug.Log("RESET");
            SceneManager.LoadScene(0);
        }
    }
}
