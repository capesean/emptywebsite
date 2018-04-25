// see: http://frhagn.github.io/Typewriter/ 
// or: https://github.com/frhagn/Typewriter

module Models {  
    export interface ErrorDTO { 
        $save: Function;
        id: string;
        date: Date;
        message: string;
        url: string;
        form: string;
        userName: string;
        exceptionId: string;
        exception: ErrorExceptionDTO;
    }
    export interface ErrorResource {
        new (): ErrorDTO;
        get: Function;
        delete: Function;
        query: Function;
        sort: Function;
    }
 
    export interface ErrorExceptionDTO { 
        $save: Function;
        id: string;
        message: string;
        stackTrace: string;
        innerExceptionId: string;
        innerException: ErrorExceptionDTO;
    }
    export interface ErrorExceptionResource {
        new (): ErrorExceptionDTO;
        get: Function;
        delete: Function;
        query: Function;
        sort: Function;
    }


}