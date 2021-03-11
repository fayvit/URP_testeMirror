using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindSizeInTheCanvas : MonoBehaviour
{
    RectTransform rt;
    RectTransform rtt;
    // Start is called before the first frame update
    void Start()
    {
        rt = GetComponent<RectTransform>();
        rtt = rt.parent.GetComponent<RectTransform>();
        Debug.Log(rt.name + " : " + rtt.name);

        Rect doRt = rt.rect;
        Rect doRtt = rtt.rect;
        Vector2 minPos = GetMinVector2Position(rtt, rt);
        Vector2 maxPos = GetMaxVector2Position(rtt, rt);
        Debug.Log("minPosition: (" +minPos.x+";"+minPos.y + ") extends: (" + maxPos.x+"; "+maxPos.y+")");
        //Debug.Log(doRt + " : " + doRtt);
        //Bounds b = RectTransformUtility.CalculateRelativeRectTransformBounds(rtt, rt);

        //Debug.Log("OFFset: "+rt.offsetMax + " : " + rt.offsetMin);
        //Debug.Log("max x: "+rt.offsetMax.x/doRtt.width);
        //Debug.Log("max y: " + rt.offsetMax.y / doRtt.height);
        //Debug.Log("min x: " + rt.offsetMin.x / doRtt.width);
        //Debug.Log("min y: " + rt.offsetMin.y / doRtt.height);
        ////Debug.Log("rt sizedelta: " + rt.sizeDelta + " rtt: " + rtt.sizeDelta);
        ////Debug.Log("RT anchored: " + rt.anchoredPosition + " : " + rt.anchorMax.x + " : " + rt.anchorMin.x + " : " + rt.anchorMax.y + " : " + rt.anchorMin.y);
        ////Debug.Log("RTT anchored: " + rtt.anchoredPosition + " : " + rtt.anchorMax + " : " + rtt.anchorMin);

        //Debug.Log((rt.anchorMax.x-rt.anchorMin.x)+" : "+(rt.anchorMax.y-rt.anchorMin.y));

        //Debug.Log((rt.anchorMin.x+ rt.offsetMin.x / doRtt.width) + " : " 
        //    +(1- (rt.anchorMax.y+ rt.offsetMax.y / doRtt.height)));

        //Debug.Log(b + " : " + (rt.rect.x + 0.5 * doRtt.width) / rtt.rect.width + " : " + (rt.rect.y + 0.5 * doRtt.height) / rtt.rect.height);
        //Debug.Log(b + " : " + b.extents);
        //Debug.Log((b.center.x - 0.5f * b.extents.x + 0.5f * doRt.width) / doRtt.width);
        //Debug.Log((b.center.y - 0.5f * b.extents.y + 0.5f * doRt.height) / doRtt.height);


        Destroy(this);
    }

    Vector2 GetExtends(Rect pai, Rect filho)
    {
        return new Vector2(filho.width / pai.width, filho.height / pai.height);
    }

    Vector2 GetMaxVector2Position(RectTransform pai, RectTransform filho)
    {
        Vector2 extends = GetExtends(pai.rect, filho.rect);
        Vector2 min = GetMinVector2Position(pai, filho);
        return new Vector2(extends.x + min.x, extends.y + min.y);
    }

    Vector2 GetMinVector2Position(RectTransform pai,RectTransform filho)
    {
        return new Vector2(filho.anchorMin.x + filho.offsetMin.x / pai.rect.width, 
            1 - (filho.anchorMax.y + filho.offsetMax.y / pai.rect.height));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
