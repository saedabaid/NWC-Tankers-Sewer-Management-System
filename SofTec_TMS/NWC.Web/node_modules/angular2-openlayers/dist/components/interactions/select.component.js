"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var openlayers_1 = require("openlayers");
var map_component_1 = require("../map.component");
var SelectInteractionComponent = (function () {
    function SelectInteractionComponent(map) {
        this.map = map;
        this.onChange = new core_1.EventEmitter();
        this.onSelect = new core_1.EventEmitter();
        this.onPropertyChange = new core_1.EventEmitter();
    }
    SelectInteractionComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.instance = new openlayers_1.interaction.Select(this);
        this.instance.on('change', function (event) { return _this.onChange.emit(event); });
        this.instance.on('select', function (event) { return _this.onSelect.emit(event); });
        this.instance.on('propertychange', function (event) { return _this.onPropertyChange.emit(event); });
        this.map.instance.addInteraction(this.instance);
    };
    SelectInteractionComponent.prototype.ngOnDestroy = function () {
        this.map.instance.removeInteraction(this.instance);
    };
    return SelectInteractionComponent;
}());
SelectInteractionComponent.decorators = [
    { type: core_1.Component, args: [{
                selector: 'aol-interaction-select',
                template: ''
            },] },
];
/** @nocollapse */
SelectInteractionComponent.ctorParameters = function () { return [
    { type: map_component_1.MapComponent, },
]; };
SelectInteractionComponent.propDecorators = {
    'addCondition': [{ type: core_1.Input },],
    'condition': [{ type: core_1.Input },],
    'layers': [{ type: core_1.Input },],
    'style': [{ type: core_1.Input },],
    'removeCondition': [{ type: core_1.Input },],
    'toggleCondition': [{ type: core_1.Input },],
    'multi': [{ type: core_1.Input },],
    'features': [{ type: core_1.Input },],
    'filter': [{ type: core_1.Input },],
    'wrapX': [{ type: core_1.Input },],
    'onChange': [{ type: core_1.Output },],
    'onSelect': [{ type: core_1.Output },],
    'onPropertyChange': [{ type: core_1.Output },],
};
exports.SelectInteractionComponent = SelectInteractionComponent;
//# sourceMappingURL=select.component.js.map