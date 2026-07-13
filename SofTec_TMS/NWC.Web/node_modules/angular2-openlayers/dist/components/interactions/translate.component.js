"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var openlayers_1 = require("openlayers");
var map_component_1 = require("../map.component");
var TranslateInteractionComponent = (function () {
    function TranslateInteractionComponent(map) {
        this.map = map;
    }
    TranslateInteractionComponent.prototype.ngOnInit = function () {
        this.instance = new openlayers_1.interaction.Translate(this);
        this.map.instance.addInteraction(this.instance);
    };
    TranslateInteractionComponent.prototype.ngOnDestroy = function () {
        this.map.instance.removeInteraction(this.instance);
    };
    return TranslateInteractionComponent;
}());
TranslateInteractionComponent.decorators = [
    { type: core_1.Component, args: [{
                selector: 'aol-interaction-translate',
                template: ''
            },] },
];
/** @nocollapse */
TranslateInteractionComponent.ctorParameters = function () { return [
    { type: map_component_1.MapComponent, },
]; };
TranslateInteractionComponent.propDecorators = {
    'features': [{ type: core_1.Input },],
    'layers': [{ type: core_1.Input },],
};
exports.TranslateInteractionComponent = TranslateInteractionComponent;
//# sourceMappingURL=translate.component.js.map