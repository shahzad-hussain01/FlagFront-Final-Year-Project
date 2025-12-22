using UnityEngine;
using UnityEngine.UI;
using MFPS.Internal.Structures;
using MFPS.Runtime.UI;
using TMPro;

namespace MFPS.Addon.Customizer
{
    public class bl_CustomizerInfoButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_Text = null;
        public Image weaponIcon;
        public TextMeshProUGUI lockedText;
        public Button button;
        public GameObject lockedUI;
        [SerializeField] private bl_MFPSCoinPriceUI coinPricesUI = null;

        private bl_CustomizerInfoButton[] AllButtons;
        private bl_Customizer customizerWeapon;
        private bool isUnlocked = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="weapon"></param>
        public void Init(bl_Customizer weapon)
        {
            customizerWeapon = weapon;
            var info = customizerWeapon.GetWeaponInfo();
            m_Text.text = customizerWeapon.WeaponName;
            weaponIcon.sprite = info.GunIcon;

            isUnlocked = info.Unlockability.IsUnlocked(customizerWeapon.GunID());
            if (!isUnlocked)
            {
                var reason = info.Unlockability.GetLockReason(customizerWeapon.GunID());
                if (reason == MFPSItemUnlockability.LockReason.NoPurchased || reason == MFPSItemUnlockability.LockReason.NoPurchasedAndLevel)
                {
                    coinPricesUI.SetPrice(info.Unlockability.Price).SetActive(true);
                }
                else coinPricesUI.SetActive(false);

                if (reason == MFPSItemUnlockability.LockReason.Level || reason == MFPSItemUnlockability.LockReason.NoPurchasedAndLevel)
                {
                    lockedText.text = $"LEVEL {info.Unlockability.UnlockAtLevel}";
                }
                else lockedText.text = "";

                button.interactable = false;
                lockedUI.SetActive(true);
            }
            else
            {
                button.interactable = true;
                lockedUI.SetActive(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnSelect()
        {
            if (!isUnlocked) return;

            if (AllButtons == null || AllButtons.Length <= 0) { AllButtons = transform.parent.GetComponentsInChildren<bl_CustomizerInfoButton>(); }

            bl_CustomizerManager.Instance.ShowCustomizerWeapon(customizerWeapon);

            foreach (bl_CustomizerInfoButton b in AllButtons)
            {
                b.Deselect();
            }
            button.interactable = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Deselect()
        {
            if (!isUnlocked) return;

            button.interactable = true;
        }
    }
}