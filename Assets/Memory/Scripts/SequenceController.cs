using System.Collections.Generic;
using UnityEngine;


namespace PhyGames.Memory
{
    public class SequenceController : MonoBehaviour
    {
        public int m_MinSize = 3;
        public int m_MaxSize = 8;

        public GameObject m_SymbolCellContainer;

        private List<Symbol> m_Symbols;
        private List<UI_SymbolCell> m_SymbolCells;

        
        private void Awake()
        {
            m_Symbols = new List<Symbol>();

            m_SymbolCells = new List<UI_SymbolCell>(m_SymbolCellContainer.GetComponentsInChildren<UI_SymbolCell>());
            foreach (UI_SymbolCell cell in m_SymbolCells)
            {
                cell.m_OnClickCell += OnClickCell;
            }
        }

        public void ShuffleSequence()
        {
            m_Symbols.Clear();

            int size = Random.Range(m_MinSize, m_MaxSize);
            for (int i = 0; i < m_MaxSize; i++)
            {
                if (i < size)
                {
                    Symbol symbol = GameController.Instance.GetRandomSymbol();
                    m_Symbols.Add(symbol);
                    m_SymbolCells[i].SetEnabled(true);
                    m_SymbolCells[i].AssignSymbol(symbol);
                }
                else
                {
                    m_SymbolCells[i].SetEnabled(false);
                    m_SymbolCells[i].ClearSymbol();
                }
            }
        }

        public void SetSequenceVisibility(bool isVisible)
        {
            foreach (UI_SymbolCell cell in m_SymbolCells)
            {
                cell.SetSymbolVisibility(isVisible);
            }
        }

        public void OnClickCell(UI_SymbolCell symbolCell)
        {
            GameController.Instance.ShowSymbolPopup(symbolCell);
        }
    }
}
