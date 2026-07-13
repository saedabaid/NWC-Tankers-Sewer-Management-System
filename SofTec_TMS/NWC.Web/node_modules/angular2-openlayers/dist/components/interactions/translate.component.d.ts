/// <reference types="openlayers" />
import { OnDestroy, OnInit } from '@angular/core';
import { interaction, layer, Collection, Feature } from 'openlayers';
import { MapComponent } from '../map.component';
export declare class TranslateInteractionComponent implements OnInit, OnDestroy {
    private map;
    instance: interaction.Translate;
    features?: Collection<Feature>;
    layers?: (layer.Layer[] | ((layer: layer.Layer) => boolean));
    constructor(map: MapComponent);
    ngOnInit(): void;
    ngOnDestroy(): void;
}
