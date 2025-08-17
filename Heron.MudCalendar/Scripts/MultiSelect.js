/**
 * Enables multi-cell selection in a calendar grid.
 * Users can click and drag to select multiple cells horizontally or vertically.
 * 
 * @param {number} CellsInDay - Number of rows per day in the calendar (used for vertical wrap-around).
 * @param {object} obj - The Blazor JSInterop object to invoke server-side callbacks.
 */
export function addMultiSelect(CellsInDay, obj) {
    let isMouseDown = false;           // Tracks if the mouse is currently pressed
    let startCell = null;              // The initial cell where the drag started
    let selectedCells = new Set();     // Contains "yyyy-mm-dd_row" of currently highlighted preview cells

    // --- Helper Functions ---

    // Convert date and row to a unique key string
    const toKey = (dateStr, row) => `${dateStr}_${row}`;

    // Get the DOM element for a given date and row
    const getEl = (dateStr, row) => document.querySelector(`[data-date="${dateStr}"][data-row="${row}"]`);

    // Parse a date string safely in UTC
    const parseUtcDate = (dateStr) => new Date(dateStr + "T00:00:00Z");

    // Convert a Date object to a "yyyy-mm-dd" string
    const dateToStr = (d) => d.toISOString().split('T')[0];

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
        if (!el) return false;
        if (el.dataset.selectable !== "true") return false; // Stop if cell is not selectable
        const key = toKey(dateStr, row);
        if (!selectedCells.has(key)) {
            el.classList.add("cell-selected");
            selectedCells.add(key);
        }
        return true;
    }

    /**
     * Marks cells vertically
     */
    function markVertical(start, end) {
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
     * Marks cells horizontally
     */
    function markHorizontal(start, end) {
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

    // --- Attach Event Handlers to Each Cell ---

    document.querySelectorAll("[data-row][data-date]").forEach(cell => {
        // Mouse down: start selection
        cell.addEventListener('mousedown', function (e) {
            if (cell.dataset.selectable !== "true") return;

            clearVisual();
            isMouseDown = true;
            startCell = cell;

            addCell(cell.dataset.date, parseInt(cell.dataset.row, 10));
        });

        // Mouse over: extend selection while dragging
        cell.addEventListener('mouseover', function (e) {
            if (!isMouseDown || !startCell) return;

            clearVisual();
            const currentCell = e.currentTarget;

            if (currentCell.dataset.date === startCell.dataset.date) {
                markVertical(startCell, currentCell);
            } else {
                markHorizontal(startCell, currentCell);
            }
        });

        // Mouse up: finalize selection
        cell.addEventListener('mouseup', function () {
            if (!isMouseDown) return;

            obj.invokeMethodAsync('CellsSelected', Array.from(selectedCells));
            clearVisual();

            isMouseDown = false;
            startCell = null;
        });
    });

    // --- Fallback for releasing mouse outside grid ---
    document.addEventListener('mouseup', function () {
        if (!isMouseDown) return;
        obj.invokeMethodAsync('CellsSelected', Array.from(selectedCells));
        clearVisual();
        isMouseDown = false;
        startCell = null;
    });
}