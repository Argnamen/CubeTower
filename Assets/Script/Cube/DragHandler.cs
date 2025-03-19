using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool OnTower = false;

    public CubeHierechy Hierechy;

    public delegate void DropEvent(GameObject cube, Vector2 dropPosition);
    public event DropEvent OnDrop;

    private Transform parentTransform;

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
        OnDrop?.Invoke(gameObject, eventData.position);
        if (transform.parent == parentTransform.root)
        {
            transform.SetParent(parentTransform);
        }
    }
}
