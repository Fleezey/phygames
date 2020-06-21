using System;
using UnityEngine;
using UnityEngine.UI;


namespace PhyGames
{
    public class TrackController : MonoBehaviour
    {
        public RectTransform DropRectTransform => m_Track.DropRectTransform;
        public Symbol CurrentSymbol => m_Track.CurrentSymbol;

        public Action OnDropSuccess;

        [SerializeField]
        private Track m_Track;


        private void Start()
        {
            StartNextRound();
        }

        
        public void OnHandleDrop(TrackHandle handle)
        {
            if (handle.Symbol == CurrentSymbol)
            {
                if (OnDropSuccess != null)
                {
                    OnDropSuccess();
                }

                StartNextRound();
            }
        }


        private void StartNextRound()
        {
            m_Track.Initialize(this);
        }
    }
}
