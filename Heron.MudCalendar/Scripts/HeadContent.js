export function getHeadContent() {
    return document.head.innerHTML;
}

export function addLink(href, rel, obj) {
    const link = document.createElement("link");
    link.href = href;
    link.rel = rel;
    
    if (obj) {
        link.onload = () => {
            obj.invokeMethodAsync("LinkLoaded")
        }
    }

    document.head.appendChild(link);
}