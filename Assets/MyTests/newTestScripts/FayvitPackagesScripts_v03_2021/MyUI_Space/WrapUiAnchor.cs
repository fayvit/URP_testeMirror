using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FayvitUI
{
    [ExecuteInEditMode]
    public class WrapUiAnchor : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            RectTransform rt = GetComponent<RectTransform>();
            RectTransform rtt = transform.parent.GetComponent<RectTransform>();

            Vector2 min = FayvitUiUtility.GetMinPositionInTheParent(rtt, rt);
            Vector2 max = FayvitUiUtility.GetMaxPositionInTheParent(rtt, rt);

            rt.anchorMin = new Vector2(min.x, 1 - max.y);
            rt.anchorMax = new Vector2(max.x, 1 - min.y);//0.5f * Vector2.one;
            rt.anchoredPosition = Vector2.zero;
            rt.offsetMin = Vector2.zero;//new Vector2(xStart * rtt.rect.width, yStart * rtt.rect.height);
            rt.offsetMax = Vector2.zero;

            DestroyImmediate(this);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
