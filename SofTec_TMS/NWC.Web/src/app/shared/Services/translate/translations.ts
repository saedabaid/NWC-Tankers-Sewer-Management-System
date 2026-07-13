// import translations
import { LANG_EN_NAME, LANG_EN_TRANS } from "../../Resources/lang-en.res";
import { LANG_AR_TRANS, LANG_AR_NAME } from "../../Resources/lang-ar.res";

// translation token
export const TRANSLATIONS = 'translations';

// all traslations
export const dictionary = {
	[LANG_EN_NAME]: LANG_EN_TRANS,
	[LANG_AR_NAME]: LANG_AR_TRANS
};

// providers
export const TRANSLATION_PROVIDERS = [
	{ provide: TRANSLATIONS, useValue: dictionary },
];