export class Resizer {
    
    _obj;
    _handle;
    _container;
    _cellCount;
    _intervalSize;
    _item;
    _startSize;
    _startPos;
    _resizeX;
    _position;
    
    constructor(handleId, containerClass, cellCount, resizeX, obj) {
        this._obj = obj;
        this._resizeX = resizeX;
        this._handle = document.getElementById(handleId);
        this._container = document.getElementsByClassName(containerClass)[0];
        this._cellCount = cellCount;
        this._item = this._handle.parentElement;
        
        this._handle.addEventListener("mousedown", this.onMouseDown);
    }
    
    onMouseDown = (e) => {
        e.stopPropagation();
        
        if (this._resizeX)
        {
            this._intervalSize = this._container.clientWidth / this._cellCount;
            this._startSize = this._item.clientWidth;
            this._startPos = e.clientX;
        }
        else
        {
            this._intervalSize = this._container.clientHeight / this._cellCount;
            this._startSize = this._item.clientHeight;
            this._startPos = e.clientY;   
        }
        
        document.addEventListener("mouseup", this.onMouseUp);
        document.addEventListener("mousemove", this.onMouseMove);
    }
    
    onMouseUp = async () => {
        document.removeEventListener("mouseup", this.onMouseUp);
        document.removeEventListener("mousemove", this.onMouseMove);
        
        const newSize = this.update();
        
        await this._obj.invokeMethodAsync("ResizeFinished", newSize);
    }
    
    onMouseMove = (e) => {
        this._position = this._resizeX ? e.clientX : e.clientY;
        
        this.update();
    }
    
    update() {
        const movement = this._position - this._startPos;
        let size = this._startSize + movement;
        
        // Check that item is not too big
        if (this._resizeX)
        {
            if (this._item.offsetLeft + size > this._container.clientWidth) size = this._container.clientWidth - this._item.offsetLeft;
        }
        else
        {
            if (this._item.offsetTop + size > this._container.clientHeight) size = this._container.clientHeight - this._item.offsetTop;
        }
        
        // Check that item is not too small
        if (size < this._intervalSize) size = this._intervalSize;
        
        // Resize to the closest interval
        size = Math.round(size / this._intervalSize) * this._intervalSize;
        if (this._resizeX)
        {
            this._item.style.width = size + "px";
        }
        else
        {
            this._item.style.height = size + "px";   
        }
        
        return size / this._intervalSize;
    }
}

export function newResizer(handleId, containerClass, cellCount, resizeX, obj) {
    return new Resizer(handleId, containerClass, cellCount, resizeX, obj);
}