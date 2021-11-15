using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChibiKnight.UI
{
    public class URLButton : MonoBehaviour
    {
        [SerializeField]
        private string m_url;

        public void OpenURL()
        {
            Application.OpenURL(m_url);
        }
    }
}