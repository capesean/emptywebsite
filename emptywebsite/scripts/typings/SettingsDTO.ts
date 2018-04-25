// see: http://frhagn.github.io/Typewriter/ 
// or: https://github.com/frhagn/Typewriter

module Models {  
    export interface SettingsDTO { 
        $save: Function;
        rootUrl: string;
        siteName: string;
        roles: RoleDTO[];
    }
    export interface SettingsResource {
        new (): SettingsDTO;
        get: Function;
        delete: Function;
        query: Function;
        sort: Function;
    }
 
    export interface EnumDTO { 
        $save: Function;
        id: number;
        name: string;
        label: string;
    }
    export interface EnumResource {
        new (): EnumDTO;
        get: Function;
        delete: Function;
        query: Function;
        sort: Function;
    }
 
    export interface RoleDTO { 
        $save: Function;
        id: string;
        name: string;
        label: string;
    }
    export interface RoleResource {
        new (): RoleDTO;
        get: Function;
        delete: Function;
        query: Function;
        sort: Function;
    }


}