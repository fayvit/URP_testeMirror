using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FayvitUI
{
    public class FayvitUiUtility
    {
        public static Vector2 GetMinPositionInTheParent(RectTransform pai, RectTransform filho)
        {
            return new Vector2(filho.anchorMin.x + filho.offsetMin.x / pai.rect.width,
                1 - (filho.anchorMax.y + filho.offsetMax.y / pai.rect.height));
        }

        public static Vector2 GetMaxPositionInTheParent(RectTransform pai, RectTransform filho)
        {
            Vector2 extends = GetPercentExtendsInTheParent(pai.rect, filho.rect);
            Vector2 min = GetMinPositionInTheParent(pai, filho);
            return new Vector2(extends.x + min.x, extends.y + min.y);
        }

        public static Vector2 GetPercentExtendsInTheParent(Rect pai, Rect filho)
        {
            return new Vector2(filho.width / pai.width, filho.height / pai.height);
        }
    }
}
