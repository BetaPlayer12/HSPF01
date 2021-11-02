using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ChibiKnight.MainMenu
{
    public class MainMenuNavigationHandle : MonoBehaviour
    {
        [SerializeField]
        private int m_mainSceneIndex;

        public void StartMainGame()
        {
            SceneManager.LoadScene(m_mainSceneIndex);
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }

}