using System.Collections.Generic;
using UnityEngine;


namespace PhyGames.Memory
{
    public class GameController : Singleton<GameController>
    {
        public Symbol[] Symbols
        {
            get { return m_Symbols; }
        }

        [SerializeField]
        private Symbol[] m_Symbols;

        private SequenceController m_SequenceController;
        [SerializeField]
        private UI_SymbolPopup m_SymbolPopup;

        
        private void Awake()
        {
            m_SequenceController = GetComponent<SequenceController>();
            m_SequenceController.ShuffleSequence();
        }


        public Symbol GetRandomSymbol()
        {
            return m_Symbols[Random.Range(0, m_Symbols.Length)];
        }

        public void ShowSymbolPopup(UI_SymbolCell targetCell)
        {
            m_SymbolPopup.Show(targetCell);
        }
    }
}
