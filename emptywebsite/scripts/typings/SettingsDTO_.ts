// see: http://frhagn.github.io/Typewriter/ 
// or: https://github.com/frhagn/Typewriter

module Models {  
    export interface SettingsDTO { 
        $save: Function;
    }
    export interface SettingsResource {
        new (): SettingsDTO;
        get: Function;
        delete: Function;
        query: Function;
        sort: Function;
    }


}