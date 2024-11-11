export function hideOverflows(className, moreText) {
    document.querySelectorAll("." + className).forEach(function (container) {
        // Remove any existing messages
        container.querySelectorAll(".mud-cal-overflow-message").forEach(function (message) {
            message.remove();
        });
        container.querySelectorAll(".mud-cal-overflow-hidden").forEach(function (hidden) {
            hidden.classList.remove("mud-cal-overflow-hidden");
        });

        // Get total height of all children
        let totalHeight = 0;
        for (let i = 0; i < container.children.length; i++) {
            totalHeight += container.children[i].clientHeight;
        }

        // No need to continue if no overflow
        if (totalHeight <= container.clientHeight) return;

        // Create overflow message
        const div = document.createElement("div");
        div.className = "mud-cal-overflow-message";
        div.textContent = "+";
        container.appendChild(div);
        
        // Count and hide the overflowed elements
        totalHeight = 0;
        let hiddenElements = 0;
        for (let i = 0; i < container.children.length; i++) {
            const child = container.children[i];
            if (child.className === "mud-cal-overflow-message") continue;
            totalHeight += child.clientHeight;
            if (totalHeight > container.clientHeight - div.clientHeight) {
                hiddenElements++;
                child.classList.add("mud-cal-overflow-hidden");
            }
        }
        
        // Update overflow message with count of hidden elements
        div.textContent = "+ " + hiddenElements + " " + moreText;
    });
}
