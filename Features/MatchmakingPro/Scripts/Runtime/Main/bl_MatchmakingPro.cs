using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using MFPS.Internal.Structures;
using System.Linq;

namespace MFPS.Addon.MatchMakingPro
{
    public class bl_MatchmakingPro : MonoBehaviour, ILobbyCallbacks, IMatchmakingCallbacks, IInRoomCallbacks
    {
        [SerializeField] private GameObject content = null;
        [SerializeField] private Button playButton = null;

        private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();
        private bool findingMatch = false;
        private bool waitingForAnotherPlayer = false;

        /// <summary>
        /// 
        /// </summary>
        void Awake()
        {
            PhotonNetwork.AddCallbackTarget(this);
            SetupPlayButton();
            SetActive(false);
        }

        /// <summary>
        /// 
        /// </summary>
        void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetupPlayButton()
        {
            if (playButton == null)
            {
                var parent = bl_LobbyUI.Instance.AddonsButtons[13].transform.parent;
                playButton = parent.GetChild(0).GetComponent<Button>();
            }

            if(playButton == null)
            {
                Debug.LogWarning("The Play button couldn't be found, try assigning it manually in the inspector of bl_MatchmakingPro.");
                return;
            }

            playButton.onClick.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.Off);
            playButton.onClick.RemoveAllListeners();         
            playButton.onClick.AddListener(() =>
            {
                SetActive(true);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        public void FindMatchForGameMode(GameModeSettings gameMode)
        {
            if (findingMatch) return;

            findingMatch = true;

            bl_MatchmakingProUI.Instance.ShowMatchmaking();

            StartCoroutine(MatchmakingSequence());
            IEnumerator MatchmakingSequence()
            {
                bl_MatchmakingProUI.SetStateText("Searching Game");
                yield return new WaitForSeconds(2);

                if (cachedRoomList.Count <= 0)
                {
                    bl_MatchmakingProUI.SetStateText("Creating Game");
                    yield return new WaitForSeconds(1);
                    CreateRoomForGameMode(gameMode);
                    yield break;
                }

                RoomInfo roomFound = null;
                foreach (var match in cachedRoomList.Values)
                {
                    if (!match.IsVisible || !match.IsOpen) continue;
                    if (match.PlayerCount >= match.MaxPlayers) continue;
                    if ((string)match.CustomProperties[PropertiesKeys.GameModeKey] != gameMode.gameMode.ToString()) continue;
                    if ((string)match.CustomProperties[PropertiesKeys.RoomPassword] != string.Empty) continue;

                    roomFound = match;
                    break;
                }

                if (roomFound == null)
                {
                    bl_MatchmakingProUI.SetStateText("Creating Game");
                    yield return new WaitForSeconds(1);
                    CreateRoomForGameMode(gameMode);
                    yield break;
                }

                bl_MatchmakingProUI.SetStateText("Joining a Game");
                yield return new WaitForSeconds(1);

                JoinToRoom(roomFound);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void PlayRandomMode()
        {
            if (findingMatch) return;

            findingMatch = true;

            bl_MatchmakingProUI.Instance.ShowMatchmaking();

            StartCoroutine(MatchmakingSequence());
            IEnumerator MatchmakingSequence()
            {
                bl_MatchmakingProUI.SetStateText("Searching Game");
                yield return new WaitForSeconds(2);

                PhotonNetwork.JoinRandomRoom();
            }           
        }

        /// <summary>
        /// 
        /// </summary>
        public void CreateRandomMatch()
        {
            bl_MatchmakingProUI.SetStateText("Creating Game");

           /* int scid = Random.Range(0, bl_GameData.Instance.AllScenes.Count);
            var mapInfo = bl_GameData.Instance.AllScenes[scid];

            var availableModes = bl_GameData.Instance.gameModes.Where(x => x.isEnabled).ToArray();
            var allModes = mapInfo.GetAllowedGameModes(availableModes);
            int modeRandom = Random.Range(0, allModes.Length);
            var gameMode = allModes[modeRandom];

            CreateRoomForGameMode(gameMode, mapInfo);*/
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameMode"></param>
        public void CreateRoomForGameMode(GameModeSettings gameMode, MapInfo mapInfo = null)
        {
            bl_MatchmakingProUI.SetStateText("Joining to the game");

            if (mapInfo == null)
            {

                var compatibleMaps = bl_GameData.Instance.AllScenes.Where(x =>
                {
                    if (x.NoAllowedGameModes.Contains(gameMode.gameMode)) return false;
                    return true;
                }).ToList();

                int scid = Random.Range(0, compatibleMaps.Count);
                mapInfo = compatibleMaps[scid];
            }

            int maxPlayersRandom = Random.Range(0, gameMode.maxPlayers.Length);
            int timeRandom = Random.Range(0, gameMode.timeLimits.Length);
            int randomGoal = Random.Range(0, gameMode.GameGoalsOptions.Length);

            var roomInfo = new MFPSRoomInfo();
            roomInfo.roomName = string.Format("[PUBLIC] {0}{1}", bl_PhotonNetwork.NickName.Substring(0, 2), Random.Range(0, 9999));
            roomInfo.gameMode = gameMode.gameMode;
            roomInfo.time = gameMode.timeLimits[timeRandom];
            roomInfo.sceneName = mapInfo.RealSceneName;
            roomInfo.roundStyle = gameMode.GetAllowedRoundMode();
            roomInfo.autoTeamSelection = gameMode.AutoTeamSelection;
            roomInfo.mapName = mapInfo.ShowName;
            roomInfo.goal = gameMode.GetGoalValue(randomGoal);
            roomInfo.friendlyFire = false;
            roomInfo.maxPing = 1000;
            roomInfo.password = bl_MatchmakingProUI.Instance.IsRoomOptionPublic() ? string.Empty : bl_StringUtility.GenerateKey();
            roomInfo.withBots = gameMode.supportBots;
            roomInfo.maxPlayers = gameMode.maxPlayers[maxPlayersRandom];

            if (!bl_MatchmakingProUI.Instance.IsRoomOptionPublic())
            {
                // Uncomment the following line if you have applied the matchmaking private room fix
                // bl_Lobby.Instance.JoinedWithPassword = true;
            }

            bl_Lobby.Instance.CreateRoom(roomInfo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="room"></param>
        public void JoinToRoom(RoomInfo room)
        {
            bl_MatchmakingProUI.Instance.OnMatchFound();
            PhotonNetwork.JoinRoom(room.Name);
        }

        /// <summary>
        /// 
        /// </summary>
        public void CancelMatchmaking()
        {
            findingMatch = false;

            if (bl_PhotonNetwork.InRoom)
            {
                bl_MatchmakingProUI.Instance.confirmationWindow.AskConfirmation(bl_GameTexts.LeaveRoomConfirmation.Localized(211), () =>
                {
                    bl_LobbyUI.Instance.blackScreenFader.FadeIn(0.5f);
                    PhotonNetwork.LeaveRoom();
                    bl_MatchmakingProUI.Instance.CancelMatchmaking();
                    SetActive(false);
                   
                });
            }
            else
            {
                bl_MatchmakingProUI.Instance.CancelMatchmaking();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateCachedRoomList(List<RoomInfo> roomList)
        {
            foreach (RoomInfo info in roomList)
            {
                // Remove room from cached room list if it got closed, became invisible or was marked as removed
                if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
                {
                    if (cachedRoomList.ContainsKey(info.Name))
                    {
                        cachedRoomList.Remove(info.Name);
                    }
                    continue;
                }

                // Update cached room info
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList[info.Name] = info;
                }
                // Add new room info to cache
                else
                {
                    cachedRoomList.Add(info.Name, info);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="active"></param>
        public void SetActive(bool active)
        {
            content.SetActive(active);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsActive()
        {
            if (content == null) return false;
            return content.activeSelf;
        }

        #region Photon Callbacks
        public void OnJoinedLobby()
        {
        }

        public void OnLeftLobby()
        {
            SetActive(false);
        }

        public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
        {
           
        }

        public void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            UpdateCachedRoomList(roomList);
        }

        public void OnFriendListUpdate(List<FriendInfo> friendList)
        {
            
        }

        public void OnCreatedRoom()
        {
            findingMatch = false;
            if (!IsActive()) return;

            if(bl_MatchmakingProSettings.Instance.onCreatedRoomBehave == bl_MatchmakingProSettings.CreatedRoomBehave.JoinDirectly)
            {
                SetActive(false);
                return;
            }

            bl_MatchmakingProUI.SetStateText("Waiting for More players");
            waitingForAnotherPlayer = true;
        }

        public void OnCreateRoomFailed(short returnCode, string message)
        {
            findingMatch = false;
        }

        public void OnJoinedRoom()
        {
            findingMatch = false;
           
            if (!IsActive()) return;

            if(bl_PhotonNetwork.CurrentRoom.PlayerCount > 1)
            {
                SetActive(false);
                return;
            }

            if(bl_MatchmakingProSettings.Instance.onCreatedRoomBehave == bl_MatchmakingProSettings.CreatedRoomBehave.JoinDirectly)
            {
                SetActive(false);
                return;
            }

            // wait for another player
        }

        public void OnJoinRoomFailed(short returnCode, string message)
        {
            findingMatch = false;
        }

        public void OnJoinRandomFailed(short returnCode, string message)
        {
            findingMatch = false;

            if (!IsActive()) return;

            CreateRandomMatch();
        }

        public void OnLeftRoom()
        {
            findingMatch = false;
        }

        public void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (newPlayer == bl_PhotonNetwork.LocalPlayer) return;
            if(!waitingForAnotherPlayer) return;

            SetActive(false);
        }

        public void OnPlayerLeftRoom(Player otherPlayer)
        {
           
        }

        public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
           
        }

        public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
           
        }

        public void OnMasterClientSwitched(Player newMasterClient)
        {
           
        }
        #endregion

        private static bl_MatchmakingPro _instance = null;
        public static bl_MatchmakingPro Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<bl_MatchmakingPro>();
                }
                return _instance;
            }
        }

    }
}