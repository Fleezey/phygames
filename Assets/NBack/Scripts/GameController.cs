using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PhyGames.Audio;


namespace PhyGames.NBack
{
    public class GameController : MonoBehaviour
    {
        public int m_BackAmount = 1;
        public Symbol[] m_Symbols;
        public Image[] m_ImageHolders;
        public GameObject m_ControlsUI;

        private Symbol m_CurrentSymbol;
        private LinkedList<Symbol> m_SymbolsSequence;


        private void Start()
        {
            m_ControlsUI.SetActive(false);
            m_SymbolsSequence = new LinkedList<Symbol>();
            StartCoroutine("ShowStartingSymbols");
        }


        public void HandleSameSymbolAction()
        {
            ValidateSymbol(true);
            ShowNextSymbol();
        }

        public void HandleDifferentSymbolAction()
        {
            ValidateSymbol(false);
            ShowNextSymbol();
        }

        private void ValidateSymbol(bool input)
        {
            Symbol symbolToCompare = GetPreviousSymbol();
            if ((m_CurrentSymbol == symbolToCompare) == input)
            {
                AudioManager.Instance.PlaySound("Good");
            }
            else
            {
                AudioManager.Instance.PlaySound("Wrong");
            }
        }

        private Symbol GetPreviousSymbol()
        {
            LinkedListNode<Symbol> node = m_SymbolsSequence.Last;

            for (int i = 1; i < m_BackAmount; i++)
            {
                node = node.Previous;
            }

            return node.Value;
        }


        private IEnumerator ShowStartingSymbols()
        {
            int symbolsShown = 0;
            while (symbolsShown < m_BackAmount)
            {
                ShowNextSymbol();
                symbolsShown++;
                yield return new WaitForSeconds(2);
            }

            m_ControlsUI.SetActive(true);
            AudioManager.Instance.PlaySound("Appear");
            ShowNextSymbol();
        }

        private void AssignSymbol(Symbol symbol)
        {
            m_CurrentSymbol = symbol;
			
			foreach (Image imageHolder in m_ImageHolders)
			{
				imageHolder.sprite = m_CurrentSymbol.image;
			}
        }

        private void ShowNextSymbol()
        {
            if (m_CurrentSymbol) 
            {
                m_SymbolsSequence.AddLast(m_CurrentSymbol);
            }

            AssignSymbol(GetRandomSymbol());
        }

        private Symbol GetRandomSymbol()
        {
            return m_Symbols[Random.Range(0, m_Symbols.Length)];
        }
    }
}
