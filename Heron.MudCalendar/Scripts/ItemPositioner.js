let _moreText = "";

export function positionMonthItems(moreText) {
    if (moreText) _moreText = moreText;
    
    document.querySelectorAll(".mud-cal-month-row-holder").forEach(function(container) {
        // Remove any existing messages
        container.querySelectorAll(".mud-cal-overflow-message").forEach(function (message) {
            message.remove();
        });
        
        const positions = [];
        const overlaps = [];
        container.querySelectorAll(".mud-cal-drop-item").forEach(function(item) {
            // Remove any overflow messages
            item.classList.remove("mud-cal-overflow-hidden");
            
            // Create new position object
            const position = new ItemPosition();
            position.Item = item;
            position.Top = 36;
            position.Height = item.clientHeight;
            position.Left = item.offsetLeft;
            position.Width = item.clientWidth;
            
            // Remove overlaps that are not relevant
            for (let i = 0; i < overlaps.length; i++)
            {
                if (overlaps[i].Right <= position.Left + 1)
                {
                    overlaps.splice(i, 1);
                    i--;
                }
            }
            positions.push(position);
            
            // Calculate the position
            let maxBottom = 0;
            for (let i = 0; i < overlaps.length; i++) {
                if (overlaps[i].Bottom > maxBottom) maxBottom = overlaps[i].Bottom;
                if (overlaps[i].Top < position.Top + position.Height) {
                    position.Top = maxBottom + 1;   
                }
            }
            
            // Add to overlaps
            overlaps.push(position);
        });
        
        // Find overflows
        const lefts = [];
        positions.forEach((position) => {
            if (!lefts.includes(position.Left)) lefts.push(position.Left);
        });
        lefts.forEach((left) => {
            hideOverflows(container, positions.filter(p => p.Left === left), left, moreText);
        });
        
        // Update positions
        positions.forEach((position) => {
            if (position.Hidden)
            {
                position.Item.classList.add("mud-cal-overflow-hidden");
            }
            else
            {
                position.Item.style.top = position.Top + "px";   
            }
        });
    });
}

function hideOverflows(container, positions, position, moreText) {
    // Get the lowest position
    let maxBottom = 0;
    positions.forEach((position) => {
        if (position.Bottom > maxBottom) maxBottom = position.Bottom;
    });
    
    // No need to continue if no overflow
    if (maxBottom <= container.clientHeight) return;
    
    // Create overflow message
    const div = document.createElement("div");
    div.className = "mud-cal-overflow-message";
    div.textContent = "+";
    div.style.left = position + "px";
    container.appendChild(div);
    
    // Count and hide the overflowed elements
    let hiddenElements = 0;
    positions.forEach((position) => {
        if (position.Bottom > container.clientHeight - div.clientHeight) {
            hiddenElements++;
            position.Hidden = true;
        }
    });
    
    // Update overflow message with count of hidden elements
    div.textContent = "+" + hiddenElements + " " + _moreText;
    div.style.top = (container.clientHeight - div.clientHeight) + "px";
}

class ItemPosition {
    
    Top = 0;
    Height = 0;
    Left = 0;
    Width = 0;
    Item = null;
    Hidden = false;
    
    get Bottom() {
        return this.Top + this.Height;
    }
    
    get Right() {
        return this.Left + this.Width;
    }
}