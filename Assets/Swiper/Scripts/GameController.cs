using System.Collections.Generic;
using UnityEngine;


namespace PhyGames.Swiper
{
    public class GameController : Singleton<GameController>
    {
        [SerializeField]
        private Symbol[] m_Symbols;

        private List<Symbol> m_RandomSymbols;

        
        private void Awake()
        {
            m_RandomSymbols = new List<Symbol>(m_Symbols);    
        }


        public void ShuffleSymbols()
        {
            ListExtension.Shuffle<Symbol>(m_RandomSymbols);
        }

        public Symbol GetRandomSymbol(int symbolCount)
        {
            return m_RandomSymbols[Random.Range(0, symbolCount)];
        }

        public Symbol GetSymbolFromList(int index)
        {
            return m_RandomSymbols[index];
        }
    }
}
