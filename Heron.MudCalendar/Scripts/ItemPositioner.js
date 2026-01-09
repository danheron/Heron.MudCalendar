let _moreText = "";
let _obj = null;
let _isRtl = false;

export function positionMonthItems(element, moreText, fixedHeight, obj) {
    if (moreText) _moreText = moreText;
    if (obj) _obj = obj;
    _isRtl = document.querySelector('.mud-application-layout-rtl') !== null;
    
    element.querySelectorAll(".mud-cal-month-row-holder").forEach(function(container) {
        // Remove any existing messages
        container.querySelectorAll(".mud-cal-overflow-message").forEach(function (message) {
            message.remove();
        });

        // Find the height of the header part
        const datePositions = new Map();
        const totalWidth = container.clientWidth;
        const cellCount = container.querySelectorAll(".mud-cal-month-cell").length;
        let headerHeight = 0;
        let index = 0;
        container.querySelectorAll(".mud-cal-month-cell").forEach(function(cell) {
            let height = 0;
            cell.querySelectorAll(".mud-cal-month-cell-header").forEach(function(item) {
                height += item.clientHeight;
            });
            if (height > headerHeight) headerHeight = height;
            
            datePositions.set(Math.round(totalWidth / cellCount * index), cell.attributes["Identifier"].textContent);
            index++;
        });
        
        const positions = [];
        const overlaps = [];
        container.querySelectorAll(".mud-cal-drop-item").forEach(function(item) {
            // Remove any overflow messages
            item.classList.remove("mud-cal-overflow-hidden");

            // Reset visibility on recalculation
            item.classList.remove("mud-cal-positioned"); 

            // Create the new position object
            const position = new ItemPosition();
            position.Item = item;
            position.Top = headerHeight;
            position.Height = item.clientHeight;
            position.Start = offsetStart(item);
            position.Width = item.clientWidth;
            
            // Remove overlaps that are not relevant
            for (let i = 0; i < overlaps.length; i++)
            {
                if (overlaps[i].End <= position.Start + 1)
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
        
        if (fixedHeight) {
            // Find overflows
            const starts = [];
            positions.forEach((position) => {
                if (!starts.includes(position.Start)) starts.push(position.Start);
            });
            starts.forEach((start) => {
                hideOverflows(container, positions.filter(p => p.Start === start), start, datePositions);
            });
        }
        else
        {
            // Calculate the height of the row
            let rowMaxBottom = headerHeight;
            positions.forEach((position) => {
                if (position.Bottom > rowMaxBottom) rowMaxBottom = position.Bottom;
            })
            container.style.height = rowMaxBottom + "px";
        }
        
        // Update positions
        positions.forEach((position) => {
            if (position.Hidden)
            {
                position.Item.classList.add("mud-cal-overflow-hidden");
            }
            else
            {
                position.Item.style.top = position.Top + "px";
                position.Item.classList.add("mud-cal-positioned"); // Fade in visible items
            }
        });
    });
}

function hideOverflows(container, positions, position, datePositions) {
    // Get the lowest position
    let maxBottom = 0;
    positions.forEach((position) => {
        if (position.Bottom > maxBottom) maxBottom = position.Bottom;
    });
    
    // No need to continue if no overflow
    if (maxBottom <= container.clientHeight) return;
    
    // Create the overflow message
    const div = document.createElement("div");
    div.className = "mud-cal-overflow-message";
    div.textContent = "+";
    div.style.insetInlineStart = (position / container.clientWidth) * 100 + "%";
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
    
    // Add click event if required
    if (_obj) {
        div.classList.add("mud-cal-clickable");
        div.addEventListener("click", async () => {
            const date = datePositions.get(Math.round(position));
            if (date) {
                await _obj.invokeMethodAsync("MoreClicked", date);
            }
        });
    }
}

function offsetStart(element) {
    return _isRtl ? element.offsetParent.offsetWidth - element.offsetLeft - element.offsetWidth : element.offsetLeft;
}

class ItemPosition {
    
    Top = 0;
    Height = 0;
    Start = 0;
    Width = 0;
    Item = null;
    Hidden = false;
    
    get Bottom() {
        return this.Top + this.Height;
    }
    
    get End() {
        return this.Start + this.Width;
    }
}
