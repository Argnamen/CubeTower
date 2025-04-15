using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private CubeHierechy Hierechy;

    public delegate void DropEvent(CubeHierechy cube, Vector2 dropPosition);
    [SerializeField] private event DropEvent OnDrop;

    private Transform parentTransform;

    public void AddDropEvent(DropEvent action)
    {
        OnDrop += action;
    }

    public void RemoveDropEvent(DropEvent action)
    {
        OnDrop -= action;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Hierechy.StartPosition = transform.position;
        parentTransform = transform.parent;
        transform.SetParent(transform.root);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnDrop?.Invoke(Hierechy, eventData.position);
        if (transform.parent == parentTransform.root)
        {
            transform.SetParent(parentTransform);
            transform.SetSiblingIndex(Hierechy.ChildIndex);
        }
    }
}
