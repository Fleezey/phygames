using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace PhyGames.NBack
{
    public class GameController : Singleton<GameController>
    {
        public Symbol[] m_Symbols;
        public Image m_ImageHolder;
        public GameObject m_ControlsUI;

        private Symbol m_CurrentSymbol;
        private Symbol m_PreviousSymbol;


        private void Start()
        {
            m_ControlsUI.SetActive(false);
            ShowFirstSymbol();
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

        private void ValidateSymbol(bool isSame)
        {
            if ((m_CurrentSymbol.id == m_PreviousSymbol.id) == isSame)
            {
                Debug.Log("Good");
            }
            else
            {
                Debug.Log("Bad");
            }
        }


        private void ShowFirstSymbol()
        {
            AssignSymbol(GetRandomSymbol());
            StartCoroutine(ActivateUIControls());
        }

        IEnumerator ActivateUIControls()
        {
            yield return new WaitForSeconds(2);
            m_ControlsUI.SetActive(true);
            ShowNextSymbol();
        }

        private void AssignSymbol(Symbol symbol)
        {
            m_CurrentSymbol = symbol;
            m_ImageHolder.sprite = m_CurrentSymbol.image;
        }

        private void ShowNextSymbol()
        {
            m_PreviousSymbol = m_CurrentSymbol;
            AssignSymbol(GetRandomSymbol());
        }

        private Symbol GetRandomSymbol()
        {
            return m_Symbols[Random.Range(0, 3)];
            // return m_Symbols[Random.Range(0, m_Symbols.Length)];
        }
    }
}
