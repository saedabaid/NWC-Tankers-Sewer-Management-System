"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var openlayers_1 = require("openlayers");
var map_component_1 = require("../map.component");
var ModifyInteractionComponent = (function () {
    function ModifyInteractionComponent(map) {
        this.map = map;
        this.onModifyEnd = new core_1.EventEmitter();
        this.onModifyStart = new core_1.EventEmitter();
        this.onChange = new core_1.EventEmitter();
        this.onChangeActive = new core_1.EventEmitter();
        this.onPropertyChange = new core_1.EventEmitter();
    }
    ModifyInteractionComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.instance = new openlayers_1.interaction.Modify(this);
        this.instance.on('change', function (event) { return _this.onChange.emit(event); });
        this.instance.on('change:active', function (event) { return _this.onChangeActive.emit(event); });
        this.instance.on('propertychange', function (event) { return _this.onPropertyChange.emit(event); });
        this.instance.on('modifyend', function (event) { return _this.onModifyEnd.emit(event); });
        this.instance.on('modifystart', function (event) { return _this.onModifyStart.emit(event); });
        this.map.instance.addInteraction(this.instance);
    };
    ModifyInteractionComponent.prototype.ngOnDestroy = function () {
        this.map.instance.removeInteraction(this.instance);
    };
    return ModifyInteractionComponent;
}());
ModifyInteractionComponent.decorators = [
    { type: core_1.Component, args: [{
                selector: 'aol-interaction-modify',
                template: ''
            },] },
];
/** @nocollapse */
ModifyInteractionComponent.ctorParameters = function () { return [
    { type: map_component_1.MapComponent, },
]; };
ModifyInteractionComponent.propDecorators = {
    'condition': [{ type: core_1.Input },],
    'deleteCondition': [{ type: core_1.Input },],
    'pixelTolerance': [{ type: core_1.Input },],
    'style': [{ type: core_1.Input },],
    'features': [{ type: core_1.Input },],
    'wrapX': [{ type: core_1.Input },],
    'onModifyEnd': [{ type: core_1.Output },],
    'onModifyStart': [{ type: core_1.Output },],
    'onChange': [{ type: core_1.Output },],
    'onChangeActive': [{ type: core_1.Output },],
    'onPropertyChange': [{ type: core_1.Output },],
};
exports.ModifyInteractionComponent = ModifyInteractionComponent;
//# sourceMappingURL=modify.component.js.map