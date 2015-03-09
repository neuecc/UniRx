// for uGUI(from 4.6)
#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.UI
{
    [AddComponentMenu("Event/Observable Event Trigger")]
    public class ObservableEventTrigger : EventTrigger
    {
#region IDSelect

        Subject<BaseEventData> onDeselect;

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            if (onDeselect != null) onDeselect.OnNext(eventData);
        }

        public IObservable<BaseEventData> OnDeselectAsObservable()
        {
            return onDeselect ?? (onDeselect = new Subject<BaseEventData>());
        }

#endregion

#region IMoveHandler

        Subject<AxisEventData> onMove;

        public override void OnMove(AxisEventData eventData)
        {
            base.OnMove(eventData);
            if (onMove != null) onMove.OnNext(eventData);
        }

        public IObservable<AxisEventData> OnMoveAsObservable()
        {
            return onMove ?? (onMove = new Subject<AxisEventData>());
        }

#endregion

#region IPointerDownHandler

        Subject<PointerEventData> onPointerDown;

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (onPointerDown != null) onPointerDown.OnNext(eventData);
        }

        public IObservable<PointerEventData> OnPointerDownAsObservable()
        {
            return onPointerDown ?? (onPointerDown = new Subject<PointerEventData>());
        }

#endregion

#region IPointerEnterHandler

        Subject<PointerEventData> onPointerEnter;

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            if (onPointerEnter != null) onPointerEnter.OnNext(eventData);
        }

        public IObservable<PointerEventData> OnPointerEnterAsObservable()
        {
            return onPointerEnter ?? (onPointerEnter = new Subject<PointerEventData>());
        }

#endregion

#region IPointerExitHandler

        Subject<PointerEventData> onPointerExit;

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            if (onPointerExit != null) onPointerExit.OnNext(eventData);
        }

        public IObservable<PointerEventData> OnPointerExitAsObservable()
        {
            return onPointerExit ?? (onPointerExit = new Subject<PointerEventData>());
        }

#endregion

#region IPointerUpHandler

        Subject<PointerEventData> onPointerUp;

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            if (onPointerUp != null) onPointerUp.OnNext(eventData);
        }

        public IObservable<PointerEventData> OnPointerUpAsObservable()
        {
            return onPointerUp ?? (onPointerUp = new Subject<PointerEventData>());
        }

#endregion

#region ISelect

        Subject<BaseEventData> onSelect;

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            if (onSelect != null) onSelect.OnNext(eventData);
        }

        public IObservable<BaseEventData> OnSelectAsObservable()
        {
            return onSelect ?? (onSelect = new Subject<BaseEventData>());
        }

#endregion

#region IPointerClickHandler

        Subject<PointerEventData> onPointerClick;

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            if (onPointerClick != null) onPointerClick.OnNext(eventData);
        }

        public IObservable<PointerEventData> OnPointerClickAsObservable()
        {
            return onPointerClick ?? (onPointerClick = new Subject<PointerEventData>());
        }

#endregion

#region ISubmitHandler

        Subject<BaseEventData> onSubmit;

        public override void OnSubmit(BaseEventData eventData)
        {
            base.OnSubmit(eventData);
            if (onSubmit != null) onSubmit.OnNext(eventData);
        }

        public IObservable<BaseEventData> OnSubmitAsObservable()
        {
            return onSubmit ?? (onSubmit = new Subject<BaseEventData>());
        }

#endregion

#region IDragHandler

        Subject<PointerEventData> onDrag;

        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);
            if (onDrag != null) onDrag.OnNext(eventData);
        }

        public IObservable<PointerEventData> OnDragAsObservable()
        {
            return onDrag ?? (onDrag = new Subject<PointerEventData>());
        }

#endregion

#region IBeginDragHandler

        Subject<PointerEventData> onBeginDrag;

        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
            if (onBeginDrag != null) onBeginDrag.OnNext(eventData);
        }

        public IObservable<PointerEventData> OnBeginDragAsObservable()
        {
            return onBeginDrag ?? (onBeginDrag = new Subject<PointerEventData>());
        }

#endregion

#region IEndDragHandler

        Subject<PointerEventData> onEndDrag;

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            if (onEndDrag != null) onEndDrag.OnNext(eventData);
        }

        public IObservable<PointerEventData> OnEndDragAsObservable()
        {
            return onEndDrag ?? (onEndDrag = new Subject<PointerEventData>());
        }

#endregion

#region IDropHandler

        Subject<PointerEventData> onDrop;

        public override void OnDrop(PointerEventData eventData)
        {
            base.OnDrop(eventData);
            if (onDrop != null) onDrop.OnNext(eventData);
        }

        public IObservable<PointerEventData> OnDropAsObservable()
        {
            return onDrop ?? (onDrop = new Subject<PointerEventData>());
        }

#endregion

#region IUpdateSelectedHandler

        Subject<BaseEventData> onUpdateSelected;

        public override void OnUpdateSelected(BaseEventData eventData)
        {
            base.OnUpdateSelected(eventData);
            if (onUpdateSelected != null) onUpdateSelected.OnNext(eventData);
        }

        public IObservable<BaseEventData> OnUpdateSelectedAsObservable()
        {
            return onUpdateSelected ?? (onUpdateSelected = new Subject<BaseEventData>());
        }

#endregion

#region IInitializePotentialDragHandler

        Subject<PointerEventData> onInitializePotentialDrag;

        public override void OnInitializePotentialDrag(PointerEventData eventData)
        {
            base.OnInitializePotentialDrag(eventData);
            if (onInitializePotentialDrag != null) onInitializePotentialDrag.OnNext(eventData);
        }

        public IObservable<PointerEventData> OnInitializePotentialDragAsObservable()
        {
            return onInitializePotentialDrag ?? (onInitializePotentialDrag = new Subject<PointerEventData>());
        }

#endregion

#region ICancelHandler

        Subject<BaseEventData> onCancel;

        public override void OnCancel(BaseEventData eventData)
        {
            base.OnCancel(eventData);
            if (onCancel != null) onCancel.OnNext(eventData);
        }

        public IObservable<BaseEventData> OnCancelAsObservable()
        {
            return onCancel ?? (onCancel = new Subject<BaseEventData>());
        }

#endregion

#region IScrollHandler

        Subject<PointerEventData> onScroll;

        public override void OnScroll(PointerEventData eventData)
        {
            base.OnScroll(eventData);
            if (onScroll != null) onScroll.OnNext(eventData);
        }

        public IObservable<PointerEventData> OnScrollAsObservable()
        {
            return onScroll ?? (onScroll = new Subject<PointerEventData>());
        }

#endregion
    }
}

#endif