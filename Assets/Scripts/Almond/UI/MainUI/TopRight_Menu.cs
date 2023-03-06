#if !SERVER_DRIVE
using System.Resources;
using k514;
using UnityEngine;
using UnityEngine.UI;

namespace UI2020
{
    public partial class MainUI
    {
        private void Initialize_TopRight_Menu()
        {
            var menuPath = "TopRight_Menu";

            CloseMenuPanel();
        }

        public void CloseMenuPanel()
        {
            menuPanel.SetActive(false);
        }
    }
}
#endif