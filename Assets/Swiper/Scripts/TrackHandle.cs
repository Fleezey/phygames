using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace PhyGames
{
    [RequireComponent(typeof (RectTransform), typeof (CanvasGroup), typeof (TrackHandleAnimationController))]
    public class TrackHandle : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public bool Initialized { get; private set; }
        public CanvasGroup CanvasGroup => m_CanvasGroup;
        public Image SymbolImage => m_SymbolImage;
        public TrackHandleAnimationController AnimationController => m_HandleAnimationController;
        public RectTransform RectTransform { get; private set; }
        public Symbol Symbol { get; private set; }

        [SerializeField]
        private Image m_SymbolImage;

        private Canvas m_Canvas;
        private CanvasGroup m_CanvasGroup;
        private TrackController m_TrackController;
        private TrackHandleAnimationController m_HandleAnimationController;
        private SwipeInfo m_SwipeInfo;
        private Vector3 m_InitialPosition;


        private void Awake()
        {
            m_Canvas = GetComponentInParent<Canvas>();
            m_CanvasGroup = GetComponent<CanvasGroup>();
            m_HandleAnimationController = GetComponent<TrackHandleAnimationController>();
            RectTransform = GetComponent<RectTransform>();
            gameObject.SetActive(false);
        }


        public void Initialize(TrackController controller)
        {
            m_TrackController = controller;
            m_InitialPosition = RectTransform.position;
            Initialized = true;
            gameObject.SetActive(true);
            m_HandleAnimationController.Appear();
        }

        public void ResetPosition()
        {
            RectTransform.position = m_InitialPosition;
            m_HandleAnimationController.Appear();
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

            RectTransform.position = handlePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            m_CanvasGroup.blocksRaycasts = true;
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

