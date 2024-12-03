export function scroll(element, top) {
    element.scrollTo(0, top);
}

export function getHeadContent() {
    return document.head.innerHTML;
}

export function addLink(href, rel, updateOnLoad) {
    const link = document.createElement("link");
    link.href = href;
    link.rel = rel;
    
    if (updateOnLoad) {
        link.onload = function() {
            positionMonthItems("Test")
        };
    }
    
    document.head.appendChild(link);
}