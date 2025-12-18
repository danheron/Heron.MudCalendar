/**
 * Enables multi-cell selection in a specific calendar grid container.
 * Users can click and drag to select multiple cells horizontally or vertically.
 *
 * @param {number} CellsInDay - Number of rows per day in the calendar (used for vertical wrap-around).
 * @param {object} obj - The Blazor JSInterop object to invoke server-side callbacks.
 * @param {string} containerId - Unique id of the calendar container (id attribute).
 */
export function addMultiSelect(CellsInDay, containerId, obj) {
    let isMouseDown = false;
    let startCell = null;
    let selectedCells = new Set();

    // --- Helpers inside scope ---
    const toKey = (dateStr, row) => `${dateStr}_${row}`;

    // Get the DOM element for a given date and row (scoped to container)
    const getEl = (dateStr, row) => container.querySelector(`[data-date="${dateStr}"][data-row="${row}"]`);

    // Parse a date string safely in UTC
    const parseUtcDate = (dateStr) => new Date(dateStr + "T00:00:00Z");

    // Convert a Date object to a "yyyy-mm-dd" string
    const dateToStr = (d) => d.toISOString().split('T')[0];

    
    const container = document.getElementById(containerId);
    if (!container) return;

    // --- Attach mouse down to container ---
    container.addEventListener('mousedown', (e) => handleMouseDown(e));

    /**
     * Clears all visual highlights and resets the selection set
     */
    function clearVisual() {
        selectedCells.forEach(k => {
            const [d, r] = k.split('_');
            const el = getEl(d, parseInt(r, 10));
            if (el) el.classList.remove("cell-selected");
        });
        selectedCells.clear();
    }

    /**
     * Marks a single cell as selected, if selectable
     * @param {string} dateStr - The date of the cell
     * @param {number} row - The row index
     * @returns {boolean} true if cell was successfully added
     */
    function addCell(dateStr, row) {
        const el = getEl(dateStr, row);
        if (!(el && el.dataset && el.dataset.selectable === "true")) { return false; } // Stop if the cell is not selectable
        const key = toKey(dateStr, row);
        if (!selectedCells.has(key)) {
            el.classList.add("cell-selected");
            selectedCells.add(key);
        }
        return true;
    }

    /**
    * Marks cells vertically within the same day
    */
    function markVertical(start, end) {
        if (!(start && start.dataset && end && end.dataset)) return;

        const dateStr = start.dataset.date;
        const r1 = parseInt(start.dataset.row, 10);
        const r2 = parseInt(end.dataset.row, 10);
        const step = r1 <= r2 ? 1 : -1;

        let r = r1;
        while (true) {
            if (!addCell(dateStr, r)) break; // Stop if unselectable
            if (r === r2) break;
            r += step;
        }
    }

    /**
    * Marks cells horizontally across multiple days
    */
    function markHorizontal(start, end) {
        if (!(start && start.dataset && end && end.dataset)) return;

        const startDateStr = start.dataset.date;
        const endDateStr = end.dataset.date;
        const startRow = parseInt(start.dataset.row, 10);
        const endRow = parseInt(end.dataset.row, 10);

        let d = parseUtcDate(startDateStr);
        const target = parseUtcDate(endDateStr);
        const dayStep = (d <= target) ? 1 : -1;

        let row = startRow;

        while (true) {
            const dStr = dateToStr(d);
            if (!addCell(dStr, row)) break; // Stop if unselectable

            if (d.getTime() === target.getTime() && row === endRow) break;

            row += dayStep;
            if (row < 0 || row >= CellsInDay) {
                d.setUTCDate(d.getUTCDate() + dayStep);
                row = (dayStep > 0) ? 0 : (CellsInDay - 1);
            }
        }
    }

    /**
     * wires mouseover and mouse up events. Awaits boolean true when should wire-up
     * @param {any} active
     */
    function hookMouseEvents(active) {
        if (active) {
            container.addEventListener('mouseover', (e) => handleMouseOver(e));
            container.addEventListener('mouseup',() => handleMouseUp());
            document.addEventListener('mouseup', () => handleMouseUp());
        } else {
            container.removeEventListener('mouseover', (e) => handleMouseOver(e));
            container.removeEventListener('mouseup', () => handleMouseUp());
            document.removeEventListener('mouseup', () => handleMouseUp());
        }
    }

    /**
   * Handles mousedown.
   * @param {any} e EventArgs
   * @returns
   */
    function handleMouseDown(e) {

        const cell = e.target.closest("[data-row][data-date]");

        if (!(cell && cell.dataset && cell.dataset.selectable === "true")) return;

        clearVisual();
        isMouseDown = true;
        startCell = cell;

        addCell(cell.dataset.date, parseInt(cell.dataset.row, 10));
        hookMouseEvents(true);
    }

    /**
     * Handles Mouseover.
     * @param {any} e EventArgs
     * @returns
     */
    function handleMouseOver(e) {
        if (!isMouseDown || !startCell) return;

        const currentCell = e.target.closest("[data-row][data-date]");
        if (!(currentCell && currentCell.dataset && currentCell.dataset.selectable === "true")) return;

        clearVisual();
        if (currentCell.dataset.date === startCell.dataset.date) {
            markVertical(startCell, currentCell);
        } else {
            markHorizontal(startCell, currentCell);
        }
    }

    /**
     * Handles MouseUp events
     */
    function handleMouseUp() {
        const cells = Array.from(selectedCells);
        clearVisual();
        hookMouseEvents(false);

        const flag = isMouseDown && startCell && cells.length !== 0;
        isMouseDown = false;
        startCell = null;

        if (flag) {
            obj.invokeMethodAsync('CellsSelected', cells);
        }
    }
        
}