export function scroll(element, top) {
    element.scrollTo(0, top);
}

export function getHeadContent() {
    return document.head.innerHTML;
}

export function addLink(href, rel) {
    const link = document.createElement("link");
    link.href = href;
    link.rel = rel;
    
    document.head.appendChild(link);
}