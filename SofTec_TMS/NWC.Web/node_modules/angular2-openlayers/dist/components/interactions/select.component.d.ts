/// <reference types="openlayers" />
import { OnDestroy, OnInit, EventEmitter } from '@angular/core';
import { interaction, EventsConditionType, layer, style, Collection, SelectFilterFunction, StyleFunction, Feature } from 'openlayers';
import { MapComponent } from '../map.component';
export declare class SelectInteractionComponent implements OnInit, OnDestroy {
    private map;
    instance: interaction.Select;
    addCondition?: EventsConditionType;
    condition?: EventsConditionType;
    layers?: (layer.Layer[] | ((layer: layer.Layer) => boolean));
    style?: (style.Style | style.Style[] | StyleFunction);
    removeCondition?: EventsConditionType;
    toggleCondition?: EventsConditionType;
    multi?: boolean;
    features?: Collection<Feature>;
    filter?: SelectFilterFunction;
    wrapX?: boolean;
    onChange: EventEmitter<interaction.Select.Event>;
    onSelect: EventEmitter<interaction.Select.Event>;
    onPropertyChange: EventEmitter<interaction.Select.Event>;
    constructor(map: MapComponent);
    ngOnInit(): void;
    ngOnDestroy(): void;
}
