using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public bool IsSimple => No < 6;
    public int No, Col, Row, CombinationVersion;

    public float Width => RectTransform.sizeDelta.x;
    public float Height => RectTransform.sizeDelta.y;

    public RawImage Sprite;
    public RectTransform RectTransform;
    GamePlay _parent;

    void Awake()
    {
        if (transform.parent.TryGetComponent(out GamePlay board))
            _parent = board;
    }

    public void UpdatePosition(bool animate = false, bool isNew = false)
    {
        var tempPos = RectTransform.anchoredPosition;
        var tempRect = RectTransform.sizeDelta;
        tempPos.x = Col * tempRect.x + (Width * .5f);
        RectTransform.anchoredPosition = tempPos;
        var ty = Row * tempRect.y + (isNew ? 0 : (Height * .5f));
        if (!animate)
        {
            tempPos.y = ty;
            RectTransform.anchoredPosition = tempPos;
        }
        else
            core.Coroutine.Start(this.gameObject, () => Move(ty), _parent.WorkJob);
    }

    public void ClickTile() => _parent.ClickTile(Col, Row);

    IEnumerable Move(float ty)
    {
        var g = 9.81f;
        var m = 80;
        var time = 0f;
        var sy = RectTransform.anchoredPosition.y;
        var tempPos = RectTransform.anchoredPosition;
        while (tempPos.y < ty - (Height * .5f))
        {
            time += Time.deltaTime;
            var h = g * m * time * time;
            tempPos.y = sy + h;
            RectTransform.anchoredPosition = tempPos;
            if (tempPos.y >= ty)
            {
                tempPos.y = ty;
                RectTransform.anchoredPosition = tempPos;
                break;
            }
            yield return null;
        }
        tempPos.y = ty;
        RectTransform.anchoredPosition = tempPos;
    }
} 