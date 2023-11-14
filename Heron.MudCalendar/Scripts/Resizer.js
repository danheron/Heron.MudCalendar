export class Resizer {
    
    _obj;
    _handle;
    _intervalSize;
    _item;
    _startHeight;
    _startY;
    
    constructor(handleId, intervalSize, obj) {
        this._obj = obj;
        this._handle = document.getElementById(handleId);
        this._intervalSize = intervalSize;
        this._item = this._handle.parentElement;
        
        this._handle.addEventListener("mousedown", this.onMouseDown);
    }
    
    onMouseDown = (e) => {
        e.stopPropagation();
        
        this._startHeight = this._item.clientHeight;
        this._startY = e.clientY;
        
        document.addEventListener("mouseup", this.onMouseUp);
        document.addEventListener("mousemove", this.onMouseMove);
    }
    
    onMouseUp = async () => {
        document.removeEventListener("mouseup", this.onMouseUp);
        document.removeEventListener("mousemove", this.onMouseMove);
        
        const newHeight = this.update();
        
        await this._obj.invokeMethodAsync("ResizeFinished", newHeight);
    }
    
    onMouseMove = (e) => {
        this._position = e.clientY;
        
        this.update();
    }
    
    update() {
        const movement = this._position - this._startY;
        let height = this._startHeight + movement;
        if (height < this._intervalSize) height = this._intervalSize;
        
        // Find closest interval
        height = Math.round(height / this._intervalSize) * this._intervalSize;
        
        this._item.style.height = height + "px";
        
        return height;
    }
}

export function newResizer(handleId, intervalSize, obj) {
    return new Resizer(handleId, intervalSize, obj);
}