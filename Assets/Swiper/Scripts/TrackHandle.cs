using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace PhyGames
{
    [RequireComponent(typeof (RectTransform), typeof (CanvasGroup))]
    public class TrackHandle : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public bool Initialized { get; private set; }
        public RectTransform RectTransform { get; private set; }
        public Symbol Symbol { get; private set; }

        [SerializeField]
        private Image m_SymbolImage;

        [SerializeField]
        private RectTransform m_HandleGroupRectTransform;

        private Canvas m_Canvas;
        private CanvasGroup m_CanvasGroup;
        private TrackController m_TrackController;
        private SwipeInfo m_SwipeInfo;
        private Vector3 m_InitialPosition;
        private float m_HandleGroupInitialWidth;


        private void Awake()
        {
            m_Canvas = GetComponentInParent<Canvas>();
            m_CanvasGroup = GetComponent<CanvasGroup>();
            RectTransform = GetComponent<RectTransform>();
            gameObject.SetActive(false);
        }


        public void Initialize(TrackController controller)
        {
            m_TrackController = controller;
            Initialized = true;
            gameObject.SetActive(true);
        }

        public void ResetPosition()
        {
            RectTransform.position = m_InitialPosition;
        }
        
        public void AssignSymbol(Symbol symbol)
        {
            Symbol = symbol;
            m_SymbolImage.sprite = symbol.image;
        }


        public void OnBeginDrag(PointerEventData eventData)
        {
            m_SwipeInfo = new SwipeInfo(RectTransform.position);
            m_CanvasGroup.blocksRaycasts = false;

            m_InitialPosition = RectTransform.position;
            m_HandleGroupInitialWidth = m_HandleGroupRectTransform.rect.width;
        }

        public void OnDrag(PointerEventData eventData)
        {
            m_SwipeInfo.LastPosition = eventData.position;

            Vector3 dragVector = m_SwipeInfo.LastPosition - m_SwipeInfo.StartPosition;
            Vector3 trackVector = m_TrackController.DropRectTransform.position - m_SwipeInfo.StartPosition;
            Vector3 handlePosition = m_SwipeInfo.StartPosition + Vector3.Project(dragVector, trackVector.normalized);

            float[] xPositions = { m_TrackController.DropRectTransform.position.x, m_InitialPosition.x };
            float[] yPositions = { m_TrackController.DropRectTransform.position.y, m_InitialPosition.y };

            handlePosition.x = Mathf.Clamp(handlePosition.x, Mathf.Min(xPositions), Mathf.Max(xPositions));
            handlePosition.y = Mathf.Clamp(handlePosition.y, Mathf.Min(yPositions), Mathf.Max(yPositions));

            Rect updatedGroupRect = new Rect(m_HandleGroupRectTransform.rect);
            updatedGroupRect.width = m_HandleGroupInitialWidth - (handlePosition - m_InitialPosition).magnitude;
            updatedGroupRect.width = Mathf.Clamp(updatedGroupRect.width, 0f, m_HandleGroupInitialWidth);
            m_HandleGroupRectTransform.sizeDelta = new Vector2(updatedGroupRect.width, m_HandleGroupRectTransform.sizeDelta.y);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            m_CanvasGroup.blocksRaycasts = true;
            m_HandleGroupRectTransform.sizeDelta = new Vector2(m_HandleGroupInitialWidth, m_HandleGroupRectTransform.sizeDelta.y);
            m_TrackController.OnHandleDrop(this);
        }


        private struct SwipeInfo
        {
            public Vector3 StartPosition { get; set; }
            public Vector3 LastPosition { get; set; }

            public SwipeInfo(Vector3 position)
            {
                StartPosition = LastPosition = position;
            }
        }
    }
}

