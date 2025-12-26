using MFPSEditor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MFPS.Addon.NamePlatePro
{
    public class bl_NamePlaterProUI : bl_NamePlaterProUIBase
    {
        [ScriptableDrawer] public bl_NamePlateProSettings settings;
        [SerializeField] private GameObject content = null;
        [SerializeField] private TextMeshProUGUI nameText = null;
        [SerializeField] private Image healthBar = null;
        [SerializeField] private Image delayedHealthBar = null;
        [SerializeField] private GameObject[] parts = null;
        [SerializeField] private Graphic[] coloredUI = null;

        public bl_PlayerReferencesCommon BindingPlayer { get; private set; }

        private Vector3 screenPosition;
        private float distance = 0;
        private int lastHealth = 0;
        private bool isVisible = true;
        private bl_PlayerNamePlatePro playerPlate;
        private float adjustment = 0;
        private float adjustedDistance;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        public override void Setup(bl_PlayerNamePlatePro player)
        {
            playerPlate = player;
            BindingPlayer = player.PlayerRefs;
            nameText.text = BindingPlayer.gameObject.name;
            SetHealth(player.GetHealth());
            gameObject.name = $"NamePlate [{BindingPlayer.gameObject.name}]";
            if (delayedHealthBar != null)
            {
                if (settings.hitEffect != bl_NamePlateProSettings.HitEffect.Delayed)
                {
                    delayedHealthBar.gameObject.SetActive(false);
                }
                else
                {
                    delayedHealthBar.canvasRenderer.SetAlpha(0);
                }
            }
            if (healthBar != null)
            {
                healthBar.canvasRenderer.SetColor(Color.white);
            }
            SetVisible(true);
            SetActive(true);
            if (!bl_GameData.Instance.ShowTeamMateHealthBar || !settings.showHealth)
            {
                SetActivePart(2, false);
            }
            OnSlowUpdate();
            OnLateUpdate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newHealth"></param>
        public void SetHealth(int newHealth)
        {
            if (healthBar == null) return;

            healthBar.fillAmount = (float)newHealth / 100f;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnLateUpdate()
        {
            FollowPlayer();
            CheckHealth();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnSlowUpdate()
        {
            CheckDistance();
        }

        /// <summary>
        /// 
        /// </summary>
        void FollowPlayer()
        {
            if (BindingPlayer == null || bl_CameraIdentity.CurrentCamera == null)
            {
                SetVisible(false);
                return;
            }

            if (!playerPlate.IsTeamMateOfLocalPlayer())
            {
                SetActive(false);
                return;
            }

            if (BindingPlayer.IsDeath() || adjustedDistance > settings.maxDisplayDistance)
            {
                SetVisible(false);
                return;
            }

            // Convert the world position of the player's head to screen space
            float vpo = playerPlate.IsRealPlayer() ? settings.verticalPosOffset + 1.3f : settings.verticalPosOffset;
            screenPosition = bl_CameraIdentity.CurrentCamera.WorldToScreenPoint(BindingPlayer.transform.position + (Vector3.up * vpo));

            SetVisible(screenPosition.z > 0);
            if (screenPosition.z > 0)
            {
                screenPosition.y += adjustment;
                CachedTransform.position = screenPosition;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void RefreshTeamVisibility()
        {
            if (BindingPlayer == null) return;
            if (playerPlate.IsLocal() && playerPlate.IsRealPlayer())
            {
                SetActive(false);
                return;
            }

            OnSlowUpdate();
            OnLateUpdate();

            SetActive(playerPlate.IsTeamMateOfLocalPlayer());
        }

        /// <summary>
        /// 
        /// </summary>
        private void CheckDistance()
        {
            if (BindingPlayer == null || bl_MFPS.LocalPlayerReferences == null) return;

            distance = bl_UtilityHelper.Distance(BindingPlayer.transform.position, bl_MFPS.LocalPlayerReferences.Position);
            adjustment = distance * settings.verticalAdjustment;
            adjustedDistance = bl_CameraIdentity.CurrentCamera != null && settings.distanceBasedFOV
                ? (bl_CameraIdentity.CurrentCamera.fieldOfView / 60f) * distance : distance;

            if (adjustedDistance > settings.minTagDisplayDistance)
            {
                SetActivePart(0, false);
            }
            else
            {
                SetActivePart(0, true);
            }

            if (!isVisible && adjustedDistance < settings.maxDisplayDistance)
            {
                SetVisible(true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void CheckHealth()
        {
            SetDelayedHealth();

            if (BindingPlayer == null) return;

            int health = playerPlate.GetHealth();
            if (health != lastHealth)
            {
                if (health < lastHealth)
                {
                    OnDamageDetected();
                }

                SetHealth(health);
                lastHealth = health;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void OnDamageDetected()
        {
            if (delayedHealthBar == null) return;

            if (settings.hitEffect == bl_NamePlateProSettings.HitEffect.Delayed)
            {
                delayedHealthBar.canvasRenderer.SetAlpha(1);
                delayedHealthBar.CrossFadeAlpha(0, 1, true);
            }
            else if (settings.hitEffect == bl_NamePlateProSettings.HitEffect.ColorFade)
            {
                if (healthBar != null)
                {
                    healthBar.canvasRenderer.SetColor(Color.red);
                    healthBar.CrossFadeColor(Color.white, 0.25f, true, true);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetDelayedHealth()
        {
            if (settings.hitEffect != bl_NamePlateProSettings.HitEffect.Delayed || delayedHealthBar == null || healthBar == null) return;

            delayedHealthBar.fillAmount = Mathf.Lerp(delayedHealthBar.fillAmount, healthBar.fillAmount, Time.deltaTime * 4f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="partIndex"></param>
        /// <param name="active"></param>
        /// <param name="deactiveOthers"></param>
        public void SetActivePart(int partIndex, bool active, bool deactiveOthers = false)
        {
            if (deactiveOthers)
            {
                for (int i = 0; i < parts.Length; i++)
                {
                    parts[i].SetActive(active ? i == partIndex : i != partIndex);
                }
            }
            else
            {
                parts[partIndex].SetActive(active);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="visible"></param>
        public override void SetVisible(bool visible)
        {
            if (isVisible == visible) return;

            content.SetActive(visible);
            isVisible = visible;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        public override void SetMainColor(Color color)
        {
            foreach (var item in coloredUI)
            {
                if (item == null) continue;

                item.canvasRenderer.SetColor(color);
            }
        }
    }
}