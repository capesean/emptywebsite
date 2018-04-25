// see: http://frhagn.github.io/Typewriter/ 
// or: https://github.com/frhagn/Typewriter

module Models {  
    export interface UserDTO { 
        $save: Function;
        id: string;
        email: string;
        roleIds: string[];
    }
    export interface UserResource {
        new (): UserDTO;
        get: Function;
        delete: Function;
        query: Function;
        sort: Function;
    }


}