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
            StartCoroutine(DelayLoadPlayScene());
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        private IEnumerator DelayLoadPlayScene()
        {
            yield return new WaitForSecondsRealtime(0.5f);
            SceneManager.LoadScene(m_mainSceneIndex);
        }
    }

}