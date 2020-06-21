using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhyGames.Swiper
{
    public class AnimationCenterGroup : MonoBehaviour
    {
        [SerializeField]
        private TrackController m_TrackController = null;

        private Animator m_Animator;        


        private void Awake()
        {
            m_Animator = GetComponent<Animator>();
        }

        private void Start()
        {
            m_TrackController.OnDropSuccess += OnDropSuccess;
        }


        public void OnDropSuccess()
        {
            m_Animator.SetTrigger("Drop");
        }

        public void Anim_OnDropSuccessEnded()
        {
            Debug.Log("Ended");
        }
    }
}
