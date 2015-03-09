using UnityEngine.EventSystems;

namespace UniRx.UI
{
    public abstract class ObservableDragAndDropBehaviour : ObservableUIBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
    {
        #region IDragHandler

        Subject<PointerEventData> onDrag;

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (onDrag != null) onDrag.OnNext(eventData);
        }

        public IObservable<PointerEventData> OnDragAsObservable()
        {
            return onDrag ?? (onDrag = new Subject<PointerEventData>());
        }

        #endregion

        #region IBeginDragHandler

        Subject<PointerEventData> onBeginDrag;

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (onBeginDrag != null) onBeginDrag.OnNext(eventData);
        }

        public IObservable<PointerEventData> OnBeginDragAsObservable()
        {
            return onBeginDrag ?? (onBeginDrag = new Subject<PointerEventData>());
        }

        #endregion

        #region IEndDragHandler

        Subject<PointerEventData> onEndDrag;

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (onEndDrag != null) onEndDrag.OnNext(eventData);
        }

        public IObservable<PointerEventData> OnEndDragAsObservable()
        {
            return onEndDrag ?? (onEndDrag = new Subject<PointerEventData>());
        }

        #endregion

        #region IDropHandler

        Subject<PointerEventData> onDrop;

        public virtual void OnDrop(PointerEventData eventData)
        {
            if (onDrop != null) onDrop.OnNext(eventData);
        }

        public IObservable<PointerEventData> OnDropAsObservable()
        {
            return onDrop ?? (onDrop = new Subject<PointerEventData>());
        }

        #endregion
    }
}