/// <reference types="openlayers" />
import { OnDestroy, OnInit, EventEmitter } from '@angular/core';
import { interaction, EventsConditionType, style, StyleFunction, Collection, Feature } from 'openlayers';
import { MapComponent } from '../map.component';
export declare class ModifyInteractionComponent implements OnInit, OnDestroy {
    private map;
    instance: interaction.Modify;
    condition?: EventsConditionType;
    deleteCondition?: EventsConditionType;
    pixelTolerance?: number;
    style?: (style.Style | style.Style[] | StyleFunction);
    features: Collection<Feature>;
    wrapX?: boolean;
    onModifyEnd: EventEmitter<interaction.Modify.Event>;
    onModifyStart: EventEmitter<interaction.Modify.Event>;
    onChange: EventEmitter<interaction.Modify.Event>;
    onChangeActive: EventEmitter<interaction.Modify.Event>;
    onPropertyChange: EventEmitter<interaction.Modify.Event>;
    constructor(map: MapComponent);
    ngOnInit(): void;
    ngOnDestroy(): void;
}
