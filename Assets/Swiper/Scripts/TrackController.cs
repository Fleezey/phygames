using UnityEngine;
using UnityEngine.UI;


namespace PhyGames
{
    public class TrackController : MonoBehaviour
    {
        public RectTransform DropRectTransform => m_Track.DropRectTransform;
        public Symbol CurrentSymbol => m_Track.CurrentSymbol;

        [SerializeField]
        private Track m_Track;


        private void Start()
        {
            StartNextRound();
        }

        
        public void OnHandleDrop(TrackHandle handle)
        {
            Rect handleRect = GetRectFromRectTransform(handle.RectTransform);
            Rect dropRect = GetRectFromRectTransform(DropRectTransform);

            handle.ResetPosition();

            if (handleRect.Overlaps(dropRect) && handle.Symbol == CurrentSymbol)
            {
                StartNextRound();
            }
        }


        private void StartNextRound()
        {
            m_Track.Initialize(this);
        }

        private Rect GetRectFromRectTransform(RectTransform rectTransform)
        {
            return new Rect(
                rectTransform.anchoredPosition.x,
                rectTransform.anchoredPosition.y,
                rectTransform.rect.width,
                rectTransform.rect.height
            );
        }
    }
}
