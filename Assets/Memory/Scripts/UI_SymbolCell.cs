using System;
using UnityEngine;
using UnityEngine.UI;


namespace PhyGames.Memory
{
    public class UI_SymbolCell : MonoBehaviour
    {
        public Symbol Symbol
        {
            get { return m_Symbol; }
        }

        public Action<UI_SymbolCell> m_OnClickCell;

        [SerializeField]
        private Image m_SymbolImage;

        [SerializeField]
        private Symbol m_Symbol;


        private void Awake()
        {
            m_SymbolImage.material = new Material(m_SymbolImage.material);
        }


        public void SetEnabled(bool isEnabled)
        {
            gameObject.SetActive(isEnabled);
        }

        public void SetSymbolVisibility(bool isVisible)
        {
            m_SymbolImage.enabled = isVisible;
        }

        public void AssignSymbol(Symbol symbol)
        {
            m_Symbol = symbol;
            SetSymbolTexture(symbol);
        }

        public void ClearSymbol()
        {
            m_Symbol = null;
            SetSymbolTexture(null);
        }


        private void SetSymbolTexture(Symbol symbol)
        {
            Texture texture = symbol != null ? symbol.image.texture : null;
            m_SymbolImage.material.SetTexture("_Sprite", texture);
        }

        public void OnClick()
        {
            if (m_OnClickCell != null)
            {
                m_OnClickCell(this);
            }
        }
    }
}
