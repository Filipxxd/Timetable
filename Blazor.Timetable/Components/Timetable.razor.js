﻿const timetableSelector = '.timetable-grid';
const slotSelector = '.timetable-body-cell';
const eventSelector = '.movable-event';

const eventArgument = 'data-event-id';
const slotArgument = 'data-slot-id';
const originalSlotArgument = 'data-original-slot-id';

export const dragDrop = {
    init: function (objRef) {
        interact(eventSelector).draggable({
            inertia: false,
            autoScroll: {
                container: document.querySelector(timetableSelector),
                margin: 50,
                speed: 300
            },

            ////modifiers: [
            ////    interact.modifiers.restrict({
            ////        restriction: timetableSelector,
            ////        endOnly: true,
            ////        elementRect: { top: 0, left: 0, bottom: 1, right: 1 }
            ////    })
            ////],

            listeners: {
                start(event) {
                    const target = event.target;
                    target.style.zIndex = '1000';
                    target.style.cursor = 'move';
                    const originalSlot = target.closest(slotSelector);

                    if (originalSlot) {
                        target.setAttribute(originalSlotArgument, originalSlot.getAttribute(slotArgument));
                    }
                },

                move(event) {
                    const target = event.target;
                    const x = (parseFloat(target.getAttribute('data-x')) || 0) + event.dx;
                    const y = (parseFloat(target.getAttribute('data-y')) || 0) + event.dy;

                    target.style.transform = 'translate(' + x + 'px, ' + y + 'px)';
                    target.setAttribute('data-x', x);
                    target.setAttribute('data-y', y);
                },

                async end(event) {
                    event.preventDefault();
                    const target = event.target;
                    const closestSlot = findClosestSlot(target);
                    target.style.zIndex = '';
                    target.style.cursor = 'pointer';

                    if (!closestSlot) {
                        resetPosition(target);
                        return;
                    }

                    const eventId = target.getAttribute(eventArgument);
                    const targetSlotId = closestSlot.getAttribute(slotArgument);
                    const originalSlotId = target.getAttribute(originalSlotArgument);

                    if (targetSlotId === originalSlotId) {
                        resetPosition(target);
                        return;
                    }

                    await objRef.invokeMethodAsync('MoveEvent', eventId, targetSlotId)
                        .catch(error => {
                            console.error("Error moving event: ", error);
                        });
                }
            }
        });
    }
};

function resetPosition(element) {
    element.style.transform = '';
    element.setAttribute('data-x', 0);
    element.setAttribute('data-y', 0);
}

function findClosestSlot(draggedElement) {
    const slots = document.querySelectorAll(slotSelector);
    const draggedRect = draggedElement.getBoundingClientRect();
    const draggedTopLeftX = draggedRect.left;
    const draggedTopLeftY = draggedRect.top;
    let closestSlot = null;
    let closestDistance = Infinity;

    slots.forEach(slot => {
        const slotRect = slot.getBoundingClientRect();
        const slotTopLeftX = slotRect.left;
        const slotTopLeftY = slotRect.top;
        const distance = Math.sqrt(
            Math.pow(draggedTopLeftX - slotTopLeftX, 2) +
            Math.pow(draggedTopLeftY - slotTopLeftY, 2)
        );

        if (distance < closestDistance) {
            closestDistance = distance;
            closestSlot = slot;
        }
    });

    return closestSlot;
}