using Photon.Pun;
using UnityEngine;

namespace MFPS.Addon.NamePlatePro
{
    public class bl_PlayerNamePlatePro : bl_NamePlateBase
    {
        private bl_NamePlaterProUIBase bindingUI;
        public bl_PlayerReferences playerReferences { get; private set; }
        public bl_AIShooterReferences botReferences { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsRealPlayer()
        {
            return playerReferences != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsTeamMateOfLocalPlayer()
        {
            Team pt = playerReferences != null ? playerReferences.PlayerTeam : botReferences.PlayerTeam;
            return bl_RoomSettings.Instance != null && bl_RoomSettings.Instance.isOneTeamMode ? false : pt == bl_MFPS.LocalPlayer.Team;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsLocal()
        {
            return playerReferences != null ? playerReferences.playerNetwork.isMine : botReferences.shooterNetwork.isMine;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetHealth()
        {
            return playerReferences != null ? playerReferences.playerHealthManager.GetHealth() : botReferences.shooterHealth.GetHealth();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Awake()
        {
            if (PlayerRefs.GetComponent<PhotonView>().IsMine && PlayerRefs.GetComponent<bl_PlayerNetwork>() != null)
            {
                SetActive(false);
                return;
            }

            base.Awake();

            playerReferences = GetComponent<bl_PlayerReferences>();
            botReferences = GetComponent<bl_AIShooterReferences>();

            bindingUI = bl_NamePlateProManager.BindPlayer(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="active"></param>
        public override void SetActive(bool active)
        {
            if (bindingUI != null) bindingUI.SetActive(active);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        public void SetColor(Color color)
        {
            if (bindingUI != null) bindingUI.SetMainColor(color);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerName"></param>
        public override void SetName(string playerName)
        {
            // Not needed here
        }

        private bl_PlayerReferencesCommon _playerRefs = null;
        public bl_PlayerReferencesCommon PlayerRefs
        {
            get
            {
                if (_playerRefs == null) _playerRefs = GetComponent<bl_PlayerReferencesCommon>();
                return _playerRefs;
            }
        }
    }
}