using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Script
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private Color baseColor, offsetColor;
        [SerializeField] private GameObject highlight;
        [SerializeField] private SpriteRenderer sr;
        
        public void Init(bool _b_isOffset)
        {
            //给不同组的瓦片设置不同的颜色
            sr.color = _b_isOffset ? offsetColor : baseColor;
        }

        private void OnMouseEnter()
        {
            highlight.SetActive(true);
        }

        private void OnMouseExit()
        {
            highlight.SetActive(false);
        }
    }
}
