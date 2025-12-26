using System.Collections.Generic;
using UnityEngine;

namespace MFPS.Addon.NamePlatePro
{
    public class bl_NamePlateProManager : MonoBehaviour
    {
        public bl_NamePlaterProUIBase platePrefab = null;
        [SerializeField] private RectTransform container = null;

        private Dictionary<string, bl_NamePlaterProUIBase> cachedPlates = new Dictionary<string, bl_NamePlaterProUIBase>();

        /// <summary>
        /// 
        /// </summary>
        private void OnEnable()
        {
            bl_PhotonCallbacks.PlayerPropertiesUpdate += OnPlayerPropsChanged;
            bl_EventHandler.onLocalPlayerSpawn += OnLocalSpawn;
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnDisable()
        {
            bl_PhotonCallbacks.PlayerPropertiesUpdate -= OnPlayerPropsChanged;
            bl_EventHandler.onLocalPlayerSpawn -= OnLocalSpawn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        public static bl_NamePlaterProUIBase BindPlayer(bl_PlayerNamePlatePro playerPlate)
        {
            if (Instance == null)
            {
                bl_NamePlateProData.InitInScene();
            }

            var player = playerPlate.PlayerRefs;
            string playerName = player.gameObject.name;
            if (Instance.cachedPlates.ContainsKey(playerName))
            {
                Instance.cachedPlates[playerName].Setup(playerPlate);
                return Instance.cachedPlates[playerName];
            }
            else
            {
                var plate = Instantiate(Instance.platePrefab, Instance.container, false);
                plate.Setup(playerPlate);
                Instance.cachedPlates.Add(playerName, plate);
                return plate;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void OnLocalSpawn()
        {
            UpdateVisibility();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="entries"></param>
        void OnPlayerPropsChanged(Photon.Realtime.Player player, ExitGames.Client.Photon.Hashtable entries)
        {
            UpdateVisibility();
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateVisibility()
        {
            foreach (var item in cachedPlates)
            {
                if (item.Value == null) continue;

                item.Value.RefreshTeamVisibility();
            }
        }

        private static bl_NamePlateProManager _instance = null;
        public static bl_NamePlateProManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<bl_NamePlateProManager>();
                }
                return _instance;
            }
        }
    }
}