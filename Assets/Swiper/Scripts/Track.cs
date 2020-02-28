using UnityEngine;
using UnityEngine.UI;


namespace PhyGames
{
    public class Track : MonoBehaviour
    {
        public RectTransform DropRectTransform => m_DropRectTransform;
        public Image SymbolImage => m_SymbolImage;
        public Image BaseImage => m_BaseImage;
        public TrackHandle[] Handles { get; set; }
        public Symbol CurrentSymbol { get; private set; }

        [Header("UI References")]
        [SerializeField]
        private RectTransform m_DropRectTransform;
        [SerializeField]
        private Image m_SymbolImage;
        [SerializeField]
        private Image m_BaseImage;
        [SerializeField]
        private GameObject m_HandlesParent;


        private void Awake()
        {
            Handles = m_HandlesParent.GetComponentsInChildren<TrackHandle>(true);
        }


        public void Initialize(TrackController controller)
        {
            Swiper.GameController.Instance.ShuffleSymbols();
            CurrentSymbol = Swiper.GameController.Instance.GetRandomSymbol(Handles.Length);
            SymbolImage.sprite = CurrentSymbol.image;


            for (int i = 0; i < Handles.Length; i++)
            {
                if (!Handles[i].Initialized)
                {
                    Handles[i].Initialize(controller);
                }

                Handles[i].AssignSymbol(Swiper.GameController.Instance.GetSymbolFromList(i));
            }
        }
    }
}
