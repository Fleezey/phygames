using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace PhyGames.Memory
{
    public class UI_SymbolPopup : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_SymbolCellContainer;

        private UI_SymbolCell m_TargetCell;
        private List<UI_SymbolCell> m_SymbolCells;


        private void Start()
        {
            InitializeSymbolCells();
        }

        public void Show(UI_SymbolCell targetCell)
        {
            m_TargetCell = targetCell;
            gameObject.SetActive(true);
        }

        public void SelectSymbol(UI_SymbolCell cell)
        {
            m_TargetCell.AssignSymbol(cell.Symbol);
            Hide();
        }

        public void Hide()
        {
            m_TargetCell = null;
            gameObject.SetActive(false);
        }


        private void InitializeSymbolCells()
        {
            Symbol[] symbols = GameController.Instance.Symbols;
            m_SymbolCells = new List<UI_SymbolCell>(m_SymbolCellContainer.GetComponentsInChildren<UI_SymbolCell>());

            for (int i = 0; i < m_SymbolCells.Count; i++)
            {
                m_SymbolCells[i].AssignSymbol(symbols[i]);
                m_SymbolCells[i].SetSymbolVisibility(true);
                m_SymbolCells[i].m_OnClickCell += SelectSymbol;
            }
        }
    }
}
