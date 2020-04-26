using UnityEngine;
using UnityEngine.UI;


namespace PhyGames.Swiper
{
    [RequireComponent(typeof (Image))]
    public class SymbolChanger : MonoBehaviour
    {
        [SerializeField]
        private string m_ShaderSymbolTexturePropertyName = "";

        private Image m_SymbolImage;


        private void Awake()
        {
            m_SymbolImage = GetComponent<Image>();
            m_SymbolImage.material = new Material(m_SymbolImage.material);
        }

        public void SetSymbol(Symbol symbol)
        {
            if (m_ShaderSymbolTexturePropertyName.Equals(""))
            {
                m_SymbolImage.sprite = symbol.image;
            }
            else
            {
                m_SymbolImage.material.SetTexture(m_ShaderSymbolTexturePropertyName, symbol.image.texture);
            }
        }
    }    
}
