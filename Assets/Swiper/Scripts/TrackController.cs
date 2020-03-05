using UnityEngine;


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
            m_Track.Initialize(this);
        }

        
        public void OnHandleDrop(TrackHandle handle)
        {
            Rect handleRect = GetRectFromRectTransform(handle.RectTransform);
            Rect dropRect = GetRectFromRectTransform(DropRectTransform);

            handle.ResetPosition();

            if (handleRect.Overlaps(dropRect) && handle.Symbol == CurrentSymbol)
            {
                m_Track.Refresh(handle);
            }
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
