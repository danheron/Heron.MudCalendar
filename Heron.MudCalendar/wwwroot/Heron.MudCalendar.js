export function scroll(element, top) {
    element.scrollTo(0, top);
}

export function addDragHandler(id) {
    if (document.getElementsByClassName(id).length === 0) return;
    const dragZone = document.getElementsByClassName(id)[0];
    if (dragZone.children.length === 0) return;
    const dragItem = dragZone.children[0];
    const xPos = dragItem.offsetWidth / 2;
    document.getElementsByClassName(id)[0].addEventListener("dragstart", (e) => e.dataTransfer.setDragImage(dragItem, xPos, 18));
}
