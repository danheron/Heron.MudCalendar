export class MultiSelect {
    
    _obj;
    _container;
    _cellsInDay;
    _lastItem;
    _startDate;
    _startRow;
    _pointers;
    _scrolling;
    _lastScrollY;
    _frame;
    
    constructor(cellsInDay, containerId, obj) {
        this._obj = obj;
        this._container = document.getElementById(containerId).querySelector(".mud-cal-selectable-container");
        this._cellsInDay = cellsInDay;
        this._pointers = new Map();
        this._scrolling = false;
        this._frame = null;
        
        this._container.addEventListener("pointerdown", this.onPointerDown);
    }
    
    onPointerDown = (e) => {
        const item = this.getCellFromEvent(e);
        if (!item) return;
        
        this._pointers.set(e.pointerId, { x: e.clientX, y: e.clientY });
        
        if (this._pointers.size >= 2) {
            // If 2 or more fingers are being used, then scroll instead of selecting cells
            this._scrolling = true;
            this._lastScrollY = this.getPositionY();
        } else {
            this._startDate = new Date(item.dataset.date + "T00:00:00Z");
            this._startRow = parseInt(item.dataset.row);
            this._lastItem = null;
            item.classList.add("cell-selected");
        }

        this._container.addEventListener("pointermove", this.onPointerMove);
        this._container.addEventListener("pointerup", this.onPointerUp);
        this._container.addEventListener("pointerleave", this.onPointerLeave);
        this._container.addEventListener("pointercancel", this.onPointerLeave);
    }
    
    onPointerUp = (e) => {
        this.removeEventListeners();
        
        if (this._pointers.size < 2) {
            const item = this.getCellFromEvent(e);
            if (!item) return;

            const cells = this.getSelectedCells(this._startDate, this._startRow, new Date(item.dataset.date + "T00:00:00Z"), parseInt(item.dataset.row));
            const keys = cells.map(el => el.dataset.date + "_" + el.dataset.row);

            this._obj.invokeMethodAsync("CellsSelected", keys);
        }

        this._container.querySelectorAll(".cell-selected").forEach(el => el.classList.remove("cell-selected"));
        this._pointers.clear();
        this._scrolling = false;
    }
    
    onPointerLeave = () => {
        this.removeEventListeners();
        
        this._container.querySelectorAll(".cell-selected").forEach(el => el.classList.remove("cell-selected"));
        this._pointers.clear();
        this._scrolling = false;
    }
    
    onPointerMove = (e) => {
        if (this._pointers.size >= 2) {
            const pointer = this._pointers.get(e.pointerId);
            pointer.y = e.clientY;
            this.scrollY();
        } else {
            const item = this.getCellFromEvent(e);
            if (!item || item === this._lastItem) return;
            this._lastItem = item;

            this._container.querySelectorAll(".cell-selected").forEach(el => el.classList.remove("cell-selected"));

            const currentDate = new Date(item.dataset.date + "T00:00:00Z");
            const cells = this.getSelectedCells(this._startDate, this._startRow, currentDate, parseInt(item.dataset.row));
            cells.push(item)
            cells.forEach(el => el.classList.add("cell-selected"));
        }
    }

    getCellFromEvent = (e) => {
        const x = e.clientX;
        const y = e.clientY;
        if (typeof x !== "number" || typeof y !== "number") return null;
        const element = document.elementFromPoint(x, y);
        return element ? element.closest(".mud-cal-selectable") : null;
    }
    
    getSelectedCells(startDate, startRow, endDate, endRow) {
        let firstDate = startDate;
        let lastDate = endDate;
        let firstRow = startRow;
        let lastRow = endRow;
        if (endDate < startDate || (endDate.getTime() === startDate.getTime() && endRow < startRow)) {
            lastDate = startDate;
            firstDate = endDate;
            lastRow = startRow;
            firstRow = endRow;
        }
        
        const cells = [];
        let date = new Date(firstDate);
        while (date <= lastDate) {
            const dateString = date.toISOString().split('T')[0];
            let minRow = 0;
            let maxRow = this._cellsInDay - 1;
            if (date.getTime() === firstDate.getTime()) {
                minRow = firstRow;
            }
            if (date.getTime() === lastDate.getTime()) {
                maxRow = lastRow;
            }
            
            for (let row = minRow; row <= maxRow; row++) {
                const element = this._container.querySelector(`[data-date="${dateString}"][data-row="${row}"]`);
                if(element) cells.push(element);
            }
            
            date.setDate(date.getDate() + 1);
        }
        
        return cells;
    }
    
    getPositionY() {
        // Get an average Y position of all pointers
        let sumY = 0;
        let count = 0;
        for (const pointer of this._pointers.values()) {
            sumY += pointer.y;
            count++;
        }
        return sumY / count;
    }
    
    scrollY()
    {
        if (this._frame) return;
        this._frame = requestAnimationFrame(() =>
        {
            this._frame = null;
            if (!this._scrolling) return;
            const scrollY = this.getPositionY();
            const delta = this._lastScrollY - scrollY;
            this._container.scrollBy(0, delta);
            this._lastScrollY = scrollY;
            this._container.scrollTop += delta;
        })
    }
    
    removeEventListeners() {
        this._container.removeEventListener("pointermove", this.onPointerMove);
        this._container.removeEventListener("pointerup", this.onPointerUp);
        this._container.removeEventListener("pointerleave", this.onPointerLeave);
        this._container.removeEventListener("pointercancel", this.onPointerLeave);
    }
    
    dispose() {
        if (this._container) {
            this.removeEventListeners();
            this._container.removeEventListener("pointerdown", this.onPointerDown);
        }
        
        this._obj = null;
        this._container = null;
        this._lastItem = null;
        this._startDate = null;
        this._pointers = null;
        this._frame = null;
    }
}

export function newMultiSelect(cellsInDay, containerId, obj) {
    return new MultiSelect(cellsInDay, containerId, obj);
}
