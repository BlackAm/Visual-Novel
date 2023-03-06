#if !SERVER_DRIVE
using UnityEngine.UI;

namespace UI2020
{
    public class ItemSelectPopUp : AbstractUI
    {
        

        public void Init()
        {
            GetComponent<Button>("EquipPopUp/CloseButton").onClick.AddListener(ClosePopUp);
        }

        public void ClosePopUp()
        {
            SetActive(false);
        }
    }
}
#endif