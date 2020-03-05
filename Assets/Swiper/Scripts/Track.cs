using UnityEngine;
using UnityEngine.UI;


namespace PhyGames
{
    public class Track : MonoBehaviour
    {
        public RectTransform DropRectTransform => m_DropRectTransform;
        public Image SymbolImage => m_SymbolImage;
        public TrackHandle[] Handles { get; set; }
        public Symbol CurrentSymbol { get; private set; }

        [Header("UI References")]
        [SerializeField]
        private RectTransform m_DropRectTransform;
        [SerializeField]
        private Image m_SymbolImage;
        [SerializeField]
        private GameObject m_HandlesParent;


        private void Awake()
        {
            Handles = m_HandlesParent.GetComponentsInChildren<TrackHandle>(true);
        }


        public void Initialize(TrackController controller)
        {
            Shuffle();
            for (int i = 0; i < Handles.Length; i++)
            {
                if (!Handles[i].Initialized)
                {
                    Handles[i].Initialize(controller);
                    AssignSymbol(Handles[i], i);
                }
            }
        }

        public void Refresh(TrackHandle currentHandle = null)
        {
            Shuffle();
            for (int i = 0; i < Handles.Length; i++)
            {
                Handles[i].AssignSymbol(Swiper.GameController.Instance.GetSymbolFromList(i));
                AssignSymbol(Handles[i], i);

                if (!currentHandle || Handles[i] != currentHandle)
                {
                    Handles[i].AnimationController.AppearSymbol();
                }
            }
        }


        private void Shuffle()
        {
            Swiper.GameController.Instance.ShuffleSymbols();
            CurrentSymbol = Swiper.GameController.Instance.GetRandomSymbol(Handles.Length);
            SymbolImage.sprite = CurrentSymbol.image;
        }

        private void AssignSymbol(TrackHandle handle, int symbolIndex)
        {
            handle.AssignSymbol(Swiper.GameController.Instance.GetSymbolFromList(symbolIndex));
        }
    }
}
