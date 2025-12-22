using UnityEngine;

namespace MFPS.Addon.NamePlatePro
{
    public abstract class bl_NamePlaterProUIBase : bl_MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        public abstract void Setup(bl_PlayerNamePlatePro player);

        /// <summary>
        /// 
        /// </summary>
        public abstract void RefreshTeamVisibility();

        /// <summary>
        /// Set Active the whole UI
        /// </summary>
        /// <param name="active"></param>
        public virtual void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="visible"></param>
        public abstract void SetVisible(bool visible);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        public abstract void SetMainColor(Color color);
    }
}