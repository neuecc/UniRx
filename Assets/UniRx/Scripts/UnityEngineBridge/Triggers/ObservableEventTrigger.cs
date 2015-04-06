// for uGUI(from 4.6)
#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)

using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Triggers
{
    [DisallowMultipleComponent]
    public class ObservableEventTrigger : ObservableTriggerBase, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IBeginDragHandler, IInitializePotentialDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IScrollHandler, IUpdateSelectedHandler, ISelectHandler, IDeselectHandler, IMoveHandler, ISubmitHandler, ICancelHandler
    {
        #region IDSelect

        Subject<BaseEventData> onDeselect;

        public void OnDeselect(BaseEventData eventData)
        {
            if (onDeselect != null) onDeselect.OnNext(eventData);
        }

        public IObservable<BaseEventData> OnDeselectAsObservable()
        {
            return onDeselect ?? (onDeselect = new Subject<BaseEventData>());
        }

        #endregion

        #region IMoveHandler

        Subject<AxisEventData> onMove;

        public void OnMove(AxisEventData eventData)
        {
            if (onMove != null) onMove.OnNext(eventData);
        }

        public IObservable<AxisEventData> OnMoveAsObservable()
        {
            return onMove ?? (onMove = new Subject<AxisEventData>());
        }

        #endregion

        #region IPointerDownHandler

        Subject<PointerEventData> onPointerDown;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (onPointerDown != null) onPointerDown.OnNext(eventData);
        }

        public IObservable<PointerEventData> OnPointerDownAsObservable()
        {
            return onPointerDown ?? (onPointerDown = new Subject<PointerEventData>());
        }

        #endregion

        #region IPointerEnterHandler

        Subject<PointerEventData> onPointerEnter;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (onPointerEnter != null) onPointerEnter.OnNext(eventData);
        }

        public IObservable<PointerEventData> OnPointerEnterAsObservable()
        {
            return onPointerEnter ?? (onPointerEnter = new Subject<PointerEventData>());
        }

        #endregion

        #region IPointerExitHandler

        Subject<PointerEventData> onPointerExit;

        public void OnPointerExit(PointerEventData eventData)
        {
            if (onPointerExit != null) onPointerExit.OnNext(eventData);
        }

        public IObservable<PointerEventData> OnPointerExitAsObservable()
        {
            return onPointerExit ?? (onPointerExit = new Subject<PointerEventData>());
        }

        #endregion

        #region IPointerUpHandler

        Subject<PointerEventData> onPointerUp;

        public void OnPointerUp(PointerEventData eventData)
        {
            if (onPointerUp != null) onPointerUp.OnNext(eventData);
        }

        public IObservable<PointerEventData> OnPointerUpAsObservable()
        {
            return onPointerUp ?? (onPointerUp = new Subject<PointerEventData>());
        }

        #endregion

        #region ISelect

        Subject<BaseEventData> onSelect;

        public void OnSelect(BaseEventData eventData)
        {
            if (onSelect != null) onSelect.OnNext(eventData);
        }

        public IObservable<BaseEventData> OnSelectAsObservable()
        {
            return onSelect ?? (onSelect = new Subject<BaseEventData>());
        }

        #endregion

        #region IPointerClickHandler

        Subject<PointerEventData> onPointerClick;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (onPointerClick != null) onPointerClick.OnNext(eventData);
        }

        public IObservable<PointerEventData> OnPointerClickAsObservable()
        {
            return onPointerClick ?? (onPointerClick = new Subject<PointerEventData>());
        }

        #endregion

        #region ISubmitHandler

        Subject<BaseEventData> onSubmit;

        public void OnSubmit(BaseEventData eventData)
        {
            if (onSubmit != null) onSubmit.OnNext(eventData);
        }

        public IObservable<BaseEventData> OnSubmitAsObservable()
        {
            return onSubmit ?? (onSubmit = new Subject<BaseEventData>());
        }

        #endregion

        #region IDragHandler

        Subject<PointerEventData> onDrag;

        public void OnDrag(PointerEventData eventData)
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

        public void OnBeginDrag(PointerEventData eventData)
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

        public void OnEndDrag(PointerEventData eventData)
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

        public void OnDrop(PointerEventData eventData)
        {
            if (onDrop != null) onDrop.OnNext(eventData);
        }

        public IObservable<PointerEventData> OnDropAsObservable()
        {
            return onDrop ?? (onDrop = new Subject<PointerEventData>());
        }

        #endregion

        #region IUpdateSelectedHandler

        Subject<BaseEventData> onUpdateSelected;

        public void OnUpdateSelected(BaseEventData eventData)
        {
            if (onUpdateSelected != null) onUpdateSelected.OnNext(eventData);
        }

        public IObservable<BaseEventData> OnUpdateSelectedAsObservable()
        {
            return onUpdateSelected ?? (onUpdateSelected = new Subject<BaseEventData>());
        }

        #endregion

        #region IInitializePotentialDragHandler

        Subject<PointerEventData> onInitializePotentialDrag;

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (onInitializePotentialDrag != null) onInitializePotentialDrag.OnNext(eventData);
        }

        public IObservable<PointerEventData> OnInitializePotentialDragAsObservable()
        {
            return onInitializePotentialDrag ?? (onInitializePotentialDrag = new Subject<PointerEventData>());
        }

        #endregion

        #region ICancelHandler

        Subject<BaseEventData> onCancel;

        public void OnCancel(BaseEventData eventData)
        {
            if (onCancel != null) onCancel.OnNext(eventData);
        }

        public IObservable<BaseEventData> OnCancelAsObservable()
        {
            return onCancel ?? (onCancel = new Subject<BaseEventData>());
        }

        #endregion

        #region IScrollHandler

        Subject<PointerEventData> onScroll;

        public void OnScroll(PointerEventData eventData)
        {
            if (onScroll != null) onScroll.OnNext(eventData);
        }

        public IObservable<PointerEventData> OnScrollAsObservable()
        {
            return onScroll ?? (onScroll = new Subject<PointerEventData>());
        }

        #endregion

        protected override void RaiseOnCompletedOnDestroy()
        {
            if (onDeselect != null)
            {
                onDeselect.OnCompleted();
            }
            if (onMove != null)
            {
                onMove.OnCompleted();
            }
            if (onPointerDown != null)
            {
                onPointerDown.OnCompleted();
            }
            if (onPointerEnter != null)
            {
                onPointerEnter.OnCompleted();
            }
            if (onPointerExit != null)
            {
                onPointerExit.OnCompleted();
            }
            if (onPointerUp != null)
            {
                onPointerUp.OnCompleted();
            }
            if (onSelect != null)
            {
                onSelect.OnCompleted();
            }
            if (onPointerClick != null)
            {
                onPointerClick.OnCompleted();
            }
            if (onSubmit != null)
            {
                onSubmit.OnCompleted();
            }
            if (onDrag != null)
            {
                onDrag.OnCompleted();
            }
            if (onBeginDrag != null)
            {
                onBeginDrag.OnCompleted();
            }
            if (onEndDrag != null)
            {
                onEndDrag.OnCompleted();
            }
            if (onDrop != null)
            {
                onDrop.OnCompleted();
            }
            if (onUpdateSelected != null)
            {
                onUpdateSelected.OnCompleted();
            }
            if (onInitializePotentialDrag != null)
            {
                onInitializePotentialDrag.OnCompleted();
            }
            if (onCancel != null)
            {
                onCancel.OnCompleted();
            }
            if (onScroll != null)
            {
                onScroll.OnCompleted();
            }
        }
    }
}

#endif