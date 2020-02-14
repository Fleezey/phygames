using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PhyGames.NBack
{
    public class GameController : MonoBehaviour
    {
        public Symbol[] m_Symbols;

        private Symbol m_CurrentSymbol;
        private Queue m_previousSymbols;


        private void Start()
        {
            m_previousSymbols = new Queue();
            m_CurrentSymbol = GetRandomSymbol();
        }

        private void Update()
        {
            if (Input.GetKeyDown("space"))
            {
                m_previousSymbols.Enqueue(m_CurrentSymbol);
                m_CurrentSymbol = GetRandomSymbol();
            }
        }


        private Symbol GetRandomSymbol()
        {
            return m_Symbols[Random.Range(0, m_Symbols.Length)];
        }
    }
}
